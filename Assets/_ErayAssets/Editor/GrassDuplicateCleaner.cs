using UnityEditor;
using UnityEngine;

public class GridSpawner : EditorWindow
{
    private GameObject grassPrefab;
    private int width = 83;
    private int height = 84;
    private float spacing = 1f;

    // Sabit başlangıç pozisyonu
    private Vector3 origin = new Vector3(-39.9745178f, 46.9405785f, -0.628662109f);

    [MenuItem("Tools/Grid Spawner")]
    public static void ShowWindow()
    {
        GetWindow<GridSpawner>("Grid Spawner");
    }

    private void OnGUI()
    {
        GUILayout.Label("Grid Ayarları", EditorStyles.boldLabel);

        grassPrefab = (GameObject)EditorGUILayout.ObjectField("Grass Prefab", grassPrefab, typeof(GameObject), false);
        width = EditorGUILayout.IntField("Genişlik (X):", width);
        height = EditorGUILayout.IntField("Yükseklik (Z):", height);
        spacing = EditorGUILayout.FloatField("Hücre Aralığı:", spacing);

        EditorGUILayout.LabelField("Başlangıç Pozisyonu:", origin.ToString());

        if (GUILayout.Button("Grid Oluştur"))
        {
            if (grassPrefab == null)
            {
                Debug.LogError("❌ Prefab atanmadı!");
                return;
            }

            SpawnGrid();
        }
    }

    private void SpawnGrid()
    {
        GameObject parent = new GameObject("GrassGrid");

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 pos = origin + new Vector3(x * spacing, 0, z * spacing);
                GameObject tile = (GameObject)PrefabUtility.InstantiatePrefab(grassPrefab);
                tile.transform.position = pos;
                tile.transform.SetParent(parent.transform);
            }
        }

        Debug.Log($"✅ Grid oluşturuldu: {width} x {height} ({width * height} obje)");
    }
}
