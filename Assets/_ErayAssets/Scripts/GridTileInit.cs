// using UnityEngine;

// public class GridTileInit : MonoBehaviour
// {
//     [ContextMenu("Güncelle: FlammableStatus + Layer")]
//     public void UpdateFlammableTiles()
//     {
//         GridTileRuntime[] allTiles = FindObjectsOfType<GridTileRuntime>();
//         int updatedCount = 0;

//         int flammableLayer = LayerMask.NameToLayer("FlammableGrid");
//         if (flammableLayer == -1)
//         {
//             Debug.LogError("Layer 'FlammableGrid' bulunamadı! Layer'ı önce Unity'de tanımlamalısın.");
//             return;
//         }

//         foreach (var tile in allTiles)
//         {
//             if (tile.FireStatus == TileFireStatus.Flammable)
//             {
//                 tile._flammableStatus = FlammableStatus.NormalIgnition;
//                 tile.gameObject.layer = flammableLayer;
//                 updatedCount++;
//             }
//         }

//         Debug.Log($"[GridTileInitializer] Güncellenen tile sayısı: {updatedCount}");
//     }
// }
