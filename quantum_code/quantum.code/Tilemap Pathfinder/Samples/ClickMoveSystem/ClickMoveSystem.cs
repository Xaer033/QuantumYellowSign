
namespace Quantum.Sample
{
    public unsafe struct TilePathfinderFilter
    {
        public EntityRef Entity;
        public PlayerLink* PlayerController;
        public TilePathfinder* TilePathfinder;
    }
    public unsafe class ClickMoveSystem : SystemMainThreadFilter<TilePathfinderFilter>
    {
        public override void Update(Frame f, ref TilePathfinderFilter filter) {

            // Basic implementation of click to move where we set the target and the system will
            // try to find the path. If there is a valid path, the agent will start to move

            var input = f.GetPlayerInput(filter.PlayerController->Player);
            if (input->click) {
                filter.TilePathfinder->SetTarget(f, filter.TilePathfinder, input->position);
            }

        }
    }
}
