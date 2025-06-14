using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

[CreateAssetMenu(fileName = "TilemapObjectsSO", menuName = "ScriptableObjects/TilemapObjectsSO")]
public class TilemapObjectsSO : ScriptableObject
{
    public List<GridObjectSO> prefabList = new();

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Null olmayanları al ve isme göre sırala
        prefabList = prefabList
            .Where(p => p != null)
            .OrderBy(p => p.name)
            .ToList();

        EditorUtility.SetDirty(this); // asset'i değişti olarak işaretle
    }
#endif
}