using System;
using System.Collections.Generic;
using UnityEngine;

namespace SaveLoadSystem
{
    public class SaveLoadSystem
    {
        private readonly FileSaveLoadStrategy _fileSaveLoadStrategy = new();

        private Dictionary<string, ISaveLoadObject> _componentsIdToSaveObject = new();

        public void AddToSaveLoad(ISaveLoadObject saveLoadObject) => _componentsIdToSaveObject[saveLoadObject.ComponentSaveId] = saveLoadObject;

        /// <summary>
        /// Save game.
        /// </summary>
        /// <param name="saveType">Type of save strategy.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void SaveGame(SaveType saveType)
        {
            var strategy = saveType switch
            {
                SaveType.File => _fileSaveLoadStrategy,
                _ => throw new NotImplementedException()
            };

            SaveAll(strategy);
        }

        /// <summary>
        /// Load game data.
        /// </summary>
        /// <param name="saveType">Type of load strategy.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void LoadGame(SaveType saveType)
        {
            var strategy = saveType switch
            {
                SaveType.File => _fileSaveLoadStrategy,
                _ => throw new NotImplementedException()
            };

            Load(strategy);
        }

        /// <summary>
        /// Save all registered ISaveLoadObjects.
        /// </summary>
        /// <param name="strategy"></param>
        private void SaveAll(ISaveLoadStrategy strategy) => Save(strategy, _componentsIdToSaveObject.Values);

        private void Save(ISaveLoadStrategy strategy, IEnumerable<ISaveLoadObject> data) => strategy.Save(data);

        private void Load(ISaveLoadStrategy strategy)
        {
            var loadedData = strategy.Load();

            foreach (var data in loadedData)
            {
                var objectId = data.Id;
                if (!_componentsIdToSaveObject.ContainsKey(objectId))
                {
                    Debug.LogError($"Can't restore data for object with id {objectId}");
                    continue;
                }

                _componentsIdToSaveObject[objectId].RestoreValues(data);
            }
        }
    }
}