using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TilemapObjectsSO))]
public class TilemapObjectsSOEditor : Editor
{
    private SerializedProperty prefabList;

    private void OnEnable()
    {
        prefabList = serializedObject.FindProperty("prefabList");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(prefabList, new GUIContent("Prefab List"), true);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Prefab Aktiflik Durumları", EditorStyles.boldLabel);

        for (int i = 0; i < prefabList.arraySize; i++)
        {
            SerializedProperty element = prefabList.GetArrayElementAtIndex(i);
            GridObjectSO obj = element.objectReferenceValue as GridObjectSO;

            if (obj != null)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.ObjectField(obj.name, obj, typeof(GridObjectSO), false);

                bool newActive = EditorGUILayout.Toggle("Aktif", obj.IsActive);
                if (newActive != obj.IsActive)
                {
                    Undo.RecordObject(obj, "Toggle Active");

                    // IsActive sadece getter olduğu için, backing field üzerinden güncelle
                    var so = new SerializedObject(obj);
                    so.FindProperty("_isActive").boolValue = newActive;
                    so.ApplyModifiedProperties();

                    EditorUtility.SetDirty(obj);
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
