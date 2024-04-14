using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace SaveLoadSystem.Example
{
    public class Inventory : ISaveLoadObject
    {
        public string ComponentSaveId => "Inventory";

         public List<InventoryItem> Items { get; private set; }= new();
         public int EquippedTool { get; private set; }
         public int EquippedArmor { get; private set; }

        public Inventory(params InventoryItem[] initialItems)
        {
            Items.AddRange(initialItems);
            EquippedTool = 5;
            EquippedArmor = 5;
        }
        
        public SaveLoadData GetSaveLoadData()
        {
            return new InventorySaveLoadData(ComponentSaveId, Items, EquippedTool, EquippedArmor);
        }

        public void RestoreValues(SaveLoadData loadData)
        {
            Items.Clear();

            if (loadData?.Data == null || loadData.Data.Length < 3)
            {
                Debug.LogError($"Can't restore values.");
                return;
            }

            // [0] - (JArray) with items
            // [1] - (int) equippedItem
            // [2] - (int) equippedArmor

            var items = ((JArray)loadData.Data[0]).ToObject<List<InventoryItem>>(); 
            Items.AddRange(items);

            if(int.TryParse(loadData.Data[1].ToString(), out var parsedEquippedItem)) 
                EquippedTool = parsedEquippedItem;

            if (int.TryParse(loadData.Data[2].ToString(), out var parsedEquippedArmor)) 
                EquippedArmor = parsedEquippedArmor;
        }
    }

    [Serializable]
    public class InventoryItem
    {
        [field: SerializeField] public int Id { get; set; }
        [field: SerializeField] public string Title { get; set; }
        [field: SerializeField] public string Description { get; set; }
    }
}