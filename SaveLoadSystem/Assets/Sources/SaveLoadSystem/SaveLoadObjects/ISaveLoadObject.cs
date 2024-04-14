namespace SaveLoadSystem
{
    /// <summary>
    /// Interface for an object that needs to be saved.
    /// </summary>
    public interface ISaveLoadObject
    {
        /// <summary>
        /// Id to identify object.
        /// </summary>
        public string ComponentSaveId { get; }
        
        /// <summary>
        /// Get data to save for this object.
        /// </summary>
        /// <returns>Data to save.</returns>
        public SaveLoadData GetSaveLoadData();

        /// <summary>
        /// Restore object values from saved data.
        /// </summary>
        /// <param name="loadData">Data for restoring values.</param>
        public void RestoreValues(SaveLoadData loadData);
    }
}