using System;
using UnityEngine;
using Oxide.Core;
using System.Collections.Generic;
using RustExtended;
using Oxide.Core.Libraries;

namespace Oxide.Plugins
{
    [Info("TRRMultiplicationAmmo", "Mixxe73", "1.0.0")]
    class TRRMultiplicationAmmo : RustLegacyPlugin
    {
		private void OnItemCraft(CraftingInventory inventory, BlueprintDataBlock blueprint, int amount, ulong startTime)
        {
            var netUser = inventory.GetComponent<Character>().netUser;
            if (blueprint.resultItem.name == "556 Ammo" || blueprint.resultItem.name == "9mm Ammo" || blueprint.resultItem.name == "Shotgun Shells")
            {
                int NewCount = amount + amount;
                inventory.AddItemAmount(DatablockDictionary.GetByName(blueprint.resultItem.name), NewCount);
            }
            else
            {
                return;
            }           
        } 
	}
}