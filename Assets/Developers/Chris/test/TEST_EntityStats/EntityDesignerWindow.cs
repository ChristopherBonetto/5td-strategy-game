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

    static UnitsStatsSO unitData;
    public static UnitsStatsSO UnitData { get { return unitData; } }

    static BuildingsStatsSO buildingData;
    public static BuildingsStatsSO BuildingData { get { return buildingData; } }

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
        unitData = (UnitsStatsSO)ScriptableObject.CreateInstance(typeof(UnitsStatsSO));
        buildingData = (BuildingsStatsSO)ScriptableObject.CreateInstance(typeof(BuildingsStatsSO));
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

        GUILayout.Label("PANEL TO CREATE NEW ENTITY");

        GUILayout.EndArea();
    }

    void DrawUnitSettings()
    {
        GUILayout.BeginArea(unitSection);

        GUILayout.Label("Unit");

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
        window.minSize = new Vector2(300, 350);
        window.Show();
    }

    private void OnGUI()
    {
        switch (dataSetting)
        {
            case SettingType.Unit:
                DrawSettings((EntityStatsSO)EntityDesignerWindow.UnitData);
                break;
            case SettingType.Building:
                DrawSettings((EntityStatsSO)EntityDesignerWindow.BuildingData);
                break;
        }
    }

    void DrawSettings(EntityStatsSO entity)
    {
        if (entity is UnitsStatsSO)
        {
            if (DrawUnitSettings((UnitsStatsSO)entity))
            {
                if(GUILayout.Button("Finish and Save", GUILayout.Height(30)))
                {
                    window.Close();
                    SaveData();
                }
            }
        }
        else if (entity is BuildingsStatsSO)
        {
            if (DrawBuildingSettings((BuildingsStatsSO)entity))
            {
                if (GUILayout.Button("Finish and Save", GUILayout.Height(30)))
                {
                    window.Close();
                    SaveData();
                }
            }
        }
    }

    bool DrawUnitSettings(UnitsStatsSO unit)
    {
        bool tempValue = true;

        GUILayout.Label("GENERAL STATS");
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Unit Type");
        unit.UnitType = (UnitType)EditorGUILayout.EnumPopup(unit.UnitType);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Name");
        unit.Name = EditorGUILayout.TextField(unit.Name);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();


        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Prefab");
        unit.Prefab = (GameObject)EditorGUILayout.ObjectField(unit.Prefab, typeof(GameObject), false);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        if (unit.Prefab == null)
        {
            tempValue = false;
            EditorGUILayout.HelpBox("This entity need a prefab", MessageType.Warning);
        }
        else
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Troop Quantity");
            unit.TroopsQuantity = EditorGUILayout.IntSlider(unit.TroopsQuantity, 1, 4);
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
                    tempValue = false;
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
        unit.Cost.ResourceType = (ResourceType)EditorGUILayout.EnumPopup(unit.Cost.ResourceType);
        unit.Cost.ResourceQuantity = EditorGUILayout.IntField(unit.Cost.ResourceQuantity);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        return tempValue;
    }

    bool DrawBuildingSettings(BuildingsStatsSO building)
    {
        bool tempValue = true;

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

        

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Prefab");
        building.Prefab = (GameObject)EditorGUILayout.ObjectField(building.Prefab, typeof(GameObject), false);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        if (building.Prefab == null)
        {
            tempValue = false;
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
                tempValue = false;
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
        building.Cost.ResourceType = (ResourceType)EditorGUILayout.EnumPopup(building.Cost.ResourceType);
        building.Cost.ResourceQuantity = EditorGUILayout.IntField(building.Cost.ResourceQuantity);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        return tempValue;
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