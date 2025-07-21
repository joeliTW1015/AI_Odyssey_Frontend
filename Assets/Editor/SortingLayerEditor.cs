using UnityEditor;
using UnityEngine;

// <summary>
// This script provides a custom editor window to set the sorting layer of all child renderers
// of a specified GameObject in Unity.
public class SortingLayerEditor : EditorWindow
{
    private GameObject targetObject;
    private string newSortingLayer = "Default";

    [MenuItem("Tools/Set Child Renderers Sorting Layer")]
    public static void ShowWindow()
    {
        GetWindow<SortingLayerEditor>("Set Sorting Layer");
    }

    void OnGUI()
    {
        targetObject = (GameObject)EditorGUILayout.ObjectField("父物件", targetObject, typeof(GameObject), true);
        newSortingLayer = EditorGUILayout.TextField("新 Sorting Layer 名稱", newSortingLayer);

        if (GUILayout.Button("修改所有子物件 Sorting Layer"))
        {
            if (targetObject != null)
            {
                ApplySortingLayer(targetObject.transform, newSortingLayer);
            }
        }
    }

    void ApplySortingLayer(Transform parent, string layerName)
    {
        Renderer[] renderers = parent.GetComponentsInChildren<Renderer>(true);
        foreach (Renderer r in renderers)
        {
            Undo.RecordObject(r, "Change Sorting Layer");
            r.sortingLayerName = layerName;
            EditorUtility.SetDirty(r);
        }

        Debug.Log($"共 {renderers.Length} 個渲染元件已套用 Sorting Layer: {layerName}");
    }
}
