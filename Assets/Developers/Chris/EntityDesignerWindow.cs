using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Types;

public class EntityDesignerWindow : EditorWindow
{
    Texture2D headerSectionTexture;
    Texture2D unitSectionTexture;
    Texture2D buildgingSectionTexture;

    Color headerSectionColor = new Color(13f / 255f, 255f / 255f, 150f / 255f, 1f);
    Color unitSectionColor = new Color(100f / 255f, 150f / 255f, 70f / 255f, 1f);
    Color buildingSectionColor = new Color(255f / 255f, 50f / 255f, 100f / 255f, 1f);

    Rect headerSection;
    Rect unitSection;
    Rect buildingSection;

    static UnitsSO unitData;
    public static UnitsSO UnitData { get { return unitData; } }

    static BuildingsSO buildingData;
    public static BuildingsSO BuildingData { get { return buildingData; } }

    [MenuItem("Window/Entity Designer")]
    static void OpenWindow()
    {
        EntityDesignerWindow window = (EntityDesignerWindow)GetWindow(typeof(EntityDesignerWindow));
        window.minSize = new Vector2(600, 300);
        window.Show();
    }

    void OnEnable()
    {
        InitTextures();
        InitData();
    }

    public static void InitData()
    {
        unitData = (UnitsSO)ScriptableObject.CreateInstance(typeof(UnitsSO));
        buildingData = (BuildingsSO)ScriptableObject.CreateInstance(typeof(BuildingsSO));
    }

    void InitTextures()
    {
        headerSectionTexture = new Texture2D(1, 1);
        headerSectionTexture.SetPixel(0, 0, headerSectionColor);
        headerSectionTexture.Apply();

        unitSectionTexture = new Texture2D(1, 1);
        unitSectionTexture.SetPixel(0, 0, unitSectionColor);
        unitSectionTexture.Apply();

        buildgingSectionTexture = new Texture2D(1, 1);
        buildgingSectionTexture.SetPixel(0, 0, buildingSectionColor);
        buildgingSectionTexture.Apply();
    }

    void OnGUI()
    {
        DrawLayouts();
        DrawHeader();
        DrawUnitSettings();
        DrawBuildingSettings();
    }

    void DrawLayouts()
    {
        headerSection.x = 0;
        headerSection.y = 0;
        headerSection.width = Screen.width;
        headerSection.height = 50;

        unitSection.x = 0;
        unitSection.y = 50;
        unitSection.width = Screen.width / 3f;
        unitSection.height = Screen.height - 50;

        buildingSection.x = Screen.width / 3f;
        buildingSection.y = 50;
        buildingSection.width = Screen.width / 3f;
        buildingSection.height = Screen.height - 50;


        GUI.DrawTexture(headerSection, headerSectionTexture);
        GUI.DrawTexture(unitSection, unitSectionTexture);
        GUI.DrawTexture(buildingSection, buildgingSectionTexture);
    }

    void DrawHeader()
    {
        GUILayout.BeginArea(headerSection);

        GUILayout.Label("Create a new upgrade");

        GUILayout.EndArea();
    }

    void DrawUnitSettings()
    {
        GUILayout.BeginArea(unitSection);

        GUILayout.Label("Unit");

        //EditorGUILayout.BeginHorizontal();
        //GUILayout.Label("Unit Type");
        //unitData.UnitType = (UnitType)EditorGUILayout.EnumPopup(unitData.UnitType);
        //EditorGUILayout.EndHorizontal();


        //EditorGUILayout.BeginHorizontal();
        //GUILayout.Label("Damage Type");
        //unitData.DamageType = (DamageType)EditorGUILayout.EnumPopup(unitData.DamageType);
        //EditorGUILayout.EndHorizontal();

        if(GUILayout.Button("Create!", GUILayout.Height(40)))
        {
            GeneralSettings.OpenWindow(GeneralSettings.SettingType.Unit);
        }

        GUILayout.EndArea();
    }

    void DrawBuildingSettings()
    {
        GUILayout.BeginArea(buildingSection);

        GUILayout.Label("Building");

        if (GUILayout.Button("Create!", GUILayout.Height(40)))
        {
            GeneralSettings.OpenWindow(GeneralSettings.SettingType.Building);
        }

        GUILayout.EndArea();
    }

}

public class GeneralSettings : EditorWindow
{
    public enum SettingType
    {
        Unit,
        Building
    }

    static SettingType dataSetting;
    static GeneralSettings window;

    public static void OpenWindow(SettingType setting)
    {
        dataSetting = setting;
        window = (GeneralSettings)GetWindow(typeof(GeneralSettings));
        window.minSize = new Vector2(250, 200);
        window.Show();
    }

    private void OnGUI()
    {
        switch (dataSetting)
        {
            case SettingType.Unit:
                DrawSettings((EntitySO)EntityDesignerWindow.UnitData);
                break;
            case SettingType.Building:
                DrawSettings((EntitySO)EntityDesignerWindow.BuildingData);
                break;
        }
    }

    void DrawSettings(EntitySO entity)
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Name");
        entity.Name = EditorGUILayout.TextField(entity.Name);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Health");
        entity.MaxHp = EditorGUILayout.IntSlider(entity.MaxHp, 0, 999);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Armor");
        entity.Armor = EditorGUILayout.IntSlider(entity.Armor, 0, 999);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Damage");
        entity.Damage = EditorGUILayout.IntSlider(entity.Damage, 0, 999);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Attack Speed");
        entity.AttackSpeed = EditorGUILayout.Slider(entity.AttackSpeed, 0, 999);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Cost");
        entity.Cost = EditorGUILayout.IntField(entity.Cost);
        EditorGUILayout.EndHorizontal();

        //public Mesh Mesh;

        //public EntitySO[] Upgrades;

        if (entity is UnitsSO)
        {
            if (DrawUnitSettings((UnitsSO)entity))
            {
                if(GUILayout.Button("Finish and Save", GUILayout.Height(30)))
                {
                    window.Close();
                    SaveData();
                }
            }
        }
        else if (entity is BuildingsSO)
        {
            if (DrawBuildingSettings((BuildingsSO)entity))
            {
                if (GUILayout.Button("Finish and Save", GUILayout.Height(30)))
                {
                    window.Close();
                    SaveData();
                }
            }
        }
    }

    bool DrawUnitSettings(UnitsSO unit)
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Attack Type");
        unit.AttackType = (AttackType)EditorGUILayout.EnumPopup(unit.AttackType);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Mov Speed");
        unit.UnitSpeed = EditorGUILayout.Slider(unit.UnitSpeed, 0, 100);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Carry Capacity");
        unit.CarryCapacity = EditorGUILayout.IntSlider(unit.CarryCapacity, 0, 100);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Respawn Time");
        unit.RespawnTime = EditorGUILayout.FloatField(unit.RespawnTime);
        EditorGUILayout.EndHorizontal();

        if (unit.AttackType == AttackType.MELEE)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Attack Range");
            unit.AttackRange = EditorGUILayout.IntField(unit.AttackRange);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Engage Range");
            unit.EngageRange = EditorGUILayout.IntField(unit.EngageRange);
            EditorGUILayout.EndHorizontal();
        }
        else if(unit.AttackType == AttackType.RANGED)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Attack Range");
            unit.AttackRange = EditorGUILayout.IntField(unit.AttackRange);
            EditorGUILayout.EndHorizontal();

            unit.EngageRange = unit.AttackRange;

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Projectile");
            unit.Projectile = (GameObject)EditorGUILayout.ObjectField(unit.Projectile, typeof(GameObject), false);
            EditorGUILayout.EndHorizontal();

            if(unit.Projectile == null)
            {
                EditorGUILayout.HelpBox("This entity need a projectile", MessageType.Warning);
                return false;
            }
        }

        

        return true;
    }

    bool DrawBuildingSettings(BuildingsSO building)
    {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Building Type");
        building.BuildingType = (BuildingType)EditorGUILayout.EnumPopup(building.BuildingType);
        EditorGUILayout.EndHorizontal();

        if(building.BuildingType == BuildingType.CASTLE)
        {

        }
        else if(building.BuildingType == BuildingType.TOWER)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Projectile");
            building.Projectile = (GameObject)EditorGUILayout.ObjectField(building.Projectile, typeof(GameObject), false);
            EditorGUILayout.EndHorizontal();

            if (building.Projectile == null)
            {
                EditorGUILayout.HelpBox("This entity need a projectile", MessageType.Warning);
                return false;
            }
        }

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Weight");
        building.Weight = EditorGUILayout.IntField(building.Weight);
        EditorGUILayout.EndHorizontal();

        return true;
    }

    void SaveData()
    {
        string dataPath = "Assets/Developers/Chris/test/TEST_EntityStats/";

        switch (dataSetting)
        {
            case SettingType.Unit:

                dataPath += "UnitsTest/" + EntityDesignerWindow.UnitData.Name + ".asset";
                AssetDatabase.CreateAsset(EntityDesignerWindow.UnitData, dataPath);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                break;

            case SettingType.Building:

                dataPath += "BuildingsTest/" + EntityDesignerWindow.BuildingData.Name + ".asset";
                AssetDatabase.CreateAsset(EntityDesignerWindow.BuildingData, dataPath);

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                break;
        }
    }
}