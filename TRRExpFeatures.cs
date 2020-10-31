using System;
using UnityEngine;
using Oxide.Core;
using System.Collections.Generic;
using RustExtended;
using Oxide.Core.Libraries;
//плагин позволяет крафтить по много патронов, добывать дерево по другому кол-ву
namespace Oxide.Plugins
{
    [Info("TRR Experemental Features", "Mixxe73", "1.2.5")]
    class TRRExpFeatures : RustLegacyPlugin
    {
        int[] Improved_ranks = new int[] { 4, 5 }; //Ранги с рейтами X4
            int STWood = 12; //Сколько выдавать дерева при добыче статического дерева

            /*
            Текущие ранги: 
            Ранг 5 - Боров (Самый сок)
            Ранг 4 - Мастер
            */

		private void OnItemCraft(CraftingInventory inventory, BlueprintDataBlock blueprint, int amount, ulong startTime)
        {
            var netUser = inventory.GetComponent<Character>().netUser;
            RustExtended.UserData userData = Users.GetBySteamID(netUser.userID);

            if (blueprint.resultItem.name == "556 Ammo" || blueprint.resultItem.name == "9mm Ammo" || blueprint.resultItem.name == "Shotgun Shells")
            {
                if (Array.IndexOf(Improved_ranks, userData.Rank) == 1)
                {
                int NewCount = amount + amount + amount;
                inventory.AddItemAmount(DatablockDictionary.GetByName(blueprint.resultItem.name), NewCount);
                return;
                }
                else
                {
                int NewCount = amount + amount;
                inventory.AddItemAmount(DatablockDictionary.GetByName(blueprint.resultItem.name), NewCount);
                return;
                }
            }
            else
            {
                return;
            }           
        } 

        int OnGather(Inventory inventory, ResourceTarget resourceTarget, object obj, int num)
        {
            if(obj == null)
            {
                var netUser = inventory.GetComponent<Character>().netUser;
                rust.InventoryNotice(netUser, STWood + " x Wood");
                inventory.AddItemAmount(DatablockDictionary.GetByName("Wood"), STWood);
            }
            return num;
        }

	}
}