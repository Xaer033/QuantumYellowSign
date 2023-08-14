using Photon.Deterministic;
using Quantum.Physics3D;
using Quantum.Task;

namespace Quantum.YellowSign
{
    public unsafe class TowerSystem : SystemThreadedFilter<TowerSystem.TowerFilter>, ISignalOnProjectileSpawned
    {
        public unsafe struct TowerFilter
        {
            public EntityRef Entity;
            public Tower* Tower;
        }

        public override void Update(FrameThreadSafe f, ref TowerFilter filter)
        {
            var tower = filter.Tower;
            var entity = filter.Entity;
            
            var towerConfigAsset = f.FindAsset<TowerConfig>(tower->Config.Id);
            
            f.TryGet<Transform3D>(entity, out var towerTransform);
                
            if (tower->AiState == (byte)TowerAiState.SearchingForTarget)
            {
                if (tower->ThinkTickTimer >= 0)
                {
                    tower->ThinkTickTimer -= f.DeltaTime;
                    return;
                }
                
                var hitCollection = f.Physics3D.OverlapShape(
                                                    towerTransform, 
                                                    Shape3D.CreateSphere(tower->Range),
                                                    towerConfigAsset.TargetMask.BitMask);

                if (hitCollection.Count > 0)
                {
                    Hit3D hit = hitCollection[0];
                    tower->TargetEntityRef = hit.Entity;
                    tower->AiState         = (byte)TowerAiState.TargetAcquired;
                }
                else
                {
                    tower->ThinkTickTimer = 1;
                }                                                      
            }
            else if (tower->AiState == (byte)TowerAiState.TargetAcquired)
            {
                if (!f.TryGetPointer<Transform3D>(tower->TargetEntityRef, out var targetTransform))
                {
                    tower->AiState = (byte)TowerAiState.SearchingForTarget;
                    return;
                }

                FP distance = FPVector3.Distance(targetTransform->Position, towerTransform.Position);
                if (distance > tower->Range)
                {
                    tower->AiState = (byte)TowerAiState.SearchingForTarget;
                    return;
                }

                FPVector3 forward = targetTransform->Position - towerTransform.Position;
                tower->BarrelRotation = FPQuaternion.LookRotation(forward);

                tower->ReloadTime -= f.DeltaTime;
            }
        }
        
        public void OnProjectileSpawned(Frame f, EntityRef entity, EntityRef spawningTower)
        {
            f.Unsafe.TryGetPointer<Tower>(spawningTower, out var tower);
            var towerConfig = f.FindAsset<TowerConfig>(tower->Config.Id);

            tower->ReloadTime = towerConfig.ReloadDuration;
        }
    }
}
