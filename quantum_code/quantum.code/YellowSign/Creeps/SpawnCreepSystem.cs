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
                var spawnPointList = f.Filter<SpawnPoint>();
                var endGoalList    = f.Filter<EndGoal>();

                if (!endGoalList.Next(out var endGoalEntity, out var _))
                    return;

                var endGoalTransform = f.Get<Transform3D>(endGoalEntity);
                    
                var entityPrototype = f.FindAsset<EntityPrototype>(spawnCommand.PrototypeGUID);
                
                while (spawnPointList.Next(out var spawnEntity, out var _))
                {
                    var spawnPointTransform = f.Get<Transform3D>(spawnEntity);
                    FPVector3 spawnPos = spawnPointTransform.Position;

                    var creepEntity    = f.Create(entityPrototype);
                    f.Unsafe.TryGetPointer<Transform3D>(creepEntity, out var creepTransform);
                    creepTransform->Position = spawnPos;

                    f.Unsafe.TryGetPointer<TilePathfinder>(creepEntity, out var creepAgent);
                    creepAgent->SetTarget(f, creepAgent, endGoalTransform.Position);
                }
            }
        }
    }
}
