using Photon.Deterministic;
using Quantum.Collections;
using System.Collections.Generic;
namespace Quantum.Tiles
{
    public unsafe struct TilePathfinderFilter
    {
        public EntityRef Entity;
        public TilePathfinder* TilePathfinder;
    }

    public unsafe class TilePathfinderSystem : SystemMainThreadFilter<TilePathfinderFilter>,
        ISignalOnComponentAdded<TilePathfinder>,
        ISignalOnComponentRemoved<TilePathfinder>,
        ISignalOnComponentAdded<RuntimeTileMap>
    {

        // Cache the reference for temporary data used in pathfinder algorithm
        PathfinderData pathfinderData = new PathfinderData();
        TileMapData asset;

        public override void Update(Frame f, ref TilePathfinderFilter filter) {
  
            var map = f.GetSingleton<RuntimeTileMap>();
            var agent = f.FindAsset<TileAgentConfig>(filter.TilePathfinder->Agent.Id);
            var waypoints = f.ResolveList(filter.TilePathfinder->Waypoints);
            var pathfinder = filter.TilePathfinder;

            // Find the path is some valid target was set. If it still not found the target, it try again

            if (pathfinder->TargetPosition >= 0) {
                
                if (waypoints.Count == 0) {
                    FindPath(f, filter.Entity, pathfinder->TargetPosition);
                } else if ((pathfinder->TargetPosition != waypoints[0] && pathfinder->CurrentWaypoint < 0)) {
                    FindPath(f, filter.Entity, pathfinder->TargetPosition);
                    
                }
            }

            // Movement behaviour: The agent can move using transform, kcc, or customize with callbacks
            // In all cases, the waypoint is updated when the agent is near enough

            if (f.Unsafe.TryGetPointer<Transform3D>(filter.Entity, out var transform) && pathfinder->IsMoving ) {
                
                var nextWapoint = waypoints[pathfinder->CurrentWaypoint];
                FPVector3 next = asset.IndexToPosition(nextWapoint);

                // Draw gizmos on view
                if (agent.DrawGizmos) {
                    Draw.Circle(asset.IndexToPosition(pathfinder->TargetPosition), FP._1 / 4, ColorRGBA.Red);
                    for(int i = pathfinder->CurrentWaypoint; i >= 0; i--) {
                        Draw.Circle(asset.IndexToPosition(waypoints[i]), FP._1 / 4, ColorRGBA.Cyan);
                        if(i > 0) {
                            Draw.Line(asset.IndexToPosition(waypoints[i]), asset.IndexToPosition(waypoints[i-1]), ColorRGBA.Cyan);
                        }
                    }
                    Draw.Line(transform->Position, asset.IndexToPosition(waypoints[pathfinder->CurrentWaypoint]), ColorRGBA.Cyan);
                }

                // Transform movement
                if (agent.MovementType == MovementType.TRANSFORM) {
                    var direction = next - transform->Position;
                    transform->Position += direction.Normalized * f.DeltaTime * agent.Velocity;

                }

                // KCC movement
                if (agent.MovementType == MovementType.KCC) {

                    if (f.Unsafe.TryGetPointer<CharacterController3D>(filter.Entity, out var controller)) {
                        var direction = asset.IndexToPosition(nextWapoint) - transform->Position;
                        controller->Move(f, filter.Entity, direction.Normalized);
                    }
                }

                //Custom Movement
                if (agent.MovementType == MovementType.CUSTOM) {
                    var direction = next - transform->Position;
                    f.Signals.OnTileMapMoveAgent(filter.Entity, direction.Normalized);
                }
                
                //Update progress
                if (FPVector3.Distance(transform->Position, next) <= agent.DistanceToReach) {
                    pathfinder->CurrentWaypoint -= 1;
                    f.Signals.OnTileMapWaypointReached(filter.Entity, next, pathfinder->CurrentWaypoint < 0 ? WaypointStatus.Final : WaypointStatus.Next);
                }
            }

        }

        // Cache the asset reference. Must set deterministcally if you would like to change map
        public override void OnInit(Frame f) {
            asset = f.GetSingleton<RuntimeTileMap>().Asset(f);
        }

        // Setup the initial state of component with invalid valeues and allocate the waypoint list
        public void OnAdded(Frame f, EntityRef entity, TilePathfinder* component) {
            component->TargetPosition = -1;
            component->CurrentWaypoint = -1;
            component->Waypoints = f.AllocateList<int>();
        }

        public void OnRemoved(Frame f, EntityRef entity, TilePathfinder* component) {
            f.FreeList(component->Waypoints);
        }


        // Load all data of asset map into RuntimeTileMap to allow editing the tilemap in runtime, but not change the asset
        public void OnAdded(Frame f, EntityRef entity, RuntimeTileMap* component) {
            component->Load(f);
        }

        public enum PathFindStatus
        {
            NOT_FOUND,
            SUCCESS
        }

        // This function solves some problems while the pathfinding. It check if the target is different of
        // current position and path is no reachable

        public void FindPath(Frame f, EntityRef entity, int targetPosition) {

            var map = f.GetSingleton<RuntimeTileMap>();
            var pathfinder = f.Unsafe.GetPointer<TilePathfinder>(entity);
            var agent = f.FindAsset<TileAgentConfig>(pathfinder->Agent.Id);


            if (f.Unsafe.TryGetPointer<Transform3D>(entity, out var transform)) {

                var entityPosition = asset.PositionToIndex(transform->Position);
                var waypoints = f.ResolveList(pathfinder->Waypoints);

                // If the agent targeting your own position, sends a signal that the waypoint was reached
                // and clear the waypoints

                if (entityPosition == targetPosition) {
                    waypoints.Clear();
                    pathfinder->CurrentWaypoint = -1;
                    f.Signals.OnTileMapWaypointReached(entity, asset.IndexToPosition(targetPosition), WaypointStatus.Final);;
                    pathfinder->TargetPosition = -1;
                    return;
                }

                // Does the AStar algorithm to found the shortest path. If it not found the any path,
                // reset the setup

                var result = AStar(pathfinderData, asset, f, entityPosition, targetPosition, pathfinder->Waypoints, agent.MaxCostOfSearch, agent.MaxNumberOfWaypoints);

                if (result == PathFindStatus.NOT_FOUND) {
                    waypoints.Clear();
                    pathfinder->CurrentWaypoint = -1;
                    pathfinder->TargetPosition = -1;
                    f.Signals.OnTileMapSearchFailed(entity);
                    return;
                } else {
                    pathfinder->TargetPosition = targetPosition;
                    pathfinder->CurrentWaypoint = waypoints.Count - 1;
                }
            }
        }

        static public PathFindStatus AStar(PathfinderData data, TileMapData asset, Frame f, int start, int end, QListPtr<int> waypoints, int maxCost, byte limit = 8) {
            var map = f.GetSingleton<RuntimeTileMap>();
            if (asset.Size < end || map.IsWall(f, end)) {
                return PathFindStatus.NOT_FOUND;
            }

            data.Frontier.Init();
            data.TempPath.Clear();
            data.TempPath[(ushort)start] = new PathData();
            data.Frontier.Enqueue((ushort)start, 0);

            while (data.Frontier.Count > 0) {

                int currentIndex = data.Frontier.Dequeue();
                if (currentIndex == end) {
                    SetPath(data.TempPath, asset, f, waypoints, start, end, limit);
                    return PathFindStatus.SUCCESS;
                }

                int newCost = data.TempPath[currentIndex].Cost + asset.GetCost(currentIndex);
                if(newCost > maxCost) {
                    return PathFindStatus.NOT_FOUND;
                }

                // Get the neighborn list that was baked in the map tool
                foreach (int neighbor in asset.GetNeighbors(currentIndex)) {

                    if (neighbor < 0)  continue;

                    if (!data.TempPath.ContainsKey(neighbor) || newCost < data.TempPath[neighbor].Cost) {
                        if (!map.IsWall(f, neighbor)) {
                            data.TempPath[neighbor] = new PathData() {
                                CameFrom = currentIndex,
                                Cost = newCost
                            };
                            data.Frontier.Enqueue((ushort)neighbor, (ushort)(newCost + asset.Heuristic(neighbor, end)));
                        }
                    }
                }
                
            }

            // Fallback in case not found the end directally in the frontier
             
            if (!data.TempPath.ContainsKey(end)) {
                return PathFindStatus.NOT_FOUND;
            } else {
                SetPath(data.TempPath, asset, f , waypoints, start, end, limit);
                return PathFindStatus.SUCCESS;
            }
        }

        // Once that the path was found, this function does a backtrack to get all waypoints to move
        // Also, it compress the waypoints if a group of them are in the same direction
        // If the number of waypoints is on the limit, it remove the last
        // Perharps that the list starts from the last waypoints to the initials

        static public void SetPath(Dictionary<int, PathData> tempPath, TileMapData asset,Frame f, QListPtr<int> waypoints, int start, int target, byte limit) {

            var points = f.ResolveList(waypoints);
            int count = target;
            FPVector3 lastDirection = default;
            points.Clear();

            while (tempPath.ContainsKey(count) && count != start) {

                var currentDirection = asset.IndexToPosition(tempPath[count].CameFrom) - asset.IndexToPosition(count);
                if(currentDirection != lastDirection) {
                    points.Add(count);
                }
                if(points.Count > limit) {
                    points.RemoveAt(0);
                }
                count = tempPath[count].CameFrom;
                lastDirection = currentDirection;
            }
        }
    }
}
