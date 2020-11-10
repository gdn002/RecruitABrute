using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(Entity))]
public class EntityEditor : Editor
{

    public override void OnInspectorGUI()
    {
        Entity castedTarget = (Entity)target;

        EditorGUI.BeginChangeCheck();

        CreateCollisionGUI(castedTarget);
        CreateCoordinatesGUI(castedTarget);
        
        if (EditorGUI.EndChangeCheck() && !Application.isPlaying)
        {
            castedTarget.UpdateEntity();

            EditorUtility.SetDirty(castedTarget);
            EditorSceneManager.MarkSceneDirty(castedTarget.gameObject.scene);
        }

        serializedObject.ApplyModifiedProperties();
    }

    protected void CreateCollisionGUI(Entity castedTarget)
    {
        GUILayout.Label("Grid Collision", EditorStyles.boldLabel);
        castedTarget.hasCollision = EditorGUILayout.Toggle("Enabled: ", castedTarget.hasCollision);
    }

    protected void CreateCoordinatesGUI(Entity castedTarget)
    {
        GUILayout.Label("Grid Coordinates", EditorStyles.boldLabel);

        castedTarget.coordinates = EditorGUILayout.Vector2IntField("Coordinates: ", castedTarget.coordinates);

        if (GUILayout.Button(new GUIContent("Nearest Coordinates", "Move this grid object to the nearest grid cell based on its current position.")))
        {
            castedTarget.MoveToNearest();
        }
        if (GUILayout.Button(new GUIContent("North", "Move this grid object one step North.")))
        {
            castedTarget.coordinates += Vector2Int.up;
        }
        if (GUILayout.Button(new GUIContent("South", "Move this grid object one step South.")))
        {
            castedTarget.coordinates += Vector2Int.down;
        }
        if (GUILayout.Button(new GUIContent("East", "Move this grid object one step East.")))
        {
            castedTarget.coordinates += Vector2Int.right;
        }
        if (GUILayout.Button(new GUIContent("West", "Move this grid object one step West.")))
        {
            castedTarget.coordinates += Vector2Int.left;
        }
    }
}
