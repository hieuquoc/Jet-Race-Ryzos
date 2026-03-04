using UnityEngine;
using UnityEditor;

public static class ClearPlayerPrefs
{
    [MenuItem("Tools/Clear Player Prefs")]
    private static void ClearAllPlayerPrefs()
    {
        if (EditorUtility.DisplayDialog("Clear Player Prefs",
            "Are you sure you want to delete ALL PlayerPrefs? This cannot be undone.",
            "Delete All", "Cancel"))
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            EditorUtility.DisplayDialog("Player Prefs cleared", "All PlayerPrefs have been deleted.", "OK");
        }
    }

    [MenuItem("Tools/Clear Player Prefs", true)]
    private static bool ClearAllPlayerPrefsValidate()
    {
        return true;
    }
}
