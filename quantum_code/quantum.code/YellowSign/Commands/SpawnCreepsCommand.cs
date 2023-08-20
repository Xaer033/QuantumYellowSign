using Photon.Deterministic;
namespace Quantum.YellowSign;

public unsafe class SpawnCreepsCommand : DeterministicCommand
{
    public long PrototypeGUID;

    public override void Serialize(BitStream stream)
    {
        stream.Serialize(ref PrototypeGUID);
    }
}
