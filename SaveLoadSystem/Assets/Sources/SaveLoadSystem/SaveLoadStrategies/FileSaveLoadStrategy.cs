using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace SaveLoadSystem
{
    public class FileSaveLoadStrategy : ISaveLoadStrategy
    {
        /// <summary>
        /// Local save folder name.
        /// </summary>
        private const string SaveFolderName = "Saves";

        /// <summary>
        /// Name of save file. It would be better to load this from config.
        /// </summary>
        private const string SaveFileName = "GameSaveFile.json";

        /// <summary>
        /// Folder where save file is stored.
        /// </summary>
        private static string SaveDataFolder => Path.Combine(Application.persistentDataPath, SaveFolderName);

        /// <summary>
        /// Absolute path to save file.
        /// </summary>
        private static string SaveFilePath => Path.Combine(SaveDataFolder, SaveFileName);

        public void Save(IEnumerable<ISaveLoadObject> objectsToSave)
        {
            try
            {
                var serializedData = objectsToSave.Select(@object => @object.GetSaveLoadData()).ToList();

                if (!Directory.Exists(SaveDataFolder))
                    Directory.CreateDirectory(SaveDataFolder);

                var saveFile = new SaveFile(serializedData);
                var serializedSaveFile = JsonConvert.SerializeObject(saveFile);

                //todo: make async
                File.WriteAllText(SaveFilePath, serializedSaveFile);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        public SaveLoadData[] Load()
        {
            if (!File.Exists(SaveFilePath))
            {
                Debug.LogError($"Can't load save file. File {SaveFilePath} is doesn't exist.");
                return null;
            }

            try
            {
                var serializedFile = File.ReadAllText(SaveFilePath);
                if (string.IsNullOrEmpty(serializedFile))
                {
                    Debug.LogError($"Loaded file {SaveFilePath} is empty.");
                    return null;
                }

                Debug.Log($"Save to {SaveFilePath}");
                return JsonConvert.DeserializeObject<SaveFile>(serializedFile).Data.ToArray();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}