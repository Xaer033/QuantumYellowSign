using UnityEngine;
using Quantum;
using Photon.Deterministic;
using Quantum.YellowSign;
using UnityEngine.UI;

public class ClickToEdit : MonoBehaviour
{
    public EntityPrototypeAsset _TowerViewAsset;
    

    [SerializeField] Text editionText;
    FPVector3 position;
    bool isRemoving;
    const string removeWalls = "<color=blue>remove walls</color>";
    const string addWalls = "<color=blue>add walls</color>";
    const string baseText = "Press right mouse button to ";

    bool invert;

    public void ChangeEdition() {
    }

    void Update()
    {
        isRemoving = UnityEngine.Input.GetKey(KeyCode.Space);
        editionText.text = baseText + (isRemoving?removeWalls: addWalls);

        if (UnityEngine.Input.GetMouseButton(1)) {
            Ray ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);

            if (Physics.Raycast(ray, out var hit)) {

                var f = QuantumRunner.Default.Game.Frames.Predicted;

                var runtimeMap = f.GetSingleton<RuntimeTileMap>();
                var map = runtimeMap.Asset(f);

                int index = map.PositionToIndex(hit.point.ToFPVector3());

                if (index != map.PositionToIndex(position)) {
                    position = hit.point.ToFPVector3();

                    DeterministicCommand command;
                    
                    if (!isRemoving)
                    {
                        command = new CommandSpawnTower()
                        {
                            PrototypeGUID = _TowerViewAsset.AssetObject.Guid.Value,
                            GridPositionIndex = index,
                            PlayerOwner = 1,
                        };
                    }
                    else
                    {
                        command = new CommandDestroyTower()
                        {
                            GridPositionIndex = index,
                        };
                    }
                    
                    QuantumRunner.Default.Game.SendCommand(command);
                    return;
                }
            }
        }
    }
}
