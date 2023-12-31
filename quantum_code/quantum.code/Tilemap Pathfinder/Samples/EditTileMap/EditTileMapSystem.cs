﻿using Photon.Deterministic;

namespace Quantum.Sample
{
    public unsafe struct EditTileMapFilter
    {
        public EntityRef Entity;
        public PlayerLink* PlayerController;
        public TilePathfinder* TilePathfinder;
    }
    public unsafe class EditTileMapSystem : SystemMainThreadFilter<EditTileMapFilter>, ISignalOnTileMapMoveAgent
    {
        TileMapData asset;

        public override void Update(Frame f, ref EditTileMapFilter filter) {

            var map     = f.GetSingleton<RuntimeTileMap>();
            
            // If the player click into the map to carve, it will edit the bitset in tuntimemap
            // and will send a event telling witch tile was changed

            for (int i = 0; i < f.PlayerCount; i++) {
                if (f.GetPlayerCommand(i) is CommandEditTile command) {

                    int index = asset.PositionToIndex(command.Position);

                    bool isWall            = map.IsWall(f, index);
                    bool commandTypeIsWall = command.TileType == (int)TileType.Wall;
                    
                    if (isWall == commandTypeIsWall)
                        continue;
                        
                    map.SetTile(f, index, (TileType)command.TileType);
                    
                    FPVector3 position = asset.IndexToPosition(index);
                    
                    var entityPrototype = f.FindAsset<EntityPrototype>("Resources/DB/Towers/SimpleTower/SimpleTower|EntityPrototype");
                    var towerInstance = f.Create(entityPrototype);
                    f.Unsafe.TryGetPointer<Transform3D>(towerInstance, out var towerTransform);
                    towerTransform->Position = position;
                    
                    f.Events.EditTileMap((TileType)command.TileType, position);
                }
            }
        }

        public override void OnInit(Frame f) {
            // Cache the tilemap asset reference
            asset = f.GetSingleton<RuntimeTileMap>().Asset(f);
        }

        // Check aways that the agent move if the next tile is a wall. So it can change the path
        public void OnTileMapMoveAgent(Frame f, EntityRef entity, FPVector3 desiredDirection) {

            var runtimeTileMap = f.GetSingleton<RuntimeTileMap>();
            var position = f.Get<Transform3D>(entity).Position + desiredDirection;

            var wallPosition = asset.PositionToIndex(position);

            if (runtimeTileMap.IsWall(f, wallPosition) && FPVector3.Distance(asset.IndexToPosition(wallPosition), position) < FP._0_20) {
                if (f.Unsafe.TryGetPointer<TilePathfinder>(entity, out var tilePathfinder)) {
                    tilePathfinder->Repath(f);
                }
            }
        }
    }
}
