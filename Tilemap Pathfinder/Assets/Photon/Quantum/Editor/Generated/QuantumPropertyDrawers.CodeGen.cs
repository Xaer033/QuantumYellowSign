// <auto-generated>
// This code was auto-generated by a tool, every time
// the tool executes this code will be reset.
// </auto-generated>

namespace Quantum.Editor {
  using Quantum;
  using UnityEngine;
  using UnityEditor;

  [CustomPropertyDrawer(typeof(AssetRefHexagonalMap))]
  public class AssetRefHexagonalMapPropertyDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
      AssetRefDrawer.DrawAssetRefSelector(position, property, label, typeof(HexagonalMapAsset));
    }
  }

  [CustomPropertyDrawer(typeof(AssetRefTileAgentConfig))]
  public class AssetRefTileAgentConfigPropertyDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
      AssetRefDrawer.DrawAssetRefSelector(position, property, label, typeof(TileAgentConfigAsset));
    }
  }

  [CustomPropertyDrawer(typeof(AssetRefTileMap))]
  public class AssetRefTileMapPropertyDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
      AssetRefDrawer.DrawAssetRefSelector(position, property, label, typeof(TileMapAsset));
    }
  }

  [CustomPropertyDrawer(typeof(AssetRefTileMap8))]
  public class AssetRefTileMap8PropertyDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
      AssetRefDrawer.DrawAssetRefSelector(position, property, label, typeof(TileMap8Asset));
    }
  }

  [CustomPropertyDrawer(typeof(AssetRefTileMapData))]
  public class AssetRefTileMapDataPropertyDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
      AssetRefDrawer.DrawAssetRefSelector(position, property, label, typeof(TileMapDataAsset));
    }
  }

  [CustomPropertyDrawer(typeof(Quantum.Prototypes.PlayerStates_Prototype))]
  [CustomPropertyDrawer(typeof(Quantum.Prototypes.TileType_Prototype))]
  [CustomPropertyDrawer(typeof(Quantum.Prototypes.WaypointStatus_Prototype))]
  [CustomPropertyDrawer(typeof(Quantum.Prototypes.InputButtons_Prototype))]
  partial class PrototypeDrawer {}
}
