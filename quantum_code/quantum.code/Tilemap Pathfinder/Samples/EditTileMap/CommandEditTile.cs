using Photon.Deterministic;

namespace Quantum.Sample
{
    public unsafe class CommandEditTile : DeterministicCommand
    {
        public FPVector3 Position;
        public int TileType;

        public override void Serialize(BitStream stream) {
            stream.Serialize(ref Position);
            stream.Serialize(ref TileType);
        }

    }
}
