using Photon.Deterministic;


namespace Quantum
{
    public enum AttackType
    {
        HitScan = 0,
        Projectile   = 1,
        Melee   = 2,
    }

    public enum DamageType
    {
        None = 0,
        Normal,
        Fire,
        Ice,
        Venom,
        Chaos,
    }

    public enum TowerAiState : byte
    {
        None = 0,
        SearchingForTarget,
        TargetAcquired,
    }


    public unsafe partial class TowerConfig
    {
        public LayerMask  TargetMask;
        public AttackType AttackType;
        public DamageType DamageType;
        public FP         ReloadDuration;
        public FP         BaseRange;
        public FP         BaseDamage;
        public int        GoldPrice;
        public int        MaxHealth;

        public bool DrawGizmos;
    }
}
