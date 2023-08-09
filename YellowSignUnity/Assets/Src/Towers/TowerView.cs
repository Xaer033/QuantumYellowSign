using System;
using System.Collections;
using System.Collections.Generic;
using Quantum;
using UnityEngine;

public unsafe class TowerView : MonoBehaviour
{
    public  EntityView             _EntityView;
    public  Transform              _GunBarrel;
    public  Vector3                _RotationOffset;
    
    private DispatcherSubscription _updateSubscription;
    
    void Awake()
    {
        _updateSubscription = QuantumCallback.Subscribe<CallbackUpdateView>(this, OnObservedGameUpdated);

    }
    
    private void OnObservedGameUpdated(CallbackUpdateView e)
    {
        if (_EntityView == null || !_EntityView.EntityRef.IsValid)
            return;
            
        var tower = e.Game.Frames.Predicted.Unsafe.GetPointer<Tower>(_EntityView.EntityRef);
        _GunBarrel.rotation = tower->BarrelRotation.ToUnityQuaternion() * Quaternion.Euler(_RotationOffset);
    }

    private void OnDestroy()
    {
        // QuantumCallback.Unsubscribe(_updateSubscription, OnObservedGameUpdated);
    }
}
