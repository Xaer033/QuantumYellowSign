
    using Quantum;
    using UnityEditor.Rendering.LookDev;
    using UnityEngine;
    
[CreateAssetMenu(menuName = "YellowSign/PlayerController")]
public class PlayerController : ScriptableObject
{
    private PlayerDataProvider _playerDataProvider = new PlayerDataProvider();


    public void OnPlayerDataSet(Frame f, PlayerRef playerRef)
    {
        
    }   
    
    public void OnSpawnTowerModeToggled(bool spawnTower)
    {
        _playerDataProvider.TowerSpawnMode.Value = spawnTower;
        Debug.Log($"Spawn Tower Mode: {spawnTower}");
    }

    private void Awake()
    {
        _playerDataProvider.GoldAmount.Value = 150;
        _playerDataProvider.TowerSpawnMode.Value = true;
    }
    

    private void Update()
    {
        // EntityRef localPlayer = QuantumRunner.Default.Game.Frames.Verified.Context.
        // QuantumRunner.Default.Game.Frames.Verified.Has()
    }
}
