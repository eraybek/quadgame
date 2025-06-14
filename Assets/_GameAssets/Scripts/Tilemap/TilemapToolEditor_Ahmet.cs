// using UnityEditor;
// using UnityEngine;
// using UnityEditor.SceneManagement;
// using System.Collections.Generic;

// public class TilemapTool_Ahmet : EditorWindow
// {
//     [SerializeField] private TilemapToolSettings_Ahmet manualSettings;

//     private GameObject selectedPrefab;
//     private bool isPlacing = false;

//     private float cellSize = 3f;
//     private Vector3Int hoveredCell;
//     private Vector3Int lastPlacedCell = new Vector3Int(int.MinValue, int.MinValue, int.MinValue);

//     private static TilemapTool_Ahmet _activeWindow;
//     private TileScaleMode selectedScaleMode = TileScaleMode.Uniform;

//     private int selectedLevel = 1;
//     private bool placeModeEnabled = true;

//     private GameObject previewObject;
//     private Quaternion currentRotation = Quaternion.identity;

//     [MenuItem("Tools/Tilemap Tool")]
//     public static void ShowWindow()
//     {
//         var window = GetWindow<TilemapTool_Ahmet>("Tilemap Tool");
//         _activeWindow = window;
//     }

//     private void OnEnable()
//     {
//         SceneView.duringSceneGui += OnSceneGUI;
//         _activeWindow = this;
//     }

//     private void OnDisable()
//     {
//         SceneView.duringSceneGui -= OnSceneGUI;
//         _activeWindow = null;

//         if (previewObject != null)
//         {
//             DestroyImmediate(previewObject);
//             previewObject = null;
//         }
//     }

//     private void OnGUI()
//     {
//         GUILayout.Label("Tilemap Objeleri", EditorStyles.boldLabel);

//         placeModeEnabled = EditorGUILayout.ToggleLeft("🧱 Yerleştirme Modu Aktif", placeModeEnabled);

//         if (placeModeEnabled)
//         {
//             EditorGUILayout.HelpBox("Bu mod aktifken sahnede tıklama ile objeler yerleştirilir veya silinir. R tuşuyla döndürülür.", MessageType.Warning);
//         }

//         selectedLevel = EditorGUILayout.IntSlider("Seviye", selectedLevel, 1, 10);

//         manualSettings = (TilemapToolSettings_Ahmet)EditorGUILayout.ObjectField("Ayar Dosyası", manualSettings, typeof(TilemapToolSettings_Ahmet), false);

//         if (manualSettings == null)
//         {
//             EditorGUILayout.HelpBox("TilemapToolSettings asset atanmamış!", MessageType.Error);
//             return;
//         }

//         if (GUILayout.Button("🧹 Null Prefab'ları Temizle"))
//         {
//             manualSettings.prefabEntries.RemoveAll(p => p == null);
//             EditorUtility.SetDirty(manualSettings);
//         }

//         if (manualSettings.prefabEntries == null || manualSettings.prefabEntries.Count == 0)
//         {
//             EditorGUILayout.HelpBox("Prefab listesi boş. Prefab eklemek için sürükleyin.", MessageType.Info);
//         }

//         foreach (var entry in manualSettings.prefabEntries)
//         {
//             if (entry == null || entry.prefab == null) continue;

//             EditorGUILayout.BeginHorizontal();
//             if (GUILayout.Button(entry.prefab.name))
//             {
//                 selectedPrefab = entry.prefab;
//                 selectedScaleMode = entry.scaleMode;
//                 isPlacing = true;
//                 lastPlacedCell = new Vector3Int(int.MinValue, int.MinValue, int.MinValue);
//                 currentRotation = Quaternion.identity;
//                 ResetPreviewObject();
//             }

//             entry.scaleMode = (TileScaleMode)EditorGUILayout.EnumPopup(entry.scaleMode);
//             EditorGUILayout.EndHorizontal();
//         }


//         GUILayout.Space(10);
//         GUILayout.Label("Prefabları buraya sürükle:");
//         var newPrefab = (GameObject)EditorGUILayout.ObjectField(null, typeof(GameObject), false);
//         if (newPrefab != null && !manualSettings.prefabEntries.Exists(e => e.prefab == newPrefab))
//         {
//             manualSettings.prefabEntries.Add(new TileEntry
//             {
//                 prefab = newPrefab,
//                 scaleMode = TileScaleMode.Uniform // varsayılan
//             });

//             EditorUtility.SetDirty(manualSettings);
//         }


//         if (GUILayout.Button("Temizle"))
//         {
//             manualSettings.prefabEntries.Clear();
//             EditorUtility.SetDirty(manualSettings);
//         }

//         if (GUILayout.Button("🧷 Kaydet (Scene Dirty)"))
//         {
//             EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
//         }

//         if (GUILayout.Button("🧹 Tüm Level Objelerini Temizle"))
//         {
//             for (int i = 1; i <= 10; i++)
//             {
//                 GameObject level = GameObject.Find("Level" + i);
//                 if (level != null)
//                     DestroyImmediate(level);
//             }
//         }
//     }

//     private void OnSceneGUI(SceneView sceneView)
//     {
//         if (_activeWindow == null || !placeModeEnabled || Application.isPlaying)
//         {
//             if (previewObject != null)
//             {
//                 DestroyImmediate(previewObject);
//                 previewObject = null;
//             }
//             return;
//         }

//         Event e = Event.current;
//         Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
//         Plane plane = new Plane(Vector3.up, Vector3.zero);

//         if (plane.Raycast(ray, out float distance))
//         {
//             Vector3 worldPoint = ray.GetPoint(distance);
//             hoveredCell = new Vector3Int(
//                 Mathf.FloorToInt(worldPoint.x / cellSize),
//                 0,
//                 Mathf.FloorToInt(worldPoint.z / cellSize)
//             );

//             float levelY = (selectedLevel - 1) * cellSize;

//             Vector3 cellCenter = new Vector3(
//                 (hoveredCell.x + 0.5f) * cellSize,
//                 levelY,
//                 (hoveredCell.z + 0.5f) * cellSize
//             );

//             if (previewObject == null && selectedPrefab != null)
//             {
//                 previewObject = (GameObject)PrefabUtility.InstantiatePrefab(selectedPrefab);
//                 previewObject.hideFlags = HideFlags.HideAndDontSave;
//                 previewObject.name = "PreviewObject";

//                 Vector3 originalBounds = GetObjectOriginalBounds(previewObject);
//                 if (originalBounds != Vector3.zero)
//                 {
//                     Vector3 newScale = Vector3.one;

//                     switch (selectedScaleMode)
//                     {
//                         case TileScaleMode.Uniform:
//                             newScale = new Vector3(
//                                 cellSize / originalBounds.x,
//                                 cellSize / originalBounds.y,
//                                 cellSize / originalBounds.z
//                             );
//                             break;
//                         case TileScaleMode.FlatY:
//                             newScale = new Vector3(
//                                 cellSize / originalBounds.x,
//                                 1f,
//                                 cellSize / originalBounds.z
//                             );
//                             break;
//                         case TileScaleMode.None:
//                             newScale = previewObject.transform.localScale; // or Vector3.one
//                             break;
//                     }

//                     previewObject.transform.localScale = newScale;
//                 }

//             }

//             if (previewObject != null)
//             {
//                 previewObject.transform.position = cellCenter;
//                 previewObject.transform.rotation = currentRotation;
//             }

//             if (e.type == EventType.KeyDown && e.keyCode == KeyCode.R)
//             {
//                 currentRotation *= Quaternion.Euler(0, 90, 0);
//                 e.Use();
//             }

//             Vector3 bottomLeft = cellCenter + new Vector3(-cellSize / 2, 0, -cellSize / 2);
//             Vector3 bottomRight = cellCenter + new Vector3(cellSize / 2, 0, -cellSize / 2);
//             Vector3 topRight = cellCenter + new Vector3(cellSize / 2, 0, cellSize / 2);
//             Vector3 topLeft = cellCenter + new Vector3(-cellSize / 2, 0, cellSize / 2);

//             Handles.color = new Color(1f, 1f, 1f, 0.3f);
//             Handles.DrawSolidRectangleWithOutline(
//                 new Vector3[] { bottomLeft, bottomRight, topRight, topLeft },
//                 new Color(1, 1, 1, 0.2f),
//                 new Color(1, 1, 1, 0.4f)
//             );

//             bool isLeftMouseHeld = (e.type == EventType.MouseDrag || e.type == EventType.MouseDown) && e.button == 0 && !e.alt;

//             if (isPlacing && selectedPrefab != null && isLeftMouseHeld && hoveredCell != lastPlacedCell)
//             {
//                 string levelName = "Level" + selectedLevel;
//                 GameObject levelParent = GameObject.Find(levelName);
//                 if (levelParent == null)
//                     levelParent = new GameObject(levelName);

//                 GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(selectedPrefab);
//                 obj.transform.position = cellCenter;
//                 obj.transform.rotation = currentRotation;
//                 obj.transform.SetParent(levelParent.transform);

//                 Vector3 originalBounds = GetObjectOriginalBounds(obj);
//                 if (originalBounds != Vector3.zero)
//                 {
//                     Vector3 newScale = Vector3.one;

//                     switch (selectedScaleMode)
//                     {
//                         case TileScaleMode.Uniform:
//                             newScale = new Vector3(
//                                 cellSize / originalBounds.x,
//                                 cellSize / originalBounds.y,
//                                 cellSize / originalBounds.z
//                             );
//                             break;
//                         case TileScaleMode.FlatY:
//                             newScale = new Vector3(
//                                 cellSize / originalBounds.x,
//                                 1f,
//                                 cellSize / originalBounds.z
//                             );
//                             break;
//                         case TileScaleMode.None:
//                             newScale = obj.transform.localScale; // or Vector3.one
//                             break;
//                     }

//                     obj.transform.localScale = newScale;
//                 }

//                 lastPlacedCell = hoveredCell;
//                 EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
//                 e.Use();
//             }

//             bool isRightMouseHeld = (e.type == EventType.MouseDown || e.type == EventType.MouseDrag) && e.button == 1 && !e.alt;

//             if (isRightMouseHeld && hoveredCell != lastPlacedCell)
//             {
//                 string levelName = "Level" + selectedLevel;
//                 GameObject levelParent = GameObject.Find(levelName);
//                 if (levelParent != null)
//                 {
//                     float threshold = 0.1f;

//                     foreach (Transform child in levelParent.transform)
//                     {
//                         if (Vector3.Distance(child.position, cellCenter) < threshold)
//                         {
//                             DestroyImmediate(child.gameObject);
//                             EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
//                             e.Use();
//                             break;
//                         }
//                     }
//                 }

//                 lastPlacedCell = hoveredCell;
//             }
//         }

//         SceneView.RepaintAll();
//     }

//     private void ResetPreviewObject()
//     {
//         if (previewObject != null)
//         {
//             DestroyImmediate(previewObject);
//             previewObject = null;
//         }
//     }

//     private Vector3 GetObjectOriginalBounds(GameObject obj)
//     {
//         Renderer rend = obj.GetComponentInChildren<Renderer>();
//         if (rend != null)
//             return rend.bounds.size;
//         return Vector3.one;
//     }
// }
