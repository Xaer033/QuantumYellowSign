using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Quantum;
using System;
using Photon.Deterministic;
using static Quantum.TileMapData;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
#endif

public unsafe class TileMapBaker: MonoBehaviour
{
    public int Width;
    public int Height;
    public Rect Box;
    public TileMapDataAsset asset;
    
    public GameObject Level;
    public float HeightOffset;
    public UnityEngine.LayerMask layerMask;
    public float Tolerance;
    public float stepX;
    public float stepY;
    public float offsetX;
    public float offsetY;
    public Mesh hexagon;

    [SerializeField] Mesh mesh;

    private void OnDrawGizmosSelected() {

        Vector3 cubeSize = Vector3.right * stepX + Vector3.forward * stepY + Vector3.up;
        var asset = Desserialize();
        foreach (var tile in asset.Tiles) {
            if (tile.TileType == TileType.Wall) {
                Gizmos.color = Color.red;
            } else {
                Gizmos.color = Color.green; 
            }
            
            FPVector3 rawPosition = asset.IndexToPosition(tile.Index);
            Vector3   position    = rawPosition.ToUnityVector3();
            Gizmos.DrawWireCube(position, cubeSize);
        }
    }

    public void Build() {

        BakeMap(Desserialize());
    }

    TileMapData Desserialize() {
        switch (asset) {
            case TileMapAsset tileMapAsset: {
                    return tileMapAsset.Settings;
                }
            case TileMap8Asset tileMapAsset: {
                return tileMapAsset.Settings;
                }
            case HexagonalMapAsset tileMapAsset: {
                return tileMapAsset.Settings;
                }
        }
        return null;
    }

    void BakeMap(TileMapData tileMapAsset) {

        tileMapAsset.Tiles = new Tile[Width * Height];

        Bounds bounds = GetBounds(Level);

        stepX = bounds.size.x / Width;
        stepY = bounds.size.z / Height;

        offsetX = -bounds.extents.x + (stepX / 2);
        offsetY = -bounds.extents.z + (stepY / 2);

        tileMapAsset.WIDTH = (byte)Width;
        tileMapAsset.HEIGHT = (byte)Height;

        tileMapAsset.TileWidth = stepX.ToFP();
        tileMapAsset.TileHeight = stepY.ToFP();

        tileMapAsset.OffsetX = offsetX.ToFP();
        tileMapAsset.OffsetY = offsetY.ToFP();

            for (int x = 0; x < Width; x++) {
        for (int y = 0; y < Height; y++) {

                Vector3 position = new Vector3(x, 0.5f, y);
                Vector3 colliderPosition = new Vector3(x * stepX + offsetX, Level.transform.position.y + HeightOffset, y * stepY + offsetY);

                Collider[] hitColliders = Physics.OverlapBox(colliderPosition, (transform.localScale / 2) * (stepX- Tolerance), Quaternion.identity,layerMask);

                int index = (int)position.z * Width + (int)position.x;

                tileMapAsset.Tiles[index].Index = index;

                if (hitColliders.Length > 0) {
                    int i = 0;
                    while (i < hitColliders.Length) {;
                        i++;
                        tileMapAsset.Tiles[index].TileType = TileType.Wall;

                    }
                } else {
                    tileMapAsset.Tiles[index].TileType = TileType.None;
                }
            }
        }
        SetNeighbors(tileMapAsset);
    }

    public void SetNeighbors(TileMapData tileMapAsset) {
        tileMapAsset.Neighbors = new NeighborList[tileMapAsset.Tiles.Length];    
        foreach (var tile in tileMapAsset.Tiles) {
            int count = 0;
            int[] NeighborsOffset = tileMapAsset.GetNeighborsOffset(tile.Index);
            int size = NeighborsOffset.Length;
            List<int> values = new List<int>();
            foreach (var offset in NeighborsOffset) {
                if (HasTileInDirection(tileMapAsset,tile.Index,offset, out var v)) {
                    values.Add(tile.Index + offset);
                } else {
                    values.Add(-1);
                }
                count++;
            }
            tileMapAsset.Neighbors[tile.Index] = new NeighborList() {
                Values = values.ToArray()
            };

        }
    }

    public virtual bool HasTileInDirection(TileMapData tileMapAsset, int index, int direction, out int tile) {
        int currentColumn = index / tileMapAsset.WIDTH;
        int currentLine = index % tileMapAsset.WIDTH;
        tile = index + direction;
        int collumn = tile / tileMapAsset.WIDTH;
        int line = tile % tileMapAsset.WIDTH;
        if (Math.Abs(index - tile) == 1 && collumn != currentColumn 
            ||(Math.Abs(index - tile) != 1 && collumn == currentColumn)
            ||(collumn!=currentColumn &&  Math.Abs(currentLine-line) >= 2)    )
            return false;
        return (tile >= 0 && tile < tileMapAsset.Tiles.Length);
    }

    public static Bounds GetBounds(GameObject obj) {
        Bounds bounds = new Bounds();
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length > 0) {
            foreach (Renderer renderer in renderers) {
                if (renderer.enabled) {
                    bounds.Encapsulate(renderer.bounds);
                }
            }
        }
        return bounds;

    }

}
