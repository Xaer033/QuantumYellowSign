using UnityEngine;
using UnityEngine.UI;
public class TowerMenu : MonoBehaviour
{
    // public Context   _TowerDataProvider;
    public PlayerController _PlayerController;
    public GameObject       _Root;
    // public View             _View;
    public Button           _ShowHideButton;
    public Toggle           _SpawnTowerModeToggle;
    


    public void OnSpawnModeChanged(bool isSpawnMode)
    {
        Debug.Log($"OnSpawnChanged: {isSpawnMode}");
        
    }

    public void OnToggleShowState()
    {
        _Root.SetActive(!_Root.activeSelf);
    }

    private void Awake()
    {
        _Root.SetActive(false);

        // _View.DataSource = _PlayerController.PlayerDataProvider;
    }

    private void Update()
    {
        
    }
}
