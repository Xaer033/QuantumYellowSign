using UnityEngine;

public class DebugDrawer : MonoBehaviour
{
    public ShapeType _ShapeType;
    public float     _Scale = 1.0f;
    public Color     _Color = Color.white;
    
    private void OnDrawGizmos()
    {
        Gizmos.color = _Color;
        
        switch (_ShapeType)
        {
            case ShapeType.Box:
                Gizmos.DrawWireCube(transform.position, transform.localScale * _Scale);
                break;
            case ShapeType.Sphere:
                Gizmos.DrawWireSphere(transform.position, transform.localScale.x * _Scale);
                break;
        }

        Gizmos.color = Color.white;
    }
    
    public enum ShapeType
    {
        None,
        Box,
        Sphere,
    }
}
