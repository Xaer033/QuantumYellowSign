using System;
using Photon.Deterministic;

namespace Quantum
{

    public enum HexagonOffset
    {
        Odd = 1,
        Even = -1
    }
    public enum HexagonTop
    {
        Flat,
        Pointy
    }

    // This model of hexagonal map uses an offset between columns and rows and six neighborns
    // The HexagonTop will decide if the neighborns will be found in the nearest collumns or in the rows

    [Serializable]
    public class HexagonalMap : TileMapData
    {

        public HexagonOffset HexagonOffset;
        public HexagonTop HexagonTop;

        public override int[] GetNeighborsOffset(int index) {

            if (HexagonTop == HexagonTop.Pointy) {
                var cd = (index / WIDTH) % 2 == 1 ? 1 : -1;
                return new int[6] { WIDTH, 1, -WIDTH, -1, WIDTH + (int)HexagonOffset * cd, -WIDTH + (int)HexagonOffset * cd };

            } else {
                var ld = (index % WIDTH) % 2 == 1 ? 1 : -1;
                var offset = (int)HexagonOffset;
                return new int[6] { WIDTH, 1, -WIDTH, -1, WIDTH * offset * ld + offset, WIDTH * offset * ld - offset };
            }

        }

        public override FPVector3 IndexToPosition(int index) {

            int x = index % WIDTH;
            int y = index / HEIGHT;
            FP offsetX = 0;
            FP offsetY = 0;

            if (HexagonTop == HexagonTop.Pointy) {
                offsetX += (y % 2) * (TileWidth / 2) * (int)HexagonOffset;
            } else {
                offsetY += (x % 2) * (TileWidth / 2) * (int)HexagonOffset;
            }

            return new FPVector3((x * TileWidth) + offsetX, 0, (y * TileHeight) + offsetY) + Offset;

        }
        public override ushort Heuristic(int from, int to) {
            FPVector3 a = IndexToPosition(from);
            FPVector3 b = IndexToPosition(to);
            return (ushort)(FPVector3.Distance(a, b));
        }
    }
}
