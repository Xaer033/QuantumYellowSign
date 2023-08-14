
using Photon.Deterministic;
namespace Quantum.YellowSign
{
    public unsafe class HealthSystem : SystemMainThreadFilter<HealthSystem.HealthFilter>, ISignalOnProjectileHit
    {
        public unsafe struct HealthFilter
        {
            public EntityRef        Entity;
            public HealthContainer* HealthContainer;
        }

        public override void Update(Frame f, ref HealthFilter filter)
        {
            if (filter.HealthContainer->Health <= FP._0)
            {
                f.Destroy(filter.Entity);
            }
        }

        public void OnProjectileHit(Frame f, EntityRef projectileEntity, EntityRef targetEntity, FP damage)
        {
            if (!f.Unsafe.TryGetPointer<HealthContainer>(targetEntity, out var healthContainer))
                return;

            healthContainer->Health = FPMath.Max(0, healthContainer->Health - damage);
        }
    }
}
