using UnityEngine;
using UnityEditor;

namespace ZyroX{
    [CustomEditor(typeof(MapController))]
public class MapControllerEditor : Editor
{
    private float inputValue = 0f;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();
        inputValue = EditorGUILayout.FloatField("Set Value", inputValue);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Set Run Distance"))
        {
            SetRunDistance(inputValue);
        }
        if (GUILayout.Button("Set Loop Start Distance"))
        {
            SetLoopStartDistance(inputValue);
        }
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Set Both Distances"))
        {
            SetBoth(inputValue);
        }
    }

    private void SetRunDistance(float value)
    {
        if (MapController.Instance == null)
        {
            Debug.LogWarning("MapController.Instance is null. Ensure a MapController exists in the scene.");
            return;
        }
        Undo.RecordObject(MapController.Instance, "Set Run Distance");
        MapController.RunDistance = value;
        EditorUtility.SetDirty(MapController.Instance);
        Debug.Log($"RunDistance set to {value}");
    }

    private void SetLoopStartDistance(float value)
    {
        if (MapController.Instance == null)
        {
            Debug.LogWarning("MapController.Instance is null. Ensure a MapController exists in the scene.");
            return;
        }
        Undo.RecordObject(MapController.Instance, "Set Loop Start Distance");
        MapController.LoopStartDistance = value;
        EditorUtility.SetDirty(MapController.Instance);
        Debug.Log($"LoopStartDistance set to {value}");
    }

    private void SetBoth(float value)
    {
        if (MapController.Instance == null)
        {
            Debug.LogWarning("MapController.Instance is null. Ensure a MapController exists in the scene.");
            return;
        }
        Undo.RecordObject(MapController.Instance, "Set Distances");
        MapController.RunDistance = value;
        MapController.LoopStartDistance = value;
        EditorUtility.SetDirty(MapController.Instance);
        Debug.Log($"RunDistance and LoopStartDistance set to {value}");
    }
}

}

