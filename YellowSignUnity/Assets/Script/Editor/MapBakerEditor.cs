using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(TileMapBaker))]
public class MapBakerEditor : Editor
{
    public override void OnInspectorGUI() {
        TileMapBaker myTarget = (TileMapBaker)target;

        myTarget.asset = (TileMapDataAsset)EditorGUILayout.ObjectField(myTarget.asset, typeof(TileMapDataAsset), true);
        myTarget.Level = (GameObject)EditorGUILayout.ObjectField(myTarget.Level, typeof(GameObject), true);

        myTarget.Width = EditorGUILayout.IntField("Width", myTarget.Width);
        myTarget.Height = EditorGUILayout.IntField("Height", myTarget.Height);
        myTarget.HeightOffset = EditorGUILayout.FloatField("Bake Height Offset", myTarget.HeightOffset);
        myTarget.Tolerance = EditorGUILayout.FloatField("Tile Tolerance", myTarget.Tolerance);
        var tempMask = EditorGUILayout.MaskField("Layer", InternalEditorUtility.LayerMaskToConcatenatedLayersMask(myTarget.layerMask), InternalEditorUtility.layers);
        myTarget.layerMask = tempMask;

        if (GUILayout.Button("Bake")) {
            myTarget.Build();
        }
        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(myTarget.asset);
    }
}
