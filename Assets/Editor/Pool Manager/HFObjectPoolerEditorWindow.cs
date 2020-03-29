using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class HFObjectPoolerEditorWindow : ExtendedEditorWindow
{
    public static void Open(HFObjectPooler manager)
    {
        HFObjectPoolerEditorWindow window = GetWindow<HFObjectPoolerEditorWindow>("Object Pooler");
        window.m_serializedObject = new SerializedObject(manager);
    }

    private void OnGUI()
    {
        m_serializedProperty = m_serializedObject.FindProperty("m_poolItems");

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.BeginVertical("box", GUILayout.MaxWidth(150), GUILayout.ExpandHeight(true));
        DrawSidebar(m_serializedProperty);

        EditorGUILayout.Space();

        DrawAddElementButton();
        DrawDeleteElementButton();

        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("box", GUILayout.ExpandHeight(true));
        if (m_selectedProperty != null)
        {
            DrawProperties(m_selectedProperty, true);
        }
        else
        {
            EditorGUILayout.LabelField("Select an item from the list");
        }

        // Remove button

        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();

        if (EditorGUI.EndChangeCheck())
            m_serializedObject.ApplyModifiedProperties();
    }

    protected void DrawAddElementButton()
    {
        if (GUILayout.Button("Add", GUILayout.MaxWidth(150)))
        {
            m_serializedProperty.arraySize++;
        }
    }

    protected void DrawDeleteElementButton()
    {
        if (GUILayout.Button("Remove the last", GUILayout.MaxWidth(150)))
        {
            if (m_serializedProperty.arraySize > 1)
            {
                m_selectedProperty = null;

                if (m_serializedProperty.GetArrayElementAtIndex(m_serializedProperty.arraySize - 1) != m_selectedProperty)
                {
                    if (EditorUtility.DisplayDialog("Delete", "Are you sure you want to delete the last poolable object? This cannot be undone.", "OK", "Cancel"))
                    {
                        m_serializedProperty.DeleteArrayElementAtIndex(m_serializedProperty.arraySize - 1);
                    }
                }
            }
        }
    }
}
