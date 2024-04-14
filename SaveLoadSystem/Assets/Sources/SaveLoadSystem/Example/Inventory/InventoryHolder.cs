using System.Collections.Generic;
using UnityEngine;

namespace SaveLoadSystem.Example
{
    public class InventoryHolder : MonoBehaviour
    {
        [field: SerializeField] public Inventory Inventory { get; private set; }

        public List<InventoryItem> LoadedItems;
        public int LoadedEquippedTool;
        public int LoadedEquippedArmor;

        private void Awake()
        {
            var items = new List<InventoryItem>();

            for (int i = 0; i < 10; i++)
            {
                items.Add(new InventoryItem
                {
                    Id = i,
                    Title = $"Item {i} title",
                    Description = $"Item {i} description",
                });
            }

            Inventory = new Inventory(items.ToArray());
        }

        public void UpdateItems()
        {
            LoadedItems = Inventory.Items;
            LoadedEquippedTool = Inventory.EquippedTool;
            LoadedEquippedArmor = Inventory.EquippedArmor;
        }
    }
}