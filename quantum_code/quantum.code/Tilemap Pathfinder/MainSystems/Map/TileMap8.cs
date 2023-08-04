using System;

namespace Quantum
{
    [Serializable]
    public class TileMap8 : TileMapData
    {
        public override int[] GetNeighborsOffset(int index) {
            return new int[8] { WIDTH, -WIDTH, 1, -1,WIDTH+1,WIDTH-1,-WIDTH-1,-WIDTH+1 };
        }
    }
}
