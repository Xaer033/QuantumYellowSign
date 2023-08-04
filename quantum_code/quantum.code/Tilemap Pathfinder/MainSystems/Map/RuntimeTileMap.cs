namespace Quantum
{
    public unsafe partial struct RuntimeTileMap
    {
        public TileMapData Asset(Frame f) {
             return f.FindAsset<TileMapData>(MapAsset.Id);
        }

        // Called in the start of simulation, the array of tiles are converted in a bitset structure
        // The asset is readonly, but the bitset can be changed to add or remove walls using a tiny 
        // data structure

        public void Load(Frame f) {

            Tiles = f.AllocateList<byte>();
            var map = Asset(f);
            var list = f.ResolveList(Tiles);
            int size = (map.Tiles.Length / 8) + 1;

            while (list.Count < size) {
                list.Add(0);
            }
            
            for (int i = 0; i < map.Tiles.Length; i++) {
                var offset = i / 8;
                if (map.Tiles[i].TileType == TileType.Wall) {
                    list[offset] |= (byte)(1 << (i % 8));
                }
            }
        }

        public bool IsWall(Frame f, int index) {

            var list = f.ResolveList(Tiles);
            var offset = index / 8;

            if (index < 0 || offset > list.Count) 
                return false;

            return (list[offset] & (byte)(1 << index % 8)) > 0;
        }

        public void SetTile(Frame f, int index, TileType type) {

            var list = f.ResolveList(Tiles);
            var offset = index / 8;

            if (index < 0 || offset > list.Count)
                return;

            if (type == TileType.Wall) {
                list[offset] |= (byte)(1 << index % 8);
            } else {
                list[offset] &= (byte)(~(1 << index % 8));
            }

        }
    }
}
