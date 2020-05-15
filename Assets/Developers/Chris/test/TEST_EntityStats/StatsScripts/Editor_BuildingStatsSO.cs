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

    private void OnEnable()
    {
        building = (BuildingsStatsSO)target;
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
        building.Icon = (Sprite)EditorGUILayout.ObjectField("Sprite", building.Icon, typeof(Sprite), true);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Name");
        building.Name = EditorGUILayout.TextField(building.Name);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("BehaviorHandler");
        building.BehaviorHandler = (GameObject)EditorGUILayout.ObjectField(building.BehaviorHandler, typeof(GameObject), false);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Building Prefab");
        building.VisualPrefab = (GameObject)EditorGUILayout.ObjectField(building.VisualPrefab, typeof(GameObject), false);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        if(building.VisualPrefab == null || building.BehaviorHandler == null)
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
            building.Projectile = (GameObject)EditorGUILayout.ObjectField(building.Projectile, typeof(GameObject), false);
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

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Upgrades");
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Upgrades"), true);
        EditorGUILayout.EndHorizontal();

        GUILayout.Label("Cost");
        EditorGUILayout.BeginHorizontal();
        building.Cost.ResourceType = (ResourceType)EditorGUILayout.EnumPopup(building.Cost.ResourceType);
        building.Cost.ResourceQuantity = EditorGUILayout.IntField(building.Cost.ResourceQuantity);
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }
}
