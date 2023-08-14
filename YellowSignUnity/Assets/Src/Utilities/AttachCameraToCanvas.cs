using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class AttachCameraToCanvas : MonoBehaviour
{
    public  float  _PlaneDistance;
    
    private Canvas _canvas;
    
    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        CheckCanvasForCamera();

        _canvas.planeDistance = _PlaneDistance;
    }

    // Update is called once per frame
    void Update()
    {
        CheckCanvasForCamera();
    }

    private void CheckCanvasForCamera()
    {
        if (_canvas.worldCamera == null)
            _canvas.worldCamera = Camera.main;
    }
}
