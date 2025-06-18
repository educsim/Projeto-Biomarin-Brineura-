using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class TextureCleanupTool : EditorWindow
{
    private string textureFolder = "Assets/Brineura/NewTextures";

    [MenuItem("Tools/Remover Texturas Duplicadas")]
    public static void ShowWindow()
    {
        GetWindow<TextureCleanupTool>("Limpar Texturas Duplicadas");
    }

    private void OnGUI()
    {
        GUILayout.Label("Limpeza de Texturas com Sufixos", EditorStyles.boldLabel);

        textureFolder = EditorGUILayout.TextField("Pasta de Texturas:", textureFolder);

        if (GUILayout.Button("Remover Texturas Duplicadas"))
        {
            RemoveDuplicateTextures();
        }
    }

    private void RemoveDuplicateTextures()
    {
        int removedCount = 0;
        var allTextures = new Dictionary<string, string>(); // baseName ? path
        var guids = AssetDatabase.FindAssets("t:Texture", new[] { textureFolder });

        // Primeiro, coletamos as texturas base (sem sufixo)
        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string fileName = Path.GetFileNameWithoutExtension(path);
            string baseName = GetBaseName(fileName);

            if (fileName == baseName && !allTextures.ContainsKey(baseName))
            {
                allTextures[baseName] = path;
            }
        }

        // Agora deletamos as duplicadas que têm base existente
        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string fileName = Path.GetFileNameWithoutExtension(path);
            string baseName = GetBaseName(fileName);

            if (fileName != baseName && allTextures.ContainsKey(baseName))
            {
                Debug.Log($"Deletando textura '{fileName}' (duplicata de '{baseName}')");
                AssetDatabase.DeleteAsset(path);
                removedCount++;
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Total de texturas redundantes removidas: {removedCount}");
    }

    private string GetBaseName(string name)
    {
        return Regex.Replace(name, @"\s+\d+$", ""); // Remove " 1", " 2", etc.
    }
}
