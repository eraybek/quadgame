#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

public class TilemapPreviewHandler
{
    private GameObject previewObject;
    private GridObjectSO lastData;

    public void UpdatePreview(GridObjectSO data, Vector3 position, Quaternion rotation)
    {
        if (data != lastData)
        {
            ClearPreview();
            lastData = data;
        }

        // Prefab objesi GameObject.Instantiate ile alınmalı (Preview için daha doğru çalışır)
        if (previewObject == null && data != null && data.Prefab != null)
        {
            GameObject temp = GameObject.Instantiate(data.Prefab);
            temp.hideFlags = HideFlags.HideAndDontSave;
            temp.name = "PreviewObject";

            previewObject = temp;
        }

        if (previewObject != null)
        {
            previewObject.transform.position = position + data.ManualPositionOffset;
            previewObject.transform.rotation = Quaternion.Euler(data.ManualRotation) * rotation;
            previewObject.transform.localScale = data.ManualScale;

            SetTransparentMaterial(previewObject);
        }
    }

    public void ClearPreview()
    {
        if (previewObject != null)
        {
            Object.DestroyImmediate(previewObject);
            previewObject = null;
        }

        lastData = null;
    }

    private void SetTransparentMaterial(GameObject obj)
    {
        foreach (var renderer in obj.GetComponentsInChildren<Renderer>())
        {
            foreach (var mat in renderer.sharedMaterials)
            {
                if (mat != null && mat.HasProperty("_Color"))
                {
                    Color c = mat.color;
                    c.a = 0.5f; // yarı saydam
                    mat.color = c;
                }
            }
        }
    }
}
#endif