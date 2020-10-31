using System;
using UnityEngine;
using Oxide.Core;
using System.Collections.Generic;
using RustExtended;
using Oxide.Core.Libraries;

namespace Oxide.Plugins
{
    [Info("TRRPOS", "Mixxe73", "1.0.0")]
    class TRRPOS : RustLegacyPlugin
    {
		public string chatName = "TRR";

		[ChatCommand("pp")] //команда
		void cmdTRRPOS(NetUser netuser, string command, string[] args)
		{
            if (!netuser.CanAdmin()) //Если не админ
            {
                rust.SendChatMessage(netuser, chatName, "У вас недостаточно прав на выполнение команды!"); //Вывод в чат если не админ
                return;
            }
            else //Если все же админ
            {
            var pos = netuser.playerClient.lastKnownPosition; //Определение
            rust.SendChatMessage(netuser, chatName, "Позиция успешно записана!"); //Вывод в чат
            Debug.Log($"[{netuser.playerClient.userName}] " + "NewCoords - " + pos.ToString());
            }
        }   
	}
}