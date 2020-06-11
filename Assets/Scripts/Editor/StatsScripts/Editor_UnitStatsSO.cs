using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Types;
using BehaviorDesigner.Runtime;

[CustomEditor(typeof(UnitsStatsSO))]
public class Editor_UnitStatsSO : Editor
{
    UnitsStatsSO unit;

    SerializedProperty m_attackSound, m_deathSound, m_takeDamageSound, m_hittedSound, m_liftSound, m_dropSound;

    private void OnEnable()
    {
        unit = (UnitsStatsSO)target;

        m_attackSound = serializedObject.FindProperty("AttackSound");
        m_deathSound = serializedObject.FindProperty("DeathSound");
        m_takeDamageSound = serializedObject.FindProperty("TakeDamageSound");
        m_hittedSound = serializedObject.FindProperty("HittedSound");
        m_liftSound = serializedObject.FindProperty("LiftSound");
        m_dropSound = serializedObject.FindProperty("DropSound");
    }
    private void OnDisable()
    {
        EditorUtility.SetDirty(unit);
        AssetDatabase.SaveAssets();
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        //UnitsStatsSO unit = target as UnitsStatsSO;

        GUILayout.Label("GENERAL STATS");
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Unit Type");
        unit.UnitType = (UnitType)EditorGUILayout.EnumPopup(unit.UnitType);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Unit Icon");
        unit.Icon = EditorGUILayout.ObjectField(unit.Icon, typeof(Sprite)) as Sprite;
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Name");
        unit.Name = EditorGUILayout.TextField(unit.Name);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Description");
        unit.Description = EditorGUILayout.TextArea(unit.Description);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Default Description");
        unit.DefaultDescription = EditorGUILayout.TextArea(unit.DefaultDescription);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Unit Prefab");
        unit.VisualPrefab = (GameObject)EditorGUILayout.ObjectField(unit.VisualPrefab, typeof(GameObject), false);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        if (unit.VisualPrefab == null)
        {
            EditorGUILayout.HelpBox("This entity need a prefab", MessageType.Warning);
        }
        else
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Troop Quantity");
            unit.UnitQuantity = EditorGUILayout.IntSlider(unit.UnitQuantity, 1, 4);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }


        EditorGUILayout.Space();
        EditorGUILayout.Space();
        GUILayout.Label("DEFENSIVE STATS");
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Can Take Damage");
        unit.CanTakeDamage = EditorGUILayout.Toggle(unit.CanTakeDamage);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        if (unit.CanTakeDamage)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Health");
            unit.MaxHp = EditorGUILayout.IntSlider(unit.MaxHp, 0, 999);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Armor");
            unit.Armor = EditorGUILayout.IntSlider(unit.Armor, 0, 999);
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
        unit.CanAttack = EditorGUILayout.Toggle(unit.CanAttack);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        if (unit.CanAttack)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Attack Type");
            unit.AttackType = (AttackType)EditorGUILayout.EnumPopup(unit.AttackType);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Damage");
            unit.Damage = EditorGUILayout.IntSlider(unit.Damage, 0, 999);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Attack Range");
            unit.AttackRange = EditorGUILayout.IntField(unit.AttackRange);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            if (unit.AttackType == AttackType.MELEE)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Engage Range");
                unit.EngageRange = EditorGUILayout.IntField(unit.EngageRange);
                EditorGUILayout.EndHorizontal();
            }
            else if (unit.AttackType == AttackType.RANGED)
            {
                unit.EngageRange = unit.AttackRange;

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Projectile");
                unit.Projectile = (GameObject)EditorGUILayout.ObjectField(unit.Projectile, typeof(GameObject), false);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();

                if (unit.Projectile == null)
                {
                    EditorGUILayout.HelpBox("This entity need a projectile", MessageType.Warning);
                }
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Attack Speed");
            unit.AttackSpeed = EditorGUILayout.Slider(unit.AttackSpeed, 0, 999);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }
        EditorGUILayout.Space();


        EditorGUILayout.Space();
        EditorGUILayout.Space();
        GUILayout.Label("OTHER STATS");
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Mov Speed");
        unit.UnitSpeed = EditorGUILayout.Slider(unit.UnitSpeed, 0, 100);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Carry Capacity");
        unit.CarryCapacity = EditorGUILayout.IntSlider(unit.CarryCapacity, 0, 100);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Respawn Time");
        unit.RespawnTime = EditorGUILayout.FloatField(unit.RespawnTime);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        GUILayout.Label("Cost");
        EditorGUILayout.BeginHorizontal();
        unit.Cost = EditorGUILayout.IntField(unit.Cost);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        GUILayout.Label("SOUNDS EFFECTS");
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(m_attackSound, new GUIContent ("Attack Sound"), true);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(m_deathSound, new GUIContent ("Death Sound"), true);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(m_takeDamageSound, new GUIContent ("Take Damage Sound"), true);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(m_hittedSound, new GUIContent ("Hitted Sound"), true);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(m_liftSound, new GUIContent ("Lift Sound"), true);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(m_dropSound, new GUIContent ("Drop Sound"), true);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        

        serializedObject.ApplyModifiedProperties();
    }
}
