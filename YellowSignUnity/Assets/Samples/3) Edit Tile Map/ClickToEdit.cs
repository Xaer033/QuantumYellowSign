using UnityEngine;
using Quantum;
using Photon.Deterministic;
using Quantum.YellowSign;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using LayerMask = UnityEngine.LayerMask;

public class ClickToEdit : MonoBehaviour
{
    public EntityPrototypeAsset _TowerViewAsset;

    public LayerMask _BoardLayerMask;
    
    FPVector3        position;
    bool             isRemoving;

    bool invert;

    void Update()
    {
        isRemoving = Keyboard.current.spaceKey.isPressed;
        
        if (Mouse.current.rightButton.isPressed)
        {
            Vector3 mousePosition = Mouse.current.position.ReadValue();
            mousePosition.z = Camera.main.farClipPlane;
            
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);

            if (Physics.Raycast(ray, out var hit, 1000.0f))
            {
                int bitMask = (_BoardLayerMask.value & hit.transform.gameObject.layer);
                if(bitMask != 0)
                    return;
                
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
