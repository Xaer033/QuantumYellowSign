using Photon.Deterministic;
using Quantum.Physics3D;

namespace Quantum.YellowSign
{
    public unsafe class ProjectileSystem : SystemMainThreadFilter<ProjectileSystem.ProjectileFilter>,
                                            ISignalOnProjectileSpawned
    {
        public unsafe struct ProjectileFilter
        {
            public EntityRef Entity;
            public Projectile* Projectile;
        }
        
        public override void Update(Frame f, ref ProjectileFilter filter)
        {
            var projectile = filter.Projectile;
            var entity = filter.Entity;
            
            
            if (projectile->SelfDestructTimer <= FP._0)
            {
                f.Destroy(entity);
                return;
            }
            
            var projectileConfig = f.FindAsset<ProjectileConfig>(projectile->Config.Id);
            f.Unsafe.TryGetPointer<Transform3D>(entity, out var projectileTransform);

            if (!f.TryGet<Transform3D>(projectile->TargetEntityRef, out var targetTransform))
            {
                f.Destroy(entity);
                return;
            }

            FPVector3 aimDirection = (targetTransform.Position - projectileTransform->Position).Normalized;
            FP distance = projectile->Speed * f.DeltaTime;

            Hit3D? hit3D = f.Physics3D.Raycast(projectileTransform->Position, aimDirection, distance, projectileConfig.TargetLayerMask.BitMask);

            if (hit3D != null)
            {
                // TODO: Deal damage
                
                projectileTransform->Position = hit3D.Value.Point;
                projectile->SelfDestructTimer = 0;
                return;
            }
            
            projectileTransform->Position += aimDirection * distance;
            projectile->SelfDestructTimer -= f.DeltaTime;
        }
        
        public void OnProjectileSpawned(Frame f, EntityRef entity, EntityRef spawningTower)
        {
            // f.Get<entity
        }
    }
}
