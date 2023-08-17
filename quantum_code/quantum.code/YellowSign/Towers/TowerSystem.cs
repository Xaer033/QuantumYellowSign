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
            
            var towerConfig = f.FindAsset<TowerConfig>(tower->Config.Id);
            
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
                                                    towerConfig.TargetMask.BitMask);

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
                
                FPVector3 aimAtPosition = CalculateAimAheadPoint(f, towerConfig, tower, towerTransform.Position, tower->TargetEntityRef, targetTransform->Position);
                
                tower->AimAtPosition  = targetTransform->Position;
                tower->BarrelRotation = FPQuaternion.LookRotation(aimAtPosition - towerTransform.Position);

                tower->ReloadTime -= f.DeltaTime;
            }
        }
        
        public void OnProjectileSpawned(Frame f, EntityRef entity, EntityRef spawningTower)
        {
            f.Unsafe.TryGetPointer<Tower>(spawningTower, out var tower);
            var towerConfig = f.FindAsset<TowerConfig>(tower->Config.Id);

            tower->ReloadTime = towerConfig.ReloadDuration;
        }

        private FPVector3 CalculateAimAheadPoint(FrameThreadSafe f, TowerConfig towerConfig, Tower* tower, FPVector3 towerPosition, EntityRef target, FPVector3 targetPosition)
        {
            var targetAgent = f.Get<TilePathfinder>(target);
            var targetAgentConfig = f.FindAsset<TileAgentConfig>(targetAgent.Agent.Id);
            var targetCreep = f.Get<Creep>(target);
            
            var projectilePrototype   = f.FindAsset<EntityPrototype>(towerConfig.ProjectilePrototypeRef.AssetId);
            var projectileConfigAsset = f.FindAsset<ProjectileConfig>("Resources/DB/Gameplay/Projectiles/Bullet/BulletConfig");
            
            FPVector3 toTargetDirection  = targetPosition - towerPosition;
            FPVector3 targetVelocity     = targetAgent.IsMoving ? (targetCreep.Direction * targetAgentConfig.Velocity) : FPVector3.Zero;
            FP        projectileVelocity = projectileConfigAsset.BaseSpeed;

            FP h = toTargetDirection.Magnitude;
            FP a = FPVector3.Dot(targetVelocity, targetVelocity) - (projectileVelocity * projectileVelocity);
            FP b = FP._2 * FPVector3.Dot(targetVelocity, toTargetDirection);
            FP c = FPVector3.Dot(toTargetDirection, toTargetDirection);

            FPVector3 aimAtPosition = targetPosition;
           // if (a > FP._0)
            {
                FP p       = -b / (FP._2 * a);
                FP q       = FPMath.Sqrt((b * b) - (FP._4 * a * c)) / (FP._2 * a);
                
                FP t1      = p - q;
                FP t2      = p + q;
                FP t       = t1;
                
                if (t1 > t2 && t2 > FP._0)
                {
                    t = t2;
                }

                if (t < 0)
                {
                    return targetPosition;
                }
                
                aimAtPosition = targetPosition + (targetVelocity) * t;
            }
            
                // FPVector3 bulletPath   = aimSpot - towerTransform.Position;
                // FP        timeToImpact = bulletPath.Magnitude / projectileConfig.BaseSpeed;
            return aimAtPosition;
        }
    }
}
