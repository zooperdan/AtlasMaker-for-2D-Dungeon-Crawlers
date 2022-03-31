using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace zooperdan.AtlasMaker
{

    [CustomEditor(typeof(Atlas))]
    public class AtlasEditor : Editor
    {
        public SerializedProperty idProp;
        public SerializedProperty enabledProp;
        public SerializedProperty layersProp;
        public SerializedProperty typeProp;

        private Atlas _target;

        private List<string> _generateResult = new List<string>();

        void OnEnable()
        {
            _target = (Atlas)target;

            idProp = serializedObject.FindProperty("id");
            enabledProp = serializedObject.FindProperty("enabled");
            layersProp = serializedObject.FindProperty("layers");
            typeProp = serializedObject.FindProperty("type");

        }

        public override void OnInspectorGUI()
        {

            GUILayout.BeginHorizontal("HelpBox");
            EditorGUILayout.Space();
            GUILayout.BeginVertical();
            EditorGUILayout.LabelField("Atlas");
            GUILayout.EndVertical();
            EditorGUILayout.Space();
            GUILayout.EndVertical();

            serializedObject.Update();

            EditorGUILayout.PropertyField(idProp, new GUIContent("ID"), GUILayout.Height(20));
            EditorGUILayout.PropertyField(enabledProp, new GUIContent("Enabled"), GUILayout.Height(20));

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (GUILayout.Button("Add layer"))
            {
                _target.layers.Add(null);
            }

            EditorGUI.indentLevel++;

            for (int i = 0; i < layersProp.arraySize; i++)
            {

                SerializedProperty listRef = layersProp.GetArrayElementAtIndex(i);
                GUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(listRef);
                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    layersProp.DeleteArrayElementAtIndex(i);
                }
                if (GUILayout.Button("U", GUILayout.Width(24)))
                {
                    if (i > 0)
                    {
                        MoveUp(serializedObject, layersProp, i);
                    }
                }
                if (GUILayout.Button("D", GUILayout.Width(24)))
                {
                    int arrayLength = layersProp.arraySize;

                    if (i < arrayLength - 1)
                    {
                        MoveDown(serializedObject, layersProp, i);
                    }
                }
                GUILayout.EndHorizontal();
            }

            EditorGUI.indentLevel--;

            serializedObject.ApplyModifiedProperties();
        }

        static public void MoveDown(SerializedObject target, SerializedProperty array, int index)
        {
            array.MoveArrayElement(index, index + 1);
        }

        static public void MoveUp(SerializedObject target, SerializedProperty array, int index)
        {
            array.MoveArrayElement(index, index - 1);
        }

    }

}