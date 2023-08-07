using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Quantum;
using Photon.Deterministic;

public class WallsPool : MonoBehaviour
{
    
    public Dictionary<FPVector3,GameObject> Walls = new Dictionary<FPVector3, GameObject>();
    public Stack<GameObject> Pool = new Stack<GameObject> ();
    [SerializeField] public GameObject WallsHexagon;

    void Start() {
        QuantumEvent.Subscribe(listener: this, handler: (EventEditTileMap e) => UpdateTiles(e));
    }

    void UpdateTiles(EventEditTileMap e)
    {

        FPVector3 pos = e.Position;
        if (e.TileType == TileType.Wall) {
            if (Pool.Count == 0) {
                if (Walls.ContainsKey(pos) && Walls[pos].activeSelf)
                    return;
                Walls[pos] = Instantiate(WallsHexagon);
                Walls[pos].transform.position = pos.ToUnityVector3();
            } else {
                Walls[pos] = Pool.Pop();
                Walls[pos].transform.position = pos.ToUnityVector3();
                Walls[pos].SetActive(true);
            }

        } else {
            if (Walls.ContainsKey(pos)) {
                var walls = Walls[pos];
                Walls[pos].SetActive(false);
                Walls.Remove(pos);
                Pool.Push(walls);
            }
            
        }
    }
}
