using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class MaterialCleanupTool : EditorWindow
{
    private string materialFolder = "Assets/Brineura/Materials";

    [MenuItem("Tools/Remover Materiais Duplicados")]
    public static void ShowWindow()
    {
        GetWindow<MaterialCleanupTool>("Limpar Materiais Duplicados");
    }

    private void OnGUI()
    {
        GUILayout.Label("Limpeza de Materiais com Sufixos", EditorStyles.boldLabel);

        materialFolder = EditorGUILayout.TextField("Pasta de Materiais:", materialFolder);

        if (GUILayout.Button("Remover Materiais Duplicados"))
        {
            RemoveDuplicateMaterials();
        }
    }

    private void RemoveDuplicateMaterials()
    {
        int removedCount = 0;
        var allMaterials = new Dictionary<string, string>(); // baseName ? path
        var guids = AssetDatabase.FindAssets("t:Material", new[] { materialFolder });

        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string fileName = Path.GetFileNameWithoutExtension(path);
            string baseName = GetBaseName(fileName);

            if (fileName == baseName)
            {
                if (!allMaterials.ContainsKey(baseName))
                    allMaterials[baseName] = path;
            }
        }

        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string fileName = Path.GetFileNameWithoutExtension(path);
            string baseName = GetBaseName(fileName);

            if (fileName != baseName && allMaterials.ContainsKey(baseName))
            {
                Debug.Log($"Deletando '{fileName}' (duplicata de '{baseName}')");
                AssetDatabase.DeleteAsset(path);
                removedCount++;
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Total de materiais redundantes removidos: {removedCount}");
    }

    private string GetBaseName(string name)
    {
        return Regex.Replace(name, @"\s+\d+$", ""); // Remove " 1", " 2", etc.
    }
}
