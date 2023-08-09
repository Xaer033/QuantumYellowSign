using Photon.Deterministic;
using Quantum.Physics3D;
using Quantum.Sample;
using Quantum.Task;
namespace Quantum
{

    public unsafe class TowerSystem : SystemThreadedFilter<TowerSystem.TowerFilter>
    {
        public unsafe struct TowerFilter
        {
            public EntityRef Entity;
            public Tower* Tower;
        }

        // private HitCollection3D *_hitCollection3DCache;

        //
        // public override void Update(Frame f, ref TowerFilter filter)
        // {
        //     var map = f.GetSingleton<RuntimeTileMap>();
        //     
        //     var es = f.Filter<Tower>();
        //     while (es.NextUnsafe(out EntityRef entity, out var tower))
        //     {
        //         var towerConfigAsset = f.FindAsset<TowerConfig>(tower->Config.Id);
        //         
        //         f.TryGet<Transform3D>(entity, out var towerTransform);
        //             
        //         if (tower->AiState == (byte)TowerAiState.None)
        //         {
        //             tower->AiState    = (byte)TowerAiState.SearchingForTarget;
        //             tower->Health     = towerConfigAsset.MaxHealth;
        //             tower->Damage     = towerConfigAsset.BaseDamage;
        //             tower->Range      = towerConfigAsset.BaseRange;
        //             tower->ReloadTime = 0;
        //         }
        //         else if (tower->AiState == (byte)TowerAiState.SearchingForTarget)
        //         {
        //             if (tower->ThinkTickTimer >= 0)
        //             {
        //                 tower->ThinkTickTimer -= f.DeltaTime;
        //                 continue;
        //             }
        //             
        //             f.Physics3D.OverlapShape(_hitCollection3DCache,
        //                                                 towerTransform, 
        //                                                 Shape3D.CreateSphere(tower->Range),
        //                                                 towerConfigAsset.TargetMask.BitMask);
        //     
        //             if (_hitCollection3DCache->Count > 0)
        //             {
        //                 Hit3D hit = (*_hitCollection3DCache)[0];
        //                 tower->TargetEntityRef = hit.Entity;
        //                 tower->AiState         = (byte)TowerAiState.TargetAcquired;
        //             }
        //             else
        //             {
        //                 tower->ThinkTickTimer = 1;
        //             }                                                      
        //         }
        //         else if (tower->AiState == (byte)TowerAiState.TargetAcquired)
        //         {
        //             if (!f.Unsafe.TryGetPointer<Transform3D>(tower->TargetEntityRef, out var targetTransform))
        //             {
        //                 tower->AiState = (byte)TowerAiState.SearchingForTarget;
        //                 continue;
        //             }
        //     
        //             FP distance = FPVector3.Distance(targetTransform->Position, towerTransform.Position);
        //             if (distance > tower->Range)
        //             {
        //                 tower->AiState = (byte)TowerAiState.SearchingForTarget;
        //                 continue;
        //             }
        //     
        //             FPVector3 forward = targetTransform->Position - towerTransform.Position;
        //             tower->BarrelRotation = FPQuaternion.LookRotation(forward);
        //         }
        //     }
        // }

        // public override void OnEnabled(Frame f) 
        // {
        //     // Cache the tilemap asset reference
        //     // asset = f.GetSingleton<RuntimeTileMap>().Asset(f);
        //     _hitCollection3DCache = f.Physics3D.AllocatePersistentHitCollection3D(128);
        // }
        //
        // public override void OnDisabled(Frame f) 
        // {
        //     f.Physics3D.FreePersistentHitCollection3D(_hitCollection3DCache);
        // }

        public override void Update(FrameThreadSafe f, ref TowerFilter filter)
        {
            var map = f.GetSingleton<RuntimeTileMap>();

            var es = f.Filter<Tower>();
            while (es.NextUnsafe(out EntityRef entity, out var tower))
            {
                var towerConfigAsset = f.FindAsset<TowerConfig>(tower->Config.Id);
                
                f.TryGet<Transform3D>(entity, out var towerTransform);
                    
                if (tower->AiState == (byte)TowerAiState.None)
                {
                    tower->AiState    = (byte)TowerAiState.SearchingForTarget;
                    tower->Health     = towerConfigAsset.MaxHealth;
                    tower->Damage     = towerConfigAsset.BaseDamage;
                    tower->Range      = towerConfigAsset.BaseRange;
                    tower->ReloadTime = 0;
                }
                else if (tower->AiState == (byte)TowerAiState.SearchingForTarget)
                {
                    if (tower->ThinkTickTimer >= 0)
                    {
                        tower->ThinkTickTimer -= f.DeltaTime;
                        continue;
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
                        continue;
                    }

                    FP distance = FPVector3.Distance(targetTransform->Position, towerTransform.Position);
                    if (distance > tower->Range)
                    {
                        tower->AiState = (byte)TowerAiState.SearchingForTarget;
                        continue;
                    }

                    FPVector3 forward = targetTransform->Position - towerTransform.Position;
                    tower->BarrelRotation = FPQuaternion.LookRotation(forward);
                }
            }
        }
    }
}
