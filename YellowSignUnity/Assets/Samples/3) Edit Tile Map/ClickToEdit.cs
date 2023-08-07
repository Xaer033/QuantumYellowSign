using UnityEngine;
using Quantum;
using Photon.Deterministic;
using Quantum.Sample;
using UnityEngine.UI;

public class ClickToEdit : MonoBehaviour
{

    [SerializeField] Text editionText;
    FPVector3 position;
    bool isRemoving;
    const string removeWalls = "<color=blue>remove walls</color>";
    const string addWalls = "<color=blue>add walls</color>";
    const string baseText = "Press right mouse button to ";

    bool invert;

    public void ChangeEdition() {
        /*isRemoving = !isRemoving;
        if (isRemoving) {
            editionText.text = baseText + removeWalls;
        } else {
            editionText.text = baseText + addWalls;
        }*/
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

                    CommandEditTile command = new CommandEditTile() {
                        Position = position,
                        TileType = isRemoving ? 0 : 1
                    };
                    QuantumRunner.Default.Game.SendCommand(command);
                    return;
                }
            }
        }
    }
}
