using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridObjectSO))]
public class GridObjectSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Özellikleri al
        SerializedProperty prefab = serializedObject.FindProperty("_prefab");
        SerializedProperty manualScale = serializedObject.FindProperty("_manualScale");
        SerializedProperty manualRotation = serializedObject.FindProperty("_manualRotation");
        SerializedProperty manualPositionOffset = serializedObject.FindProperty("_manualPositionOffset");

        SerializedProperty category = serializedObject.FindProperty("_category");
        SerializedProperty fireStatus = serializedObject.FindProperty("_fireStatus");
        SerializedProperty flammableStatus = serializedObject.FindProperty("_flammableStatus");
        SerializedProperty movementStatus = serializedObject.FindProperty("_movementStatus");
        SerializedProperty specialStatus = serializedObject.FindProperty("_specialStatus");

        EditorGUILayout.PropertyField(prefab);

        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("Transform Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(manualScale, new GUIContent("Manual Scale"));
        EditorGUILayout.PropertyField(manualRotation, new GUIContent("Manual Rotation"));
        EditorGUILayout.PropertyField(manualPositionOffset, new GUIContent("Manual Position Offset"));

        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("Tile Properties", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(category);
        EditorGUILayout.PropertyField(fireStatus);

        if ((TileFireStatus)fireStatus.enumValueIndex == TileFireStatus.Flammable)
        {
            EditorGUILayout.PropertyField(flammableStatus);
        }

        EditorGUILayout.PropertyField(movementStatus);
        EditorGUILayout.PropertyField(specialStatus);

        serializedObject.ApplyModifiedProperties();
    }
}
