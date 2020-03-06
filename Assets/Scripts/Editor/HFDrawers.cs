using UnityEngine;
using UnityEditor;

#region Editors

public class HFEditorHelper : Editor
{
	public static void ShowList(SerializedProperty list, bool includeChildren, string displayName = "")
	{
		EditorGUILayout.Space();
		string label = (displayName != "" ? displayName : list.displayName);
		EditorGUILayout.LabelField(label);
		list.isExpanded = true;
		EditorGUI.indentLevel += 1;
		if (list.isExpanded && includeChildren)
		{
			if (list.arraySize == 0 && GUILayout.Button("+", EditorStyles.miniButton))
			{
				list.arraySize += 1;
			}
			for (int i = 0; i < list.arraySize; i++)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PropertyField(list.GetArrayElementAtIndex(i));
				ShowListButtons(list, i);
				EditorGUILayout.EndHorizontal();
			}
		}
		EditorGUI.indentLevel -= 1;
	}

	private static void ShowListButtons(SerializedProperty list, int index)
	{
		if (GUILayout.Button("+", EditorStyles.miniButtonLeft, GUILayout.Width(20f)))
		{
			list.InsertArrayElementAtIndex(index);
		}
		if (GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.Width(20f)))
		{
			list.DeleteArrayElementAtIndex(index);
		}
	}
}

[CustomEditor(typeof(HFBaseStats), true)]
public class HFBaseStatsEditor : Editor
{
	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("Icon"));
		EditorGUILayout.Space();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("UnitType"));
		EditorGUILayout.Space();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("Visuals"));
		EditorGUILayout.Space();
		HFEditorHelper.ShowList(serializedObject.FindProperty("m_levels").GetArrayElementAtIndex(0).FindPropertyRelative("List"), true, "Level 2 upgrades");
		HFEditorHelper.ShowList(serializedObject.FindProperty("m_levels").GetArrayElementAtIndex(1).FindPropertyRelative("List"), true, "Level 3 upgrades");
		EditorGUILayout.Space();
		EditorGUILayout.PropertyField(serializedObject.FindProperty("RewardCondition"));
		HFEditorHelper.ShowList(serializedObject.FindProperty("m_floatStats"), true);
		HFEditorHelper.ShowList(serializedObject.FindProperty("m_intStats"), true);
		HFEditorHelper.ShowList(serializedObject.FindProperty("m_stringStats"), true);
		HFEditorHelper.ShowList(serializedObject.FindProperty("m_boolStats"), true);
		serializedObject.ApplyModifiedProperties();
	}
}

[CustomEditor(typeof(HFStatUpgrade), true)]
public class HFStatUpgradeEditor : Editor
{
	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		HFEditorHelper.ShowList(serializedObject.FindProperty("m_floatAddModifiers"), true);
		HFEditorHelper.ShowList(serializedObject.FindProperty("m_pctModifiers"), true);
		HFEditorHelper.ShowList(serializedObject.FindProperty("m_intAddModifiers"), true);
		HFEditorHelper.ShowList(serializedObject.FindProperty("m_stringModifiers"), true);
		serializedObject.ApplyModifiedProperties();
	}
}

[CustomEditor(typeof(HF.HFUnitVisuals), true)]
public class HFUnitVisualsEditor : Editor
{
	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		EditorGUILayout.LabelField("Pivot", EditorStyles.boldLabel);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("m_pivot"));
		EditorGUILayout.PropertyField(serializedObject.FindProperty("m_usesPivot"));
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Target", EditorStyles.boldLabel);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("m_target"));
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Level Meshes", EditorStyles.boldLabel);
		HFEditorHelper.ShowList(serializedObject.FindProperty("m_visuals").GetArrayElementAtIndex(0).FindPropertyRelative("List"), true, "Level 1");
		HFEditorHelper.ShowList(serializedObject.FindProperty("m_visuals").GetArrayElementAtIndex(1).FindPropertyRelative("List"), true, "Level 2");
		HFEditorHelper.ShowList(serializedObject.FindProperty("m_visuals").GetArrayElementAtIndex(2).FindPropertyRelative("List"), true, "Level 3");
		serializedObject.ApplyModifiedProperties();
	}
}

#endregion

#region Properties

public class HFDrawerHelper : PropertyDrawer
{
	public static void DrawKVPair(Rect position, SerializedProperty property, GUIContent label, string keyName, string valueName)
	{
		EditorGUI.BeginProperty(position, label, property);

		EditorGUI.indentLevel += 1;
		position.width *= 0.5f;
		EditorGUI.PropertyField(position, property.FindPropertyRelative(keyName), GUIContent.none);
		EditorGUI.indentLevel -= 1;

		position.x += position.width;
		EditorGUI.PropertyField(position, property.FindPropertyRelative(valueName), GUIContent.none);

		EditorGUI.EndProperty();
	}
}

[CustomPropertyDrawer(typeof(HFFloatKV))]
public class HFFloatKVDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		HFDrawerHelper.DrawKVPair(position, property, label, "Key", "Value");
	}
}

[CustomPropertyDrawer(typeof(HFIntKV))]
public class HFIntKVDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		HFDrawerHelper.DrawKVPair(position, property, label, "Key", "Value");
	}
}

[CustomPropertyDrawer(typeof(HFStringKV))]
public class HFStringKVDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		HFDrawerHelper.DrawKVPair(position, property, label, "Key", "Value");
	}
}

[CustomPropertyDrawer(typeof(HFBoolKV))]
public class HFBoolKVDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		HFDrawerHelper.DrawKVPair(position, property, label, "Key", "Value");
	}
}

#endregion

#region Attributes

[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		GUI.enabled = false;
		EditorGUI.PropertyField(position, property, label, true);
		GUI.enabled = true;
	}
}

#endregion
