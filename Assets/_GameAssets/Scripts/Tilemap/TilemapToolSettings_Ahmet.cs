using UnityEngine;
using System.Collections.Generic;

public enum TileScaleMode { Uniform, FlatY, None }

[System.Serializable]
public class TileEntry
{
    public GameObject prefab;
    public TileScaleMode scaleMode = TileScaleMode.Uniform;
}

// [CreateAssetMenu(fileName = "TilemapToolSettings", menuName = "Tilemap/Settings")]
public class TilemapToolSettings_Ahmet : ScriptableObject
{
    public List<TileEntry> prefabEntries = new List<TileEntry>();
}