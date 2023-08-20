using Photon.Deterministic;

namespace Quantum.YellowSign
{
        
    public unsafe class TowerSpawnSystem : SystemMainThread,
            ISignalOnComponentAdded<RuntimeTowerMap>,
            ISignalOnComponentAdded<Tower>,
            ISignalOnTileMapMoveAgent
    {
        private TileMapData _tileMapData;

        public override void OnInit(Frame f)
        {
            _tileMapData = f.GetSingleton<RuntimeTileMap>().Asset(f);
        }
        
        
        public void OnAdded(Frame f, EntityRef entity, Tower* component)
        {
            var towerConfigAsset = f.FindAsset<TowerConfig>(component->Config.Id);
                
            component->AiState    = (byte)TowerAiState.SearchingForTarget;
            component->Health     = towerConfigAsset.MaxHealth;
            component->Damage     = towerConfigAsset.BaseDamage;
            component->Range      = towerConfigAsset.BaseRange;
            component->ReloadTime = 0;
        }
        
        public void OnAdded(Frame f, EntityRef entity, RuntimeTowerMap* component)
        {
            component->Load(f);
        }
        
         // Check aways that the agent move if the next tile is a wall. So it can change the path
        public void OnTileMapMoveAgent(Frame f, EntityRef entity, FPVector3 desiredDirection) {

            var runtimeTileMap = f.GetSingleton<RuntimeTileMap>();
            var position = f.Get<Transform3D>(entity).Position + desiredDirection;

            var wallPosition = _tileMapData.PositionToIndex(position);

            if (runtimeTileMap.IsWall(f, wallPosition) 
                && FPVector3.Distance(_tileMapData.IndexToPosition(wallPosition), position) < FP._0_20) 
            {
                if (f.Unsafe.TryGetPointer<TilePathfinder>(entity, out var tilePathfinder)) 
                {
                    tilePathfinder->Repath(f);
                }
            }
        }

        public override void Update(Frame f)
        {
            var map     = f.GetSingleton<RuntimeTileMap>();
            
            if (!f.TryGetSingleton<RuntimeTowerMap>(out var towerMap))
            {
                return;
            }
            
            for (int i = 0; i < f.PlayerCount; i++) 
            {
                if (f.GetPlayerCommand(i) is SpawnTowerCommand spawnCommand) 
                {
                    bool isWall = map.IsWall(f, spawnCommand.GridPositionIndex);
                    if (isWall)
                        continue;
                        
                    FPVector3 position = _tileMapData.IndexToPosition(spawnCommand.GridPositionIndex);
                    
                    var entityPrototype = f.FindAsset<EntityPrototype>(spawnCommand.PrototypeGUID);
                    var towerInstance = f.Create(entityPrototype);
                   
                    f.Unsafe.TryGetPointer<Transform3D>(towerInstance, out var towerTransform);
                    f.Unsafe.TryGetPointer<PlayerLink>(towerInstance, out var towerPlayerLink);
                    
                    towerTransform->Position = position;
                    towerPlayerLink->Player  = i + 1;
                    
                    map.SetTile(f, spawnCommand.GridPositionIndex, TileType.Wall);
                    towerMap.SetTower(f, spawnCommand.GridPositionIndex, towerInstance);
                    
                    f.Events.EditTileMap(TileType.Wall, position);
                }
                else if (f.GetPlayerCommand(i) is DestroyTowerCommand destroyCommand)
                {
                    bool isWall = map.IsWall(f, destroyCommand.GridPositionIndex);
                    if (!isWall)
                        continue;
                       
                    if (!towerMap.TryGetTower(f, destroyCommand.GridPositionIndex, out var towerInstance))
                        continue;
                        
                    map.SetTile(f, destroyCommand.GridPositionIndex, TileType.None);
                    towerMap.SetTower(f, destroyCommand.GridPositionIndex, EntityRef.None);
                    
                    f.Destroy(towerInstance);
                }
            }
        }
    }
}
