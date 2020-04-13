using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Types;

[CustomEditor(typeof(BuildingsSO))]
public class Editor_BuildingSO : Editor
{
    public override void OnInspectorGUI()
    {
        BuildingsSO building = target as BuildingsSO;

        GUILayout.Label("GENERAL STATS");
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Unit Type");
        building.BuildingType = (BuildingType)EditorGUILayout.EnumPopup(building.BuildingType);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Name");
        building.Name = EditorGUILayout.TextField(building.Name);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();



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
        GUILayout.Label("Cost");
        building.Cost = EditorGUILayout.IntField(building.Cost);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Building Type");
        building.BuildingType = (BuildingType)EditorGUILayout.EnumPopup(building.BuildingType);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Weight");
        building.Weight = EditorGUILayout.IntField(building.Weight);
        EditorGUILayout.EndHorizontal();
    }
}
