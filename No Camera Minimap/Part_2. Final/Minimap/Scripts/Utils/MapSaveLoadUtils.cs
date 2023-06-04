using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

public static class MapSaveLoadUtils
{
    #region constants

    public const string SCREENSHOT_FILE_NAME = "map_screen_*.png";

    private const string WINDOW_FILE_NAME = "minimapWindowData.json";

    #endregion

    private static string SaveFolder => Path.Combine(Application.dataPath, "Sources", "Minimap", "EditorSaveData");

    public static MinimapWindowDataModel LoadWindowData()
    {
        string path = Path.Combine(SaveFolder, WINDOW_FILE_NAME);

        if (!File.Exists(path))
            return null;

        string serializedData = File.ReadAllText(path);

        if (string.IsNullOrEmpty(serializedData))
            return null;

        MinimapWindowDataModel data = JsonConvert.DeserializeObject<MinimapWindowDataModel>(serializedData);

        return data;
    }

    public static void SaveWindowData(MinimapWindowDataModel dataModel)
    {
        if(dataModel == null)
            return;

        string serializedObject = JsonConvert.SerializeObject(dataModel, MinimapWindowDataModel.SerializeSettings());

        string path = Path.Combine(SaveFolder, WINDOW_FILE_NAME);
        CreateDirectoryIfNotExists(SaveFolder);
        
        File.WriteAllText(path, serializedObject);

        Debug.LogWarning($"Save window data with path {path}");
    }

    public static void CreateDirectoryIfNotExists(string path)
    {
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
    }

    public static void SaveTexture(Texture2D screen, int i)
    {
        CreateDirectoryIfNotExists(SaveFolder);
        string path = Path.Combine(SaveFolder, SCREENSHOT_FILE_NAME.Replace("*", i.ToString()));

        byte[] textureBytes = screen.EncodeToPNG();
        File.WriteAllBytes(path, textureBytes);

        Debug.LogWarning($"Save texture at path {path}");
    }

    public static Texture2D[] LoadAllMapTextures()
    {
        string path = Path.Combine(SaveFolder, SCREENSHOT_FILE_NAME);
        string relativePath = path.Substring(path.IndexOf("Assets", StringComparison.Ordinal));

        List<Texture2D> textures = new List<Texture2D>();
        int counter = 0;

        string texturePath = relativePath.Replace("*", counter.ToString());

        while (File.Exists(texturePath))
        {
            string localPath = texturePath.Substring(texturePath.IndexOf("Assets", StringComparison.Ordinal));

            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(localPath);

            if(texture)
                textures.Add(texture);

            counter++;
            texturePath = relativePath.Replace("*", counter.ToString());
        }

        return textures.ToArray();
    }

    public static void ClearDirectory(string directory)
    {
        if (Directory.Exists(directory)) 
            Directory.Delete(directory, true);

        CreateDirectoryIfNotExists(directory);
    }
}