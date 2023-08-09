using System;
using Photon.Deterministic;

namespace Quantum
{

    [Serializable]
    public struct Tile
    {
        public int Index;
        public TileType TileType;
    }

    // This is the base asset structure. It has a one-dimentional array to represent the map.
    // All the functions works only with indexs in this array, then you need to convert a 2d space
    // or hexagonal space to index position array.
    // The neighborns and the costs can be overrided to add more behaviour if necessary
    // This example is based on a simple grid map with cross movement base.

    public abstract partial class TileMapData
    {
        public Tile[] Tiles;
        public Byte HEIGHT;
        public Byte WIDTH;
        public FP TileWidth;
        public FP TileHeight;
        public FP OffsetX;
        public FP OffsetY;
        public NeighborList[] Neighbors;

        [Serializable]
        public struct NeighborList {
            public int[] Values;
        }

        // Properties
        public FPVector3 Offset => new FPVector3(OffsetX, 0, OffsetY);
        public int Size => Tiles.Length;
        public int GetWidth => WIDTH;
        public int GetHeight => HEIGHT;

        
        // Used by baker tool to know the direcion of movement of this map. 
        public virtual int[] GetNeighborsOffset(int index) {
            return new int[4] { WIDTH, -WIDTH, 1, -1 };
        }

        public virtual int[] GetNeighbors(int index) {
            return Neighbors[index].Values;
        }

        // The defaut cost of movement is 1. This can be changed with some metadata to add differents
        // types of terrain with different terrains
        public int GetCost(int index) {
            return 1;
        }

        // The heuristic can be changed to 
        public virtual ushort Heuristic(int from, int to) {
            FPVector3 a = IndexToPositionRaw(from);
            FPVector3 b = IndexToPositionRaw(to);
            // return (ushort)(FPMath.Abs(a.X - b.X) + FPMath.Abs(a.Z - b.Z));
            
            return (ushort)(FPVector3.Distance(a, b));
        }

        // Below is the conversion algorithms of basic grid to index position

        public virtual FPVector3 IndexToPositionRaw(int index) {
            int x = index % WIDTH;
            int y = index / WIDTH;
            return new FPVector3(x, 0, y);
        }

        public virtual FPVector3 IndexToPosition(int index) {
            int x = index % WIDTH;
            int y = index / WIDTH;
            return new FPVector3(x * TileWidth, 0, y * TileHeight) + Offset;
        }

        public virtual int PositionToIndex(FPVector3 position) {
            FPVector3 correctPosition = position - Offset;
            correctPosition.X /= TileWidth;
            correctPosition.Z /= TileHeight;
            correctPosition.X += TileWidth / 2;
            correctPosition.Z += TileHeight / 2;
            return (FPMath.FloorToInt(correctPosition.Z) * WIDTH) + FPMath.FloorToInt(correctPosition.X);
        }

    }

   
}
