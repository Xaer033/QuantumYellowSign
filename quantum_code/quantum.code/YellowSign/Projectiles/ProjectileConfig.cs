
using Photon.Deterministic;
namespace Quantum
{
    public unsafe partial class ProjectileConfig
    {
        public LayerMask  TargetLayerMask;  
        public DamageType DamageType;
        public FP         BaseDamage;
        public FP         BaseSpeed;

        public FP SelfDestructTimeInSeconds;
    }
}
