using System;
using System.Collections.Generic;

namespace SaveLoadSystem
{
    /// <summary>
    /// Game save file.
    /// </summary>
    [Serializable]
    public struct SaveFile
    {
        /// <summary>
        /// Save DateTime.
        /// </summary>
        public DateTime SaveTime { get; }

        /// <summary>
        /// List of saved data.
        /// </summary>
        public List<SaveLoadData> Data { get; }

        public SaveFile(List<SaveLoadData> data) : this()
        {
            Data = data;
            SaveTime = DateTime.Now;
        }
    }
}