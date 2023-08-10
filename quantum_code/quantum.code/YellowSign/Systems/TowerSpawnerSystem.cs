using Photon.Deterministic;
namespace Quantum.YellowSign;

public unsafe class TowerSpawnerSystem : SystemMainThread
{
    private TileMapData _tileMapData;

    public override void Update(Frame f)
    {
        var map     = f.GetSingleton<RuntimeTileMap>();
         
        for (int i = 0; i < f.PlayerCount; i++) 
        {
            if (f.GetPlayerCommand(i) is CommandSpawnTower command) 
            {
                bool isWall = map.IsWall(f, command.GridPositionIndex);
                if (isWall)
                    continue;
                    
                map.SetTile(f, command.GridPositionIndex, TileType.Wall);
                
                FPVector3 position = _tileMapData.IndexToPosition(command.GridPositionIndex);
                
                var entityPrototype = f.FindAsset<EntityPrototype>(command.PrototypeGUID);
                var towerInstance = f.Create(entityPrototype);
               
                f.Unsafe.TryGetPointer<Transform3D>(towerInstance, out var towerTransform);
                f.Unsafe.TryGetPointer<PlayerLink>(towerInstance, out var towerPlayerLink);
                
                towerTransform->Position = position;
                towerPlayerLink->Player = command.PlayerOwner;
                
                f.Events.EditTileMap(TileType.Wall, position);
            }
        }
    }
    
    public override void OnInit(Frame f) 
    {
        _tileMapData = f.GetSingleton<RuntimeTileMap>().Asset(f);
    }
}
