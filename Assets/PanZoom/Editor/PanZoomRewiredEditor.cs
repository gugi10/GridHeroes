using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Dossamer.PanZoom
{
    public class PanZoomRewiredEditor : PanZoomBehaviorEditor
    {
        SerializedProperty playerId;
        SerializedProperty rewiredHorizontalAxis;
        SerializedProperty rewiredVerticalAxis;

        protected override void OnEnable()
        {
            base.OnEnable();

            playerId = serializedObject.FindProperty("playerId");
            rewiredHorizontalAxis = serializedObject.FindProperty("rewiredHorizontalAxis");
            rewiredVerticalAxis = serializedObject.FindProperty("rewiredVerticalAxis");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Rewired Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(playerId, new GUIContent("Rewired Player ID", "Defaults to 0"));
            EditorGUILayout.PropertyField(rewiredHorizontalAxis, new GUIContent("Horizontal Axis Name", "You can define an axis by going to the Rewired Input Manager"));
            EditorGUILayout.PropertyField(rewiredVerticalAxis, new GUIContent("Vertical Axis Name", "You can define an axis by going to the Rewired Input Manager"));

            serializedObject.ApplyModifiedProperties();

            base.OnInspectorGUI();
        }
    }
}
