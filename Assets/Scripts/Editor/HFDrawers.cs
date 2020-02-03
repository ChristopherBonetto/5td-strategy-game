using UnityEngine;
using UnityEditor;

#region Properties

public class DrawerHelper : PropertyDrawer
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
public class PBRarityIconDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		DrawerHelper.DrawKVPair(position, property, label, "Key", "Value");
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
