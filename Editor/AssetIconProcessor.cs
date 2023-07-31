using System;
using System.Linq;

using UnityEditor;

using UnityEngine;

namespace AssetIcons.Editor
{
    [InitializeOnLoad]
    public class AssetIconProcessor
    {
        static AssetIconProcessor()
        {
            AssemblyReloadEvents.afterAssemblyReload += AssemblyReloadEvents_afterAssemblyReload;
        }

        private static void AssemblyReloadEvents_afterAssemblyReload()
        {
            ProcessProject();
        }

        private static void ProcessProject()
        {
            var scripts = MonoImporter.GetAllRuntimeMonoScripts();
            foreach (var script in scripts)
            {
                var assetIcon = GetAssetIconAttribute(script, out var type);
                if (assetIcon == null)
                {
                    continue;
                }

                if (!Process(script, assetIcon))
                {
                    Debug.LogWarning($"{type.Name} - Failed to assign icon ({assetIcon.UnityIcon})");
                }
            }
        }

        public static AssetIconAttribute GetAssetIconAttribute(MonoScript script, out Type type)
        {
            type = script.GetClass();
            if (type == null)
            {
                return null;
            }

            var attributes = type.GetCustomAttributesIncludingBaseInterfaces<AssetIconAttribute>();
            if (!attributes.Any())
            {
                return null;
            }

            return attributes.FirstOrDefault() as AssetIconAttribute;
        }

        public static bool Process(MonoScript script, AssetIconAttribute assetIcon)
        {
            if (!string.IsNullOrWhiteSpace(assetIcon.UnityIcon))
            {
                var path = AssetDatabase.GetAssetPath(script.GetInstanceID());
                var monoImporter = AssetImporter.GetAtPath(path) as MonoImporter;
                //This is can be prone to misspellings/not being found, throwing a console exception you can't capture in a try/catch
                //Perhaps it's better to search directly for the image using asset bundles
                var iconContent = EditorGUIUtility.IconContent(assetIcon.UnityIcon);
                if (iconContent != null)
                {
                    var icon = (Texture2D)iconContent.image;
                    if (icon != null)
                    {
                        if (monoImporter.GetIcon() != icon)
                        {
                            Debug.Log($"{script.name} - Set icon '{assetIcon.UnityIcon}'");
                            monoImporter.SetIcon(icon);
                            monoImporter.SaveAndReimport();
                        }

                        return true;
                    }
                }
            }

            return false;
        }
    }
}