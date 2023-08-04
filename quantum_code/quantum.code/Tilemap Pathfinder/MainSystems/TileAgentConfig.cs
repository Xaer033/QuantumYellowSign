using Photon.Deterministic;

namespace Quantum
{
    public enum MovementType {
        TRANSFORM = 0,
        KCC = 1,
        CUSTOM = 2
    }
    public unsafe partial class TileAgentConfig
    {
        public MovementType MovementType;
        public FP Velocity;
        public FP DistanceToReach;
        public byte MaxNumberOfWaypoints;
        public int MaxCostOfSearch;
        public bool DrawGizmos;
    }
}
