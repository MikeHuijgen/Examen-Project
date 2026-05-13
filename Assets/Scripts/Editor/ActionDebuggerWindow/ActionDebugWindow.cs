using UnityEditor;
using UnityEngine;

public class ActionDebugWindow : EditorWindow
{
    [MenuItem("Tools/Action Debug Window")]
    public static void OpenWindow()
    {
        GetWindow<ActionDebugWindow>("Action Debug");
    }

    private void OnGUI()
    {
        GUILayout.Label("Active Actions", EditorStyles.boldLabel);

        if (Application.isPlaying == false)
        {
            GUILayout.Label("Enter Play Mode to view actions.");
            return;
        }

        if (ActionDebugRegistry.ActiveActions == null)
        {
            GUILayout.Label("No registry found.");
            return;
        }

        foreach (var action in ActionDebugRegistry.ActiveActions)
        {
            if (action == null) continue;

            GUILayout.BeginVertical("box");

            GUILayout.Label(action.GetType().Name);
            GUILayout.Label($"State: {action.actionState}");

            GUILayout.EndVertical();
        }

        Repaint();
    }
}
