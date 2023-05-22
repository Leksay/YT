using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public static class MapSaveLoadUtils
{
    #region constants

    private const string WINDOW_FILE_NAME = "minimapWindowData.json";
    private const string SCREENSHOT_FILE_NAME = "map_screen_*.png";

    #endregion

    private static string SaveFolder => Path.Combine(Application.dataPath, "Sources", "Minimap", "SaveData");

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

    public static void SaveWindowData(Vector3 startPoint, Vector3 endPoint)
    {
        MinimapWindowDataModel dataModel = new MinimapWindowDataModel
        {
            StartPosition = Vector3Model.FromVector3(startPoint),
            EndPosition = Vector3Model.FromVector3(endPoint)
        };

        string serializedObject = JsonConvert.SerializeObject(dataModel);

        string path = Path.Combine(SaveFolder, WINDOW_FILE_NAME);
        CreateDirectoryIfNotExists(SaveFolder);
        
        File.WriteAllText(path, serializedObject);

        Debug.LogWarning($"Save window data with path {path}");
    }

    private static void CreateDirectoryIfNotExists(string path)
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
}