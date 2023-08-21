using Quantum;
using UnityEngine;

[CreateAssetMenu(menuName = "YellowSign/PlayerController")]
public class PlayerController : ScriptableObject
{
    
    public PlayerDataProvider PlayerDataProvider { get; private set; } = new();
    
    private SimulationConfig   _simulationConfig;
    
    public void OnPlayerDataSet(Frame f, PlayerRef playerRef)
    {
        
    }   
    
    public void OnSpawnTowerModeToggled(bool spawnTower)
    {
        // PlayerDataProvider.TowerSpawnMode = spawnTower;
        Debug.Log($"Spawn Tower Mode: {spawnTower}");
    }

    private void OnEnable()
    {
        
        
        //
        // _simulationConfig             = FindObjectOfType<SimulationConfig>();
        // _SimulationConfig.ThreadCount = SystemInfo.processorCount;
    }
}
