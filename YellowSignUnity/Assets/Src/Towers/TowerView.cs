using System;
using System.Collections;
using System.Collections.Generic;
using Quantum;
using UnityEngine;

public unsafe class TowerView : EntityView
{
    public  Transform              _GunBarrel;
    public  Vector3                _RotationOffset;
    
    private DispatcherSubscription _updateSubscription;

    private void Awake()
    {
        _updateSubscription = QuantumCallback.Subscribe<CallbackUpdateView>(this, OnObservedGameUpdated);
    }
    
    private void OnObservedGameUpdated(CallbackUpdateView e)
    {
        if (!EntityRef.IsValid)
            return;

        var tower = e.Game.Frames.Predicted.Unsafe.GetPointer<Tower>(EntityRef);
        _GunBarrel.rotation = tower->BarrelRotation.ToUnityQuaternion() * Quaternion.Euler(_RotationOffset);
    }

    private void OnDestroy()
    {
        // QuantumCallback.Unsubscribe(_updateSubscription, OnObservedGameUpdated);
    }
}
