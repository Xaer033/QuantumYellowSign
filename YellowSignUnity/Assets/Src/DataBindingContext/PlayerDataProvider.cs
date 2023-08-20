using de.JochenHeckl.Unity.DataBinding;
using UnityEngine;
public class PlayerDataProvider : DataSourceBase<PlayerDataProvider>
{
    public string GoldAmount { get; set; }
    public bool TowerSpawnMode { get; set; }
}
