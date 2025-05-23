#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

using UnityEngine;
using System.Linq;

public class TilemapTool : EditorWindow
{
    [SerializeField] private TilemapObjectsSO toolSO;
    [SerializeField] private TilemapSaveSO saveSO;

    private static TilemapTool _activeWindow;

    private GridObjectSO selectedPrefab;
    private TilemapPreviewHandler previewHandler = new();
    private bool isPlacing = false;
    private bool placeModeEnabled = true;
    private Vector2 scrollPosition;

    private float cellSize = 1f;
    private Vector3Int hoveredCell;
    private Vector3Int lastPlacedCell = new Vector3Int(int.MinValue, int.MinValue, int.MinValue);
    private Quaternion currentRotation = Quaternion.identity;
    private GridCategory selectedCategoryFilter = GridCategory.None;
    private Grid targetGrid;

    [MenuItem("Tools/Tilemap Tool")]
    public static void ShowWindow()
    {
        var window = GetWindow<TilemapTool>("Tilemap Tool");
        _activeWindow = window;
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
        _activeWindow = this;

        targetGrid = FindFirstObjectByType<Grid>(); // otomatik bul

        if (targetGrid != null)
            cellSize = targetGrid.cellSize.x;
        else
            cellSize = 1f;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
        _activeWindow = null;
        previewHandler.ClearPreview();
    }

    private void OnGUI()
    {
        if (targetGrid == null)
        {
            EditorGUILayout.HelpBox("Grid sahnede bulunamadı!", MessageType.Warning);
            return;
        }

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        // --- GUI bileşenleri ---
        DrawHeader();
        DrawSettings();
        DrawCategoryFilter();
        if (toolSO != null)
        {
            DrawPrefabButtons();
            DrawSaveLoadButtons();
        }

        EditorGUILayout.EndScrollView();

    }


    private void OnSceneGUI(SceneView sceneView)
    {
        if (_activeWindow == null || !placeModeEnabled || Application.isPlaying)
        {
            previewHandler.ClearPreview();
            return;
        }

        Event e = Event.current;
        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        Plane plane = new Plane(Vector3.up, Vector3.zero);

        if (!plane.Raycast(ray, out float distance)) return;

        Vector3 worldPoint = ray.GetPoint(distance);
        hoveredCell = new Vector3Int(
            Mathf.FloorToInt(worldPoint.x / cellSize),
            0,
            Mathf.FloorToInt(worldPoint.z / cellSize)
        );

        Vector3 cellCenter = new Vector3(
            (hoveredCell.x + 0.5f) * cellSize,
            0f,
            (hoveredCell.z + 0.5f) * cellSize
        );


        if (isPlacing && selectedPrefab != null)
        {
            Quaternion previewRotation = Quaternion.Euler(selectedPrefab.ManualRotation) * currentRotation;
            Vector3 previewPosition = cellCenter + selectedPrefab.ManualPositionOffset;

            previewHandler.UpdatePreview(selectedPrefab, previewPosition, previewRotation);
        }

        HandleKeyboardInput(e);
        HandleMouseInput(e, cellCenter);
        DrawGridHighlight(cellCenter);

        SceneView.RepaintAll();
    }

    private void DrawHeader()
    {
        GUILayout.Label("Tilemap Tool", EditorStyles.boldLabel);
        placeModeEnabled = EditorGUILayout.ToggleLeft("Place Mode", placeModeEnabled);

        if (placeModeEnabled)
        {
            EditorGUILayout.HelpBox("Place Mode Enabled", MessageType.Info);
        }

    }

    private void DrawSettings()
    {
        toolSO = (TilemapObjectsSO)EditorGUILayout.ObjectField("Tool SO", toolSO, typeof(TilemapObjectsSO), false);
        saveSO = (TilemapSaveSO)EditorGUILayout.ObjectField("Save SO", saveSO, typeof(TilemapSaveSO), false);

        if (toolSO == null)
        {
            EditorGUILayout.HelpBox("Tool SO is required!", MessageType.Warning);
        }
    }

    private void DrawCategoryFilter()
    {
        selectedCategoryFilter = (GridCategory)EditorGUILayout.EnumPopup("Category", selectedCategoryFilter);
    }

    private void DrawPrefabButtons()
    {
        GUILayout.Space(10);
        GUILayout.Label("Prefabs", EditorStyles.boldLabel);

        var filteredPrefabs = toolSO.prefabList
            .Where(p => p != null && p.IsActive) // 👈 sadece aktif olanlar
            .Where(p => selectedCategoryFilter == GridCategory.None || p.Category == selectedCategoryFilter)
            .ToList();


        foreach (var data in filteredPrefabs)
        {
            // Rastgele ama tutarlı renk üret (örneğin isme göre hash)
            GUI.backgroundColor = GetColorFromName(data.name);

            if (GUILayout.Button(data.name, GUILayout.Height(30)))
            {
                selectedPrefab = data;
                isPlacing = true;
                currentRotation = Quaternion.identity;
                previewHandler.ClearPreview();
                lastPlacedCell = new Vector3Int(int.MinValue, int.MinValue, int.MinValue);
            }

            GUI.backgroundColor = Color.white; // geri sıfırla
        }
    }

    private Color GetColorFromName(string name)
    {
        int hash = name.GetHashCode();
        Random.InitState(hash); // Aynı isim için aynı renk
        return new Color(
            Random.Range(0.3f, 0.8f), // R
            Random.Range(0.3f, 0.8f), // G
            Random.Range(0.3f, 0.8f)  // B
        );
    }



    private void DrawSaveLoadButtons()
    {
        GUILayout.Space(10);
        if (GUILayout.Button("🧷 Save (Scene Dirty)"))
        {
            EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        }

        if (GUILayout.Button(new GUIContent("Clear All", EditorGUIUtility.IconContent("d_TreeEditor.Trash").image)))
        {
            GameObject level = GameObject.Find("LEVEL_OBJECTS");
            if (level != null)
                DestroyImmediate(level);
        }

        if (GUILayout.Button("Save Tilemap") && saveSO != null)
            TilemapSaveLoadManager.Save(saveSO);

        if (GUILayout.Button("Load Tilemap") && saveSO != null)
            TilemapSaveLoadManager.Load(saveSO);
    }

    private void HandleKeyboardInput(Event e)
    {
        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.R)
        {
            currentRotation *= Quaternion.Euler(0, 90, 0);
            e.Use();
        }
    }

    private void HandleMouseInput(Event e, Vector3 cellCenter)
    {
        bool isLeftMouse = (e.type == EventType.MouseDown || e.type == EventType.MouseDrag) && e.button == 0 && !e.alt;
        bool isRightMouse = (e.type == EventType.MouseDown || e.type == EventType.MouseDrag) && e.button == 1 && !e.alt;

        if (isPlacing && selectedPrefab != null && isLeftMouse && hoveredCell != lastPlacedCell)
        {
            TilemapPlacementHandler.PlacePrefab(selectedPrefab, cellCenter, currentRotation);
            lastPlacedCell = hoveredCell;
            e.Use();
        }
        else if (isRightMouse && hoveredCell != lastPlacedCell)
        {
            TilemapPlacementHandler.RemovePrefab(cellCenter);
            lastPlacedCell = hoveredCell;
            e.Use();
        }
    }

    private void DrawGridHighlight(Vector3 cellCenter)
    {
        Vector3 bottomLeft = cellCenter + new Vector3(-cellSize / 2, 0, -cellSize / 2);
        Vector3 bottomRight = cellCenter + new Vector3(cellSize / 2, 0, -cellSize / 2);
        Vector3 topRight = cellCenter + new Vector3(cellSize / 2, 0, cellSize / 2);
        Vector3 topLeft = cellCenter + new Vector3(-cellSize / 2, 0, cellSize / 2);

        Handles.color = new Color(1f, 1f, 1f, 0.3f);
        Handles.DrawSolidRectangleWithOutline(
            new Vector3[] { bottomLeft, bottomRight, topRight, topLeft },
            new Color(1, 1, 1, 0.2f),
            new Color(1, 1, 1, 0.4f)
        );
    }

    public static void RepaintWindow()
    {
        if (_activeWindow != null)
        {
            _activeWindow.Repaint();             // OnGUI'yi tetikler
            SceneView.RepaintAll();              // Scene'de preview'u günceller
        }
    }


}
#endif