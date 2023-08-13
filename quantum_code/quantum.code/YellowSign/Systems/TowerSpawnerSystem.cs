using Photon.Deterministic;
namespace Quantum.YellowSign;

public unsafe class TowerSpawnerSystem : SystemMainThread,
        ISignalOnComponentAdded<RuntimeTowerMap>
{
    private TileMapData _tileMapData;

    public override void OnInit(Frame f)
    {
        _tileMapData = f.GetSingleton<RuntimeTileMap>().Asset(f);
    }
    
    public void OnAdded(Frame f, EntityRef entity, RuntimeTowerMap* component)
    {
        component->Load(f);
    }

    public override void Update(Frame f)
    {
        var map     = f.GetSingleton<RuntimeTileMap>();
        var towerMap = f.GetSingleton<RuntimeTowerMap>();
        
        for (int i = 0; i < f.PlayerCount; i++) 
        {
            if (f.GetPlayerCommand(i) is CommandSpawnTower spawnCommand) 
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
                towerPlayerLink->Player = spawnCommand.PlayerOwner;
                
                map.SetTile(f, spawnCommand.GridPositionIndex, TileType.Wall);
                towerMap.SetTower(f, spawnCommand.GridPositionIndex, towerInstance);
                
                f.Events.EditTileMap(TileType.Wall, position);
            }
            else if (f.GetPlayerCommand(i) is CommandDestroyTower destroyCommand)
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
