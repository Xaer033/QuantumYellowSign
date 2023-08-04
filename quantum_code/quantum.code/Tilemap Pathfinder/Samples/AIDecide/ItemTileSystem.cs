namespace Quantum.Sample
{
    public unsafe struct ItemTileFilter
    {
        public EntityRef Entity;
        public ItemTile* ItemTile;
        public Transform3D* Transform;
    }

    // This systems is agnostic to pathfinder but can be used to indentfy witch objects are in a specific
    // tile. Then it can be used to check collisions or find interesting points to chase. To register the position
    // in this system, just add ItemTile component to entity. The objects in the map can be acessed in global entities
    public unsafe class ItemTileSystem : SystemMainThreadFilter<ItemTileFilter>
    {
        TileMapData asset;

        public override void OnInit(Frame f) {
            f.Global->TileMapItems = f.AllocateDictionary<int, ItemsInTile>();
            asset = f.GetSingleton<RuntimeTileMap>().Asset(f);
        }

        public override void Update(Frame f, ref ItemTileFilter filter) {

            var map = f.GetSingleton<RuntimeTileMap>();
            var newPosition = filter.Transform->Position;
            var lastPosition = filter.ItemTile->LastPosition;

            // If the position of item changed, the system going to move it to the specific list 
            // There is a list of items for each tile and they can be search using the index of tile
            // But the lists only exists if there is something inside, otherwise they are removed

            if (newPosition != lastPosition) {
                
                var items = f.ResolveDictionary(f.Global->TileMapItems);
                var newIndex = asset.PositionToIndex(newPosition);
                var lastIndex = asset.PositionToIndex(lastPosition);

                // If the indexes already contains a list, the item is added
                // Otherwise, a new list is added and the item is added to it

                if (items.ContainsKey(newIndex)) {
                    f.ResolveList(items[newIndex].Items).Add(filter.Entity);
                } else {
                    var newList = new ItemsInTile();
                    newList.Items = f.AllocateList<EntityRef>();
                    items.Add(newIndex, newList);
                    f.ResolveList(items[newIndex].Items).Add(filter.Entity);
                }

                // Remove the item fom the old position, and if the position
                // hasn't any items it is removed from indexes

                if (items.ContainsKey(lastIndex)) {
                    var list = f.ResolveList(items[lastIndex].Items);
                    list.Remove(filter.Entity);
                    if(list.Count == 0) {
                        f.FreeList(items[lastIndex].Items);
                        items.Remove(lastIndex);
                    }
                }

                filter.ItemTile->LastPosition = f.Get<Transform3D>(filter.Entity).Position;

            }

        }
    }
}
