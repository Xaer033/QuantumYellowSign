using System;
using Photon.Deterministic;
using Quantum;
using UnityEngine;
using UnityEngine.InputSystem;
using LayerMask = UnityEngine.LayerMask;

public class LocalInput : MonoBehaviour
{

    public LayerMask _PlaceableLayerMask;
    TileMapData      tilemap;
    short            position;
    
    private void OnEnable() {
    
          QuantumCallback.Subscribe(this, (CallbackPollInput callback) => PollInput(callback));
    }

    public void PollInput(CallbackPollInput callback) 
    {
       
        if (tilemap == null) 
        {
            var verifiedFrame = QuantumRunner.Default.Game.Frames.Verified;
            var asset = verifiedFrame.GetSingleton<RuntimeTileMap>().MapAsset;
            tilemap = verifiedFrame.FindAsset<TileMapData>(asset.Id);
        }

        Quantum.Input i = new Quantum.Input();

        if (Mouse.current.leftButton.isPressed) 
        {
            Vector3 mousePosition = Mouse.current.position.ReadValue();
            mousePosition.z = Camera.main.farClipPlane;
            
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out var hit))
            {
                int bitMask = (_PlaceableLayerMask.value & hit.transform.gameObject.layer);
                if(bitMask != 0)
                    return;
                
                    
                position = (short)tilemap.PositionToIndex(hit.point.ToFPVector3());
                i.position = position;
                i.click = true;
                callback.SetInput(i, DeterministicInputFlags.Repeatable);
                return;
            }
        }
        
        i.click = false;
        callback.SetInput(i, DeterministicInputFlags.Repeatable);
    }
}
