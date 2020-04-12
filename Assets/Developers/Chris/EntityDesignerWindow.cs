using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EntityDesignerWindow : EditorWindow
{
    Texture2D headerSectionTexture;
    Texture2D unitSectionTexture;
    Texture2D buildgingSectionTexture;

    Color headerSectionColor = new Color(13f / 255f, 32f / 255f, 44f / 255f, 1f);

    Rect headerSection;
    Rect unitSection;
    Rect buildingSection;

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
    }

    void InitTextures()
    {
        headerSectionTexture = new Texture2D(1, 1);
        headerSectionTexture.SetPixel(0, 0, headerSectionColor);
        headerSectionTexture.Apply();
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

        GUI.DrawTexture(headerSection, headerSectionTexture);


    }

    void DrawHeader()
    {

    }

    void DrawUnitSettings()
    {

    }

    void DrawBuildingSettings()
    {

    }

}
