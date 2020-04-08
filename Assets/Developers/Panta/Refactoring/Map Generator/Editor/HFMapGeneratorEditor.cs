using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;

namespace HF.Refactoring
{
    [CustomEditor(typeof(HFMapGenerator))]
    public class HFMapGeneratorEditor : Editor
    {
        HFMapGenerator mapGenerator = null;

        private void OnEnable()
        {
            mapGenerator = (HFMapGenerator)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(10);

            GUI.color = Color.grey;
            if (GUILayout.Button("Generate Map"))
            {
                mapGenerator.GenerateMap();
            }

            EditorGUILayout.BeginHorizontal();
            GUI.color = Color.Lerp(Color.gray, Color.green, 0.4f);
            if (GUILayout.Button("Save Both"))
            {
                SaveGameObject(mapGenerator.gameObject, mapGenerator.StoreMapAtPath);
                SaveGameObject(mapGenerator.MapGrid.gameObject, mapGenerator.StoreTileMapAtPath);
            }

            if (GUILayout.Button("Save Map"))
            {
                SaveGameObject(mapGenerator.gameObject, mapGenerator.StoreMapAtPath);
            }

            if (GUILayout.Button("Save Tile Map"))
            {
                SaveGameObject(mapGenerator.MapGrid.gameObject, mapGenerator.StoreTileMapAtPath);
            }
            EditorGUILayout.EndHorizontal();

            GUI.color = Color.Lerp(Color.gray, Color.red, 0.4f);
            if (GUILayout.Button("Destroy Map Generated"))
            {
                mapGenerator.DestroyPreviousMap();
            }
		}

        public void SaveGameObject(GameObject gameObject, string path)
        {
            string localPath = AssetDatabase.GenerateUniqueAssetPath("Assets/" + path + mapGenerator.name + ".prefab");
            PrefabUtility.SaveAsPrefabAsset(gameObject, localPath);
        }
	}
}
