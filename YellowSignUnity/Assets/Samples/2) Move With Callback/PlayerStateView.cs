using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Quantum;

public class PlayerStateView : MonoBehaviour
{
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] Material[] materials;

    void Start()
    {
        QuantumEvent.Subscribe(this, (EventChangePlayerState e) => ChangePlayerState(e));
    }


    void ChangePlayerState(EventChangePlayerState playerStates)
    {
        meshRenderer.material = materials[(int)playerStates.State];
    }
}
