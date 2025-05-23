#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

using UnityEngine;

public static class TilemapPlacementHandler
{
    private const string RootName = "LEVEL_OBJECTS";

    public static void PlacePrefab(GridObjectSO data, Vector3 position, Quaternion rotation)
    {
        GameObject root = GameObject.Find(RootName);
        if (root == null)
            root = new GameObject(RootName);

        Vector3 finalPosition = position + data.ManualPositionOffset;
        Quaternion finalRotation = Quaternion.Euler(data.ManualRotation) * rotation;

        // Pozisyonda zaten obje varsa ekleme
        // float threshold = 0.1f;
        // foreach (Transform child in root.transform)
        // {
        //     if (Vector3.Distance(child.position, finalPosition) < threshold)
        //     {
        //         Debug.LogWarning($"Bu pozisyonda zaten bir obje var: {finalPosition}");
        //         return;
        //     }
        // }

        GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(data.Prefab);
        obj.transform.position = finalPosition;
        obj.transform.rotation = finalRotation;
        obj.transform.localScale = data.ManualScale;
        obj.transform.SetParent(root.transform);

        foreach (Transform child in obj.GetComponentsInChildren<Transform>())
            child.gameObject.layer = obj.layer;

        GridTileRuntime tile = obj.AddComponent<GridTileRuntime>();
        tile.Initialize(data.FireStatus, data.FlammableStatus, data.MovementStatus, data.SpecialStatus);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }

    public static void RemovePrefab(Vector3 position, float threshold = 0.1f)
    {
        GameObject root = GameObject.Find(RootName);
        if (root == null) return;

        foreach (Transform child in root.transform)
        {
            Vector3 childPos = child.position;

            bool sameXZ = Mathf.Abs(childPos.x - position.x) < threshold &&
                          Mathf.Abs(childPos.z - position.z) < threshold;

            // Yükseklik kontrolü: İstediğin aralığa göre ayarlayabilirsin
            bool yCondition = childPos.y >= 1f; // örn: zemin üzerindekiler

            var tileRuntime = child.GetComponent<GridTileRuntime>();

            if (sameXZ && yCondition)
            {
                Object.DestroyImmediate(child.gameObject);
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                break;
            }
        }
    }

}
#endif
