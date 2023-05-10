using UnityEditor;
using UnityEngine;

public class EditorOpenInhierachy : EditorWindow
{
    // create Editor folder in Assets andd put it in

    // ctr + e = open
    // ctr + alt + e = close

    [MenuItem("Window/Custom Hierarchy Window")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<EditorOpenInhierachy>("Custom Hierarchy");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Open All Folders"))
        {
            foreach (GameObject obj in Selection.gameObjects)
            {
                SetExpandedRecursive(obj.transform, true);
            }
        }

        if (GUILayout.Button("Close All Folders"))
        {
            foreach (GameObject obj in Selection.gameObjects)
            {
                SetExpandedRecursive(obj.transform, false);
            }
        }
    }

    private static void SetExpandedRecursive(Transform t, bool expand)
    {
        var type = typeof(EditorWindow).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
        var methodInfo = type.GetMethod("SetExpandedRecursive");

        EditorApplication.ExecuteMenuItem("Window/General/Hierarchy");

        int objId = t.gameObject.GetHashCode();
        var window = EditorWindow.focusedWindow;
        methodInfo.Invoke(window, new object[] { objId, expand });
    }

    [MenuItem("Tools/Open All Folders %e")]
    public static void OpenAllFolders()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            SetExpandedRecursive(obj.transform, true);
        }
    }

    [MenuItem("Tools/Close All Folders %#e")]
    public static void CloseAllFolders()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            SetExpandedRecursive(obj.transform, false);
        }
    }

}
