using System.Collections.Generic;

namespace SaveLoadSystem
{
    /// <summary>
    /// Base interface for save load strategies.
    /// </summary>
    public interface ISaveLoadStrategy
    {
        /// <summary>
        /// Save list of objects.
        /// </summary>
        /// <param name="objectsToSave">Array of objects to be saved.</param>
        public void Save(IEnumerable<ISaveLoadObject> objectsToSave);

        /// <summary>
        /// Load data.
        /// </summary>
        /// <returns>Loaded data.</returns>
        public SaveLoadData[] Load();
    }
}