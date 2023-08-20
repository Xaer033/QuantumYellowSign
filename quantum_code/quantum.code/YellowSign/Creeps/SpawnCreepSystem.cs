using Photon.Deterministic;
namespace Quantum.YellowSign;

public unsafe class SpawnCreepSystem  : SystemMainThread
{
    public override void Update(Frame f)
    {   
        for (int i = 0; i < f.PlayerCount; i++)
        {
            if (f.GetPlayerCommand(i) is SpawnCreepsCommand spawnCommand)
            {
                var spawnPointList = f.Filter<SpawnPoint, Transform3D>();
                var endGoalList    = f.Filter<EndGoal, Transform3D>();
                
                if (!endGoalList.Next(out EntityRef _, out EndGoal _, out Transform3D endGoalTransform))
                    return;

                EntityPrototype creepPrototype = f.FindAsset<EntityPrototype>(spawnCommand.PrototypeGUID);
                
                while (spawnPointList.Next(
                    out EntityRef _, 
                    out SpawnPoint _, 
                    out Transform3D spawnTransform))
                {
                    FPVector3 spawnPos = spawnTransform.Position;

                    EntityRef creepEntity    = f.Create(creepPrototype);
                    f.Unsafe.TryGetPointer<Transform3D>(creepEntity, out var creepTransform);
                    creepTransform->Position = spawnPos;

                    f.Unsafe.TryGetPointer<TilePathfinder>(creepEntity, out var creepAgent);
                    creepAgent->SetTarget(f, creepAgent, endGoalTransform.Position);

                    f.Unsafe.TryGetPointer<PlayerLink>(creepEntity, out var playerLink);
                    playerLink->Player = i + 1;
                }
            }
        }
    }
}
