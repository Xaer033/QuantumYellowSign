
using Photon.Deterministic;
namespace Quantum.YellowSign
{
    public unsafe class ProjectileSpawnSystem : SystemMainThreadFilter<ProjectileSpawnSystem.SpawnFilter>             
    {
        public unsafe struct SpawnFilter
        {
            public EntityRef Entity;
            public Tower* Tower;
        }
        
        public override void Update(Frame f, ref SpawnFilter filter)
        {
            if (filter.Tower->ReloadTime > FP._0 || filter.Tower->AiState != (byte)TowerAiState.TargetAcquired)
                return;

            var towerTransform   = f.Get<Transform3D>(filter.Entity);
            
            var towerConfig      = f.FindAsset<TowerConfig>(filter.Tower->Config.Id);
            var entityPrototype  = f.FindAsset<EntityPrototype>(towerConfig.ProjectilePrototypeRef.AssetId);
            var projectileEntity = f.Create(entityPrototype);

            
            f.Unsafe.TryGetPointer<Projectile>(projectileEntity, out var projectile);
            f.Unsafe.TryGetPointer<Transform3D>(projectileEntity, out var projectileTransform);
            var projectileConfig = f.FindAsset<ProjectileConfig>(projectile->Config.Id);

            projectileTransform->Position = towerTransform.Position;
            
            projectile->Speed             = projectileConfig.BaseSpeed;
            projectile->Damage            = projectileConfig.BaseDamage;
            projectile->SelfDestructTimer = projectileConfig.SelfDestructTimeInSeconds;
            projectile->TargetEntityRef   = filter.Tower->TargetEntityRef;
            
            f.Signals.OnProjectileSpawned(projectileEntity, filter.Entity);
        }
    }
}
