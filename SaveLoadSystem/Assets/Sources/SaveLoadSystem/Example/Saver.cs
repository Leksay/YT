using EasyButtons;
using UnityEngine;

namespace SaveLoadSystem.Example
{
    /// <summary>
    /// Helper monobehaviour for saving from unity inspector.
    /// </summary>
    public class Saver : MonoBehaviour
    {
        [SerializeField] private InventoryHolder _inventoryHolder;

        private SaveLoadSystem _saveLoadSystem;

        private void Start()
        {
            _saveLoadSystem ??= new();
            _saveLoadSystem.AddToSaveLoad(_inventoryHolder.Inventory);
        }

        [Button("Save to file")]
        private void Save()
        {
            _saveLoadSystem.SaveGame(SaveType.File);
        }

        [Button("Load from file")]
        private void Load()
        {
            _saveLoadSystem.LoadGame(SaveType.File);

            _inventoryHolder.UpdateItems();
        }
    }
}