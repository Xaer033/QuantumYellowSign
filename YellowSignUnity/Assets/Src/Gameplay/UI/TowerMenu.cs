using Slash.Unity.DataBind.Core.Data;
using UnityEngine;
using UnityEngine.UI;

public class TowerMenu : MonoBehaviour
{
    public Context   _TowerDataProvider;
    
    public GameObject _Root;
    public Button    _ShowHideButton;
    public Toggle    _SpawnTowerModeToggle;
    


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
    }

    private void Update()
    {
        
    }
}
