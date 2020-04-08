using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ExtendedEditorWindow : EditorWindow
{
    protected SerializedObject m_serializedObject;
    protected SerializedProperty m_serializedProperty;

    private string m_selectedPropertyPath;
    protected SerializedProperty m_selectedProperty;

    protected virtual void DrawProperties(SerializedProperty prop, bool drawChildren)
    {
        string lastPropPath = string.Empty;

        
        foreach (SerializedProperty p in prop)
        {
            if (p == null) return;

            if (p.isArray && p.propertyType == SerializedPropertyType.Generic)
            {
                EditorGUILayout.BeginHorizontal();
                p.isExpanded = EditorGUILayout.Foldout(p.isExpanded, p.name);
                EditorGUILayout.EndHorizontal();

                if (p.isExpanded)
                {
                    EditorGUI.indentLevel++;
                    DrawProperties(p, drawChildren);
                    EditorGUI.indentLevel--;
                }
            }

            else
            {
                if (!string.IsNullOrEmpty(lastPropPath) && p.propertyPath.Contains(lastPropPath)) { continue; }
                lastPropPath = p.propertyPath;
                EditorGUILayout.PropertyField(p, drawChildren);
            }
        }
    }

    protected virtual void DrawSidebar(SerializedProperty prop)
    {
        if (prop == null)
        {
            return;
        }

        foreach (SerializedProperty p in prop)
        {
            if (GUILayout.Button(p.displayName))
            {
                m_selectedPropertyPath = p.propertyPath;
            }
        }

        if (!string.IsNullOrEmpty(m_selectedPropertyPath))
        {
            m_selectedProperty = m_serializedObject.FindProperty(m_selectedPropertyPath);
        }
    }
}
