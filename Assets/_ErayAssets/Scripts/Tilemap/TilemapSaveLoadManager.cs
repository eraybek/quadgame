#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

public static class TilemapSaveLoadManager
{
    public static void Save(TilemapSaveSO saveDataAsset)
    {
        saveDataAsset.tilePlacements.Clear();

        for (int level = 1; level <= 10; level++)
        {
            GameObject levelGO = GameObject.Find("Level" + level);
            if (levelGO == null) continue;

            foreach (Transform child in levelGO.transform)
            {
                saveDataAsset.tilePlacements.Add(new TilePlacementData
                {
                    prefab = PrefabUtility.GetCorrespondingObjectFromSource(child.gameObject),
                    position = child.position,
                    rotation = child.rotation,
                    scale = child.localScale,
                    level = level
                });
            }
        }

        EditorUtility.SetDirty(saveDataAsset);
        Debug.Log("Tilemap kaydedildi!");
    }

    public static void Load(TilemapSaveSO saveDataAsset)
    {
        for (int i = 1; i <= 10; i++)
        {
            GameObject level = GameObject.Find("Level" + i);
            if (level != null)
                Object.DestroyImmediate(level);
        }

        foreach (var tile in saveDataAsset.tilePlacements)
        {
            string levelName = "Level" + tile.level;
            GameObject levelParent = GameObject.Find(levelName);
            if (levelParent == null)
                levelParent = new GameObject(levelName);

            GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(tile.prefab);
            obj.transform.position = tile.position;
            obj.transform.rotation = tile.rotation;
            obj.transform.localScale = tile.scale;
            obj.transform.SetParent(levelParent.transform);
        }

        Debug.Log("Tilemap yüklendi!");
    }
}
#endif