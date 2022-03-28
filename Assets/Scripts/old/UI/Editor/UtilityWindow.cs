#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

public class DclUtilityWindow : EditorWindow
{

    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/dcl-edit Utilities")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        DclUtilityWindow window = (DclUtilityWindow)EditorWindow.GetWindow(typeof(DclUtilityWindow));
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Selection", EditorStyles.boldLabel);

        if (GUILayout.Button("Unity Selection >> dcl Selection"))
        {
            SelectUnityToDcl();
        }


        if (GUILayout.Button("dcl Selection >> Unity Selection"))
        {
            SelectDclToUnity();
        }
    }
    
    private void SelectUnityToDcl()
    {
        var go = Selection.activeGameObject;

        while (go != null)
        {
            if (go.TryGetComponent<Entity>(out var e))
            {
                DclSceneManager.SetSelection(e);
                return;
            }

            go = go.transform.parent?.gameObject;
        }

        DclSceneManager.SetSelection(null);
    }
    private void SelectDclToUnity()
    {
        Selection.activeGameObject = DclSceneManager.PrimarySelectedEntity?.gameObject;
    }
}

#endif