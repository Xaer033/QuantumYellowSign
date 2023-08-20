using Photon.Deterministic;
namespace Quantum.YellowSign
{
    public unsafe class DestroyTowerCommand : DeterministicCommand
    {
        public int GridPositionIndex;

        public override void Serialize(BitStream stream)
        {
            stream.Serialize(ref GridPositionIndex);
        }
    }
}
