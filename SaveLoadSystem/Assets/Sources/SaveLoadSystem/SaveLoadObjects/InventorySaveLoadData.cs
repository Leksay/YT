using System.Collections.Generic;
using SaveLoadSystem.Example;

namespace SaveLoadSystem
{
    /// <summary>
    /// Custom save load data for inventory. Appends with equipped item and equipped tool.
    /// </summary>
    public class InventorySaveLoadData : SaveLoadData
    {
        public int EquippedItem { get; private set; }
        public int EquippedArmor { get; private set; }

        public InventorySaveLoadData(string id, List<InventoryItem> data, int equippedItem, int equippedArmor) : base(id, new object[]{data, equippedItem, equippedArmor})
        {
            EquippedItem = equippedItem;
            EquippedArmor = equippedArmor;
        }
    }
}