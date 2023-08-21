using System;
using System.Collections;
using System.Collections.Generic;
using Quantum;
using UnityEngine;

public class YellowSignEntityViewUpdater : EntityViewUpdater
{
    public List<AssetPoolPair> _ViewPools;
    
    private Dictionary<long, EntityView[]> _poolMap = new Dictionary<long, EntityView[]>();
    
    private void Start()
    {
        foreach (var pair in _ViewPools)
        {
            long key = pair.ViewAsset.AssetObject.Guid.Value;
            _poolMap[key] = new EntityView[pair.PoolCount];
            for (int i = 0; i < pair.PoolCount; ++i)
            {
                var view = GameObject.Instantiate(pair.ViewAsset.View, ViewParentTransform);
                view.gameObject.SetActive(false);

                _poolMap[key][i] = view;
            }
        }
    }
    
    protected override EntityView CreateEntityViewInstance(EntityViewAsset asset, Vector3? position = null, Quaternion? rotation = null) 
    {
        Debug.Assert(asset.View != null);

        EntityView view = null;
        if (_poolMap.TryGetValue(asset.AssetObject.Guid.Value, out var entityPool))
        {
            for (int i = 0; i < entityPool.Length; ++i)
            {
                if (entityPool[i].gameObject.activeSelf) 
                    continue;
                    
                view = entityPool[i];
                
                if (position.HasValue)
                    view.transform.position = position.Value;

                if (rotation.HasValue)
                    view.transform.rotation = rotation.Value;

                view.gameObject.SetActive(true);
                break;
            }
        }

        return view;
    }

    protected override void DestroyEntityViewInstance(EntityView instance)
    {
        instance.gameObject.SetActive(false);
    }

    [Serializable]
    public class AssetPoolPair
    {
        public EntityViewAsset ViewAsset;
        public int             PoolCount;
    }
}
