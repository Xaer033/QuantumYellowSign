using Photon.Deterministic;
namespace Quantum.YellowSign
{
    public unsafe class CommandSpawnTower : DeterministicCommand
    {
        public long PrototypeGUID;
        public int GridPositionIndex;
        public PlayerRef PlayerOwner;

        public override void Serialize(BitStream stream)
        {
            stream.Serialize(ref PrototypeGUID);
            stream.Serialize(ref GridPositionIndex);
            stream.Serialize(ref PlayerOwner);
        }
    }
}
