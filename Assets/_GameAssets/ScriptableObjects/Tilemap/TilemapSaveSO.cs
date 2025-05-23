using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TilemapSaveSO", menuName = "ScriptableObjects/TilemapSaveSO")]
public class TilemapSaveSO : ScriptableObject
{
    public List<TilePlacementData> tilePlacements = new();
}
