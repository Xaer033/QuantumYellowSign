
namespace Quantum
{
    public unsafe partial struct RuntimeTowerMap
    {
        public void Load(Frame f) 
        {
            Towers = f.AllocateDictionary<int, EntityRef>();
        }
        
        public EntityRef GetTower(Frame f, int index) 
        {
            var map = f.ResolveDictionary<int, EntityRef>(Towers);
            return map[index];
        }

        public bool TryGetTower(Frame f, int index, out EntityRef entityRef)
        {
            entityRef = GetTower(f, index);
            return entityRef != EntityRef.None;
        }

        public void SetTower(Frame f, int index, EntityRef entityRef) 
        {
            var map = f.ResolveDictionary(Towers);
            map[index] = entityRef;
        }
    }
}
