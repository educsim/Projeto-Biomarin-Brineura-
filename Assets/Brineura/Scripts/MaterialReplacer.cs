#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.Text.RegularExpressions;

public class MaterialReplacer : MonoBehaviour
{
    [Tooltip("Pasta onde estão os materiais (incluindo os com e sem sufixo)")]
    public string materialFolder = "Assets/SharedMaterials";

    [ContextMenu(" Substituir Materiais com Sufixo")]
    public void ReplaceDuplicatedMaterials()
    {
        var renderers = GetComponentsInChildren<Renderer>(true);
        var allMaterials = LoadAllMaterialsFromFolder();

        int replacements = 0;

        foreach (var renderer in renderers)
        {
            var materials = renderer.sharedMaterials;
            bool changed = false;

            for (int i = 0; i < materials.Length; i++)
            {
                var currentMat = materials[i];
                if (currentMat == null) continue;

                string currentName = currentMat.name;
                string baseName = GetBaseName(currentName);

                if (baseName != currentName && allMaterials.TryGetValue(baseName, out var originalMat))
                {
                    Debug.Log($"Substituindo '{currentName}' por '{baseName}' no objeto '{renderer.gameObject.name}'");
                    materials[i] = originalMat;
                    replacements++;
                    changed = true;
                }
            }

            if (changed)
            {
                renderer.sharedMaterials = materials;

                EditorUtility.SetDirty(renderer);

            }
        }

        Debug.Log($" Total de materiais substituídos: {replacements}");
    }

    private Dictionary<string, Material> LoadAllMaterialsFromFolder()
    {
        var map = new Dictionary<string, Material>();
        var guids = AssetDatabase.FindAssets("t:Material", new[] { materialFolder });

        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat != null && !map.ContainsKey(mat.name))
            {
                map[mat.name] = mat;
            }
        }

        return map;
    }

    private string GetBaseName(string name)
    {
        return Regex.Replace(name, @"\s+\d+$", ""); // Remove " 1", " 2", etc.
    }
}
#endif