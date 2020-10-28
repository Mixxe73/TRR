using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Oxide.Core;
using Oxide.Core.Plugins;
using RustExtended;
namespace Oxide.Plugins
{
    [Info("TRRItemsFunctions", "Mixxe73", 3.0)]
    internal class TRRItemsFunctions : RustLegacyPlugin
    {
        private const string chatName = "TRR";
        private const string ItemsGift = "Armor Part 1";

        [HookMethod("OnBeltUse")]
        public object BeltDetector(PlayerInventory inv, IInventoryItem inventoryItem)
        {
            var netUser = inventoryItem.controllable.netUser;
            
            if (inventoryItem.datablock.name != ItemsGift) return null;
            string GiftItems = ItemsGift;
            int GiftId = Core.Random.Range(0, 3); // 0 - 2
            int GiftSteep = Core.Random.Range(0, 2); // 0 - 1
            Inventory inventory = netUser.playerClient.controllable.GetComponent<Inventory>();

            if (GiftId == 0)
            {
                RustExtended.Helper.InventoryItemRemove(inventory,  DatablockDictionary.GetByName(GiftItems), 1);
                inventory.AddItemAmount(DatablockDictionary.GetByName("Leather Pants"), 1);
                inventory.AddItemAmount(DatablockDictionary.GetByName("Sulfur Ore"), 15);
                if(GiftSteep == 0)
                inventory.AddItemAmount(DatablockDictionary.GetByName("Revolver"), 1);
                inventory.AddItemAmount(DatablockDictionary.GetByName("9mm Ammo"), 6);
            }

            if (GiftId == 1)
            {
                RustExtended.Helper.InventoryItemRemove(inventory,  DatablockDictionary.GetByName(GiftItems), 1);
                inventory.AddItemAmount(DatablockDictionary.GetByName("Metal Ore"), 35);
                inventory.AddItemAmount(DatablockDictionary.GetByName("Sulfur"), 75);
                inventory.AddItemAmount(DatablockDictionary.GetByName("Metal Fragments"), 65);
                inventory.AddItemAmount(DatablockDictionary.GetByName("Cloth Helmet"), 1);
                if(GiftSteep == 0)
                inventory.AddItemAmount(DatablockDictionary.GetByName("Hunting Bow"), 1);
                inventory.AddItemAmount(DatablockDictionary.GetByName("Arrow"), 15);
            }

            if (GiftId == 2)
            {
                RustExtended.Helper.InventoryItemRemove(inventory,  DatablockDictionary.GetByName(GiftItems), 1);
                inventory.AddItemAmount(DatablockDictionary.GetByName("Wood Planks"), 65);
                inventory.AddItemAmount(DatablockDictionary.GetByName("Wood"), 125);
                inventory.AddItemAmount(DatablockDictionary.GetByName("Metal Fragments"), 125);
                inventory.AddItemAmount(DatablockDictionary.GetByName("Cloth Pants"), 1);
                if(GiftSteep == 0)
                inventory.AddItemAmount(DatablockDictionary.GetByName("Cloth Vest"), 1);
                inventory.AddItemAmount(DatablockDictionary.GetByName("Hunting Bow"), 1);
                inventory.AddItemAmount(DatablockDictionary.GetByName("Arrow"), 15);
            }

            rust.SendChatMessage(netUser, chatName, "Вы успешно открыли подарок!");
            Helper.LogChat("Пользователь " + netUser.displayName + "открыл большой подарок!["+ "Выпал набор: GiftID - " + GiftId + " ,GiftSteep - "+ GiftSteep + "]", true);

            if(netUser.CanAdmin())
            rust.SendChatMessage(netUser, "Лог", "Выпал набор: GiftID - " + GiftId + " ,GiftSteep - " + GiftSteep);
            return true;
        }
    }
}