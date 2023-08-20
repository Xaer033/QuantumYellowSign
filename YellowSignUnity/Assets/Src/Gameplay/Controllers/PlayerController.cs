using Quantum;
using UnityEngine;

[CreateAssetMenu(menuName = "YellowSign/PlayerController")]
public class PlayerController : ScriptableObject
{
    public  PlayerDataProvider PlayerDataProvider { get; private set; }
    
    private SimulationConfig   _simulationConfig;
    
    public void OnPlayerDataSet(Frame f, PlayerRef playerRef)
    {
        
    }   
    
    public void OnSpawnTowerModeToggled(bool spawnTower)
    {
        PlayerDataProvider.TowerSpawnMode = spawnTower;
        Debug.Log($"Spawn Tower Mode: {spawnTower}");
    }

    private void OnEnable()
    {
        // PlayerDataProvider.NotifyChanges(x =>
        // {
        //     PlayerDataProvider.GoldAmount = $"{150}";
        //     PlayerDataProvider.TowerSpawnMode = true;
        // });
        
        
        //
        // _simulationConfig             = FindObjectOfType<SimulationConfig>();
        // _SimulationConfig.ThreadCount = SystemInfo.processorCount;
    }
}
