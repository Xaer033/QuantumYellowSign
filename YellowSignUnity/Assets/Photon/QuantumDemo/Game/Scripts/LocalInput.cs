using System;
using Photon.Deterministic;
using Quantum;
using UnityEngine;

public class LocalInput : MonoBehaviour {

    TileMapData tilemap;
    short position;
    
    private void OnEnable() {
    
          QuantumCallback.Subscribe(this, (CallbackPollInput callback) => PollInput(callback));
    }

    public void PollInput(CallbackPollInput callback) {
       
        if (tilemap == null) {
            var verifiedFrame = QuantumRunner.Default.Game.Frames.Verified;
            var asset = verifiedFrame.GetSingleton<RuntimeTileMap>().MapAsset;
            tilemap = verifiedFrame.FindAsset<TileMapData>(asset.Id);
        }

          Quantum.Input i = new Quantum.Input();

          if (UnityEngine.Input.GetMouseButton(0)) {
              Ray ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
              if (Physics.Raycast(ray, out var hit)) {
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
