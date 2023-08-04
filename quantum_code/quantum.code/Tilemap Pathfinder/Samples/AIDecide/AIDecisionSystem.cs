using Photon.Deterministic;
using Quantum.Collections;
using Quantum.Tiles;
using System;
using System.Collections.Generic;
using static Quantum.Navigation;

namespace Quantum.Sample
{
    public unsafe struct IADecisionFilter
    {
        public EntityRef Entity;
        public TilePathfinder* TilePathfinder;
        public AIDecision* IADecision;
        public Transform3D* Transform;
    }

    // This system is used to find the cost of movement to interesting points in the map.
    // It used Dijkstra algorithm and a callback is used to decide what to do with each entity

    public unsafe class AIDecisionSystem : SystemMainThreadFilter<IADecisionFilter>, 
        ISignalOnComponentAdded<AIDecision>, 
        ISignalOnComponentRemoved<AIDecision>,
        ISignalOnTileMapWaypointReached
    {

        PathfinderData pathfinderData = new PathfinderData();
        TileMapData asset;

        public override void Update(Frame f, ref IADecisionFilter filter) {

            var map = f.GetSingleton<RuntimeTileMap>();
            var position = asset.PositionToIndex(filter.Transform->Position);
            var items = f.ResolveDictionary(filter.IADecision->Targets);
            var agent = f.FindAsset<TileAgentConfig>(filter.TilePathfinder->Agent.Id);

            // Does the Dijkstra and add all items found to de list of items

            if (items.Count == 0) {
                Dijkstra(pathfinderData, f, position, agent.MaxCostOfSearch, (data) => 
                {
                    items.Add(data.Entity, data.Cost);
                });
            }

            // Decide which is the better item to chase and set the target

            if (items.Count > 0 && !filter.TilePathfinder->IsMoving) {

                filter.IADecision->CurrentTarget = Decide(items);
                var target = f.Get<Transform3D>(filter.IADecision->CurrentTarget).Position;

                // If the current target has the same position of player
                if (asset.PositionToIndex(target) == position) {
                    f.ResolveDictionary(filter.IADecision->Targets).Remove(filter.IADecision->CurrentTarget);
                    filter.IADecision->CurrentTarget = Decide(items);
                }

                filter.TilePathfinder->SetTarget(f, filter.TilePathfinder, target);
            }
        }

        public override void OnInit(Frame f) {
            asset = f.GetSingleton<RuntimeTileMap>().Asset(f);
        }

        //Return the nearest item to chase
        public EntityRef Decide(QDictionary<EntityRef,int> targets) {

            int cost = int.MaxValue;
            EntityRef currentTarget = EntityRef.None;
            foreach (var target in targets) {
                if (target.Value <= cost) {
                    currentTarget = target.Key;
                    cost = target.Value;
                }
            }
            return currentTarget;
        }

        public void OnAdded(Frame f, EntityRef entity, AIDecision* component) {
            component->Targets = f.AllocateDictionary<EntityRef,int>();
        }

        public void OnRemoved(Frame f, EntityRef entity, AIDecision* component) {
             f.FreeDictionary(component->Targets);
        }

        public void OnTileMapWaypointReached(Frame f, EntityRef entity, FPVector3 waypoint, WaypointStatus status) {

            if (status != WaypointStatus.Final) return;
            if (!f.Unsafe.TryGetPointer<AIDecision>(entity, out var IADecision)) return;

            // When the agent reaches the final waypoint, it decide which is the next to chase and if
            // it already chase all items, new waypoints will be found in the Update function
                    
            var targets = f.ResolveDictionary(IADecision->Targets).Remove(IADecision->CurrentTarget);

            if (f.Unsafe.TryGetPointer<TilePathfinder>(entity, out var pathfinder)) {
                IADecision->CurrentTarget = Decide(f.ResolveDictionary(IADecision->Targets));

                if (IADecision->CurrentTarget != EntityRef.None) {
                    pathfinder->SetTarget(f, pathfinder, f.Get<Transform3D>(IADecision->CurrentTarget).Position);
                }
            }
        }

        public void Dijkstra(PathfinderData data, Frame f, int start, int maxCost, Action<SearchData> action) {

            var map = f.GetSingleton<RuntimeTileMap>();
            var items = f.ResolveDictionary(f.Global->TileMapItems);

            data.Frontier.Init();
            data.TempPath.Clear();
            data.TempPath[(ushort)start] = new PathData();
            data.Frontier.Enqueue( (ushort)start, 0);


            while (data.Frontier.Count > 0) {

                int currentIndex = data.Frontier.Dequeue();

                // Return if the max cost of search is reach
                if (data.TempPath[currentIndex].Cost >= maxCost) {
                    return;

                // Check if there is some item in this position of map
                } else {
                    if (action != null && items.ContainsKey(currentIndex)) {
                        var list = f.ResolveList(items[currentIndex].Items);
                        foreach (var item in list) {
                            action(new SearchData() {
                                Entity = item,
                                Cost = data.TempPath[currentIndex].Cost
                            });
                        }
                    }
                }

                // Get the neighborn list that was baked in the map tool
                int newCost = data.TempPath[currentIndex].Cost + asset.GetCost(currentIndex);
                foreach (int neighbor in asset.GetNeighbors(currentIndex)) {

                    if (neighbor < 0 || map.IsWall(f, neighbor)) continue;

                    if (!data.TempPath.ContainsKey(neighbor) || newCost < data.TempPath[neighbor].Cost) {
                        data.TempPath[neighbor] = new PathData() {
                            CameFrom = currentIndex,
                            Cost = newCost
                        };
                        data.Frontier.Enqueue((ushort)neighbor, (ushort)(newCost));

                    }
                }
            }
        }
    }
}
