using Photon.Deterministic;

namespace Quantum
{
    public unsafe partial struct TilePathfinder {

        public bool IsMoving => TargetPosition >= 0 && CurrentWaypoint >= 0;

        public void SetTarget(Frame f, TilePathfinder* tilePathfinder, FPVector3 position) {
            var map = f.GetSingleton<RuntimeTileMap>();
            var newPosition = map.Asset(f).PositionToIndex(position);
            if(newPosition != tilePathfinder->TargetPosition) {
                Repath(f);
                tilePathfinder->TargetPosition = newPosition;
            }
        }

        public void SetTarget(Frame f, TilePathfinder* tilePathfinder, int position) {
            if (position != tilePathfinder->TargetPosition) {
                Repath(f);
                tilePathfinder->TargetPosition = position;
            }
        }

        public void Repath(Frame f) {
            f.ResolveList(Waypoints).Clear();
        }
    }
}
