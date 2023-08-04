using Photon.Deterministic;

namespace Quantum.Sample
{
    public unsafe struct CallbackMoveFilter
    {
        public EntityRef Entity;
        public PlayerLink* PlayerLink;
        public TilePathfinder* TilePathfinder;
    }

    // This sample of use sends to Unity event with the state of agent
    // In Unity side, the color of agent will be changed by theses states

    public unsafe class CallbackMoveSystem : SystemMainThreadFilter<CallbackMoveFilter>, 
        ISignalOnTileMapMoveAgent,
        ISignalOnTileMapWaypointReached,
        ISignalOnTileMapSearchFailed
    {

        public override void Update(Frame f, ref CallbackMoveFilter filter) {

        }

        public void OnTileMapMoveAgent(Frame f, EntityRef entity, FPVector3 desiredDirection) {

            // Moves the character in desired direction and apply the velocity of agent
            // This is used to customize the movement of check special conditions before move

            f.Events.ChangePlayerState(PlayerStates.Moving);
            var agent = f.FindAsset<TileAgentConfig>(f.Get<TilePathfinder>(entity).Agent.Id);

            if (f.Unsafe.TryGetPointer<Transform3D>(entity, out var transform3D)) {
                transform3D->Position += desiredDirection.Normalized * f.DeltaTime * agent.Velocity;
            }

        }

        public void OnTileMapWaypointReached(Frame f, EntityRef entity, FPVector3 waypoint, WaypointStatus status) {
            f.Events.ChangePlayerState(PlayerStates.Reach);
        }

        public void OnTileMapSearchFailed(Frame f, EntityRef entity) {
            f.Events.ChangePlayerState(PlayerStates.Fail);
        }
    }
}
