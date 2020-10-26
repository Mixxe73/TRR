using System;
using UnityEngine;
using Oxide.Core;
using System.Collections.Generic;
using RustExtended;
using Oxide.Core.Libraries;

namespace Oxide.Plugins
{
    [Info("TRRMultiplicationAmmo", "Mixxe73", "1.2.0")]
    class TRRMultiplicationAmmo : RustLegacyPlugin
    {
		private void OnItemCraft(CraftingInventory inventory, BlueprintDataBlock blueprint, int amount, ulong startTime)
        {

            int[] Improved_ranks = new int[] { 4, 3 }; //Ранги с рейтами X4
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
	}
}