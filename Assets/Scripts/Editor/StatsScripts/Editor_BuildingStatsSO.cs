using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using BehaviorDesigner.Runtime;
using Types;

[CustomEditor(typeof(BuildingsStatsSO))]
public class Editor_BuildingStatsSO : Editor
{
    BuildingsStatsSO building;

    SerializedProperty m_attackSound, m_deathSound, m_takeDamageSound, m_hittedSound, m_upgradeSound;

    private void OnEnable()
    {
        building = (BuildingsStatsSO)target;

        m_attackSound = serializedObject.FindProperty("AttackSound");
        m_deathSound = serializedObject.FindProperty("DeathSound");
        m_takeDamageSound = serializedObject.FindProperty("TakeDamageSound");
        m_upgradeSound = serializedObject.FindProperty("UpgradeSound");
    }
    private void OnDisable()
    {
        EditorUtility.SetDirty(building);
        AssetDatabase.SaveAssets();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        //BuildingsStatsSO building = target as BuildingsStatsSO;

        GUILayout.Label("GENERAL STATS");
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Unit Type");
        building.BuildingType = (BuildingType)EditorGUILayout.EnumPopup(building.BuildingType);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Unit Icon");
        building.Icon = EditorGUILayout.ObjectField(building.Icon, typeof(Sprite)) as Sprite;
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Name");
        building.Name = EditorGUILayout.TextField(building.Name);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Description");
        building.Description = EditorGUILayout.TextArea(building.Description);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Default Description");
        building.DefaultDescription = EditorGUILayout.TextArea(building.DefaultDescription);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();


        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Building Prefab");
        building.VisualPrefab = (GameObject)EditorGUILayout.ObjectField(building.VisualPrefab, typeof(GameObject), false);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        if(building.VisualPrefab == null)
        {
            EditorGUILayout.HelpBox("This entity need a prefab", MessageType.Warning);
        }
        

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        GUILayout.Label("DEFENSIVE STATS");
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Can Take Damage");
        building.CanTakeDamage = EditorGUILayout.Toggle(building.CanTakeDamage);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        if (building.CanTakeDamage)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Health");
            building.MaxHp = EditorGUILayout.IntSlider(building.MaxHp, 0, 999);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Armor");
            building.Armor = EditorGUILayout.IntSlider(building.Armor, 0, 999);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }



        EditorGUILayout.Space();
        EditorGUILayout.Space();
        GUILayout.Label("OFFENSIVE STATS");
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Can Attack");
        building.CanAttack = EditorGUILayout.Toggle(building.CanAttack);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        if (building.CanAttack)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Damage");
            building.Damage = EditorGUILayout.IntSlider(building.Damage, 0, 999);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Attack Range");
            building.AttackRange = EditorGUILayout.IntField(building.AttackRange);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Attack Speed");
            building.AttackSpeed = EditorGUILayout.Slider(building.AttackSpeed, 0, 999);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Projectile");
            building.Projectile = (HFPoolID)EditorGUILayout.ObjectField(building.Projectile, typeof(HFPoolID), false);
            EditorGUILayout.EndHorizontal();

            if (building.Projectile == null)
            {
                EditorGUILayout.HelpBox("This entity need a projectile", MessageType.Warning);
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        GUILayout.Label("OTHER STATS");
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Weight");
        building.Weight = EditorGUILayout.IntField(building.Weight);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        GUILayout.Label("Cost");
        EditorGUILayout.BeginHorizontal();
        building.Cost = EditorGUILayout.IntField(building.Cost);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        GUILayout.Label("SOUNDS EFFECTS");
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(m_attackSound, new GUIContent("Attack Sound"), true);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(m_deathSound, new GUIContent("Death Sound"), true);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(m_takeDamageSound, new GUIContent("Take Damage Sound"), true);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(m_upgradeSound, new GUIContent("Upgrade Sound"), true);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        serializedObject.ApplyModifiedProperties();
    }
}
