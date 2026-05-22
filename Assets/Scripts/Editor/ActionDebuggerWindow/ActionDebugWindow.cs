using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ActionDebugWindow : EditorWindow
{
    private Dictionary<BaseAction, bool> _foldoutStates = new();
    private bool _collapseFoldout;
    private Vector2 _scrollPosition;

    [MenuItem("Team 5 tools/Action Debug Window")]
    public static void OpenWindow()
    {
        GetWindow<ActionDebugWindow>("Action Debug");
    }

    private void OnGUI()
    {
        GUILayout.Label("Active Actions", EditorStyles.boldLabel);
        GUILayout.Space(10);
        GUILayout.Label("Expand action chain on creation");
        _collapseFoldout = GUILayout.Toggle(_collapseFoldout, "");
        GUILayout.Space(10);

        if (!Application.isPlaying)
        {
            GUILayout.Label("Enter Play Mode to view actions.");
            return;
        }

        if (ActionDebugRegistry.ActiveActions == null)
        {
            GUILayout.Label("No registry found.");
            return;
        }

        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

        foreach (var action in ActionDebugRegistry.ActiveActions)
        {
            if (action.Parent != null) continue;

            DrawActionRecursive(action, 0);
        }

        EditorGUILayout.EndScrollView();

        Repaint();
    }

    private void DrawActionRecursive(BaseAction action, int indent)
    {
        if (action == null) return;

        bool hasChildren = action.ChainedActions != null && action.ChainedActions.Count > 0;

        if (!_foldoutStates.ContainsKey(action))
        {
            _foldoutStates[action] = _collapseFoldout;
        }

        GUILayout.BeginHorizontal();

        GUILayout.Space(indent * 18);

        GUI.backgroundColor = GetStateColor(action.State);

        GUILayout.BeginVertical("box");

        GUI.backgroundColor = Color.white;

        GUILayout.Space(2);

        GUILayout.BeginHorizontal();

        if (hasChildren)
        {
            _foldoutStates[action] = EditorGUILayout.Foldout(
                _foldoutStates[action],
                "",
                true
            );
        }
        else
        {
            GUILayout.Space(16);
        }

        GUILayout.Label(
            $"{action.GetType().Name}",
            EditorStyles.boldLabel
        );

        GUILayout.FlexibleSpace();

        GUILayout.Label(action.State.ToString());

        GUILayout.EndHorizontal();

        GUILayout.Space(2);

        GUILayout.EndVertical();

        GUILayout.EndHorizontal();

        if (hasChildren && _foldoutStates[action])
        {
            foreach (var child in action.ChainedActions)
            {
                DrawActionRecursive(child, indent + 1);
            }
        }
    }

    private Color GetStateColor(BaseAction.ActionState state)
    {
        return state switch
        {
            BaseAction.ActionState.Running => new Color(0.35f, 0.55f, 1f, 1f),
            BaseAction.ActionState.Waiting => new Color(1f, 0.7f, 0.2f, 1f),
            BaseAction.ActionState.Completed => new Color(0.3f, 0.8f, 0.3f, 1f),
            BaseAction.ActionState.Canceled => new Color(1f, 0.3f, 0.3f, 1f),
            _ => new Color(0.25f, 0.25f, 0.25f, 1f)
        };
    }
}
