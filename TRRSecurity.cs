using UnityEngine;
using Oxide.Core;
using Oxide.Core.Plugins;
using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Oxide.Core.Libraries;
using Newtonsoft.Json;

namespace Oxide.Plugins
{
	[Info("TRRSecurity", "Mixxe73", "1.0.0")]
	[Description("Разрешение на вход только с определенного IP адресса!")]
	
	class TRRSecurity : RustLegacyPlugin
	{
		int ReportConversationId = 17;
		string ProfileToken = "6a3a1d0eba150b1077a6e0ad4c97a8a24d132b0ce6727ab770c7ef52150b4e23f9295e0112e4b276afa84";

		void execCMD(string Command)
		{
			rust.RunServerCommand(Command);
		}

		void OnPlayerConnected(NetUser netUser)
		{
			string Name = netUser.displayName;
			string Ip = netUser.networkPlayer.externalIP;
			if (Name == "Mixxe73" & Ip != "1227.0.0.1")
            {
				//SendReply(netUser, "[COLOR Red]Вход отклюнен! За последующие попытки будет произведен бан");
				execCMD("serv.kick " + netUser.userID + " " + Ip);
				string text = $"[TRRSecurity]Неудачная попытка входа на аккаунт сотрудника <{Name}> [{Ip}]";
				SendPostRequest(text);
			}
			else
            {
				Debug.Log("Администратор " + Name + " с " + Ip + " успешно подключился");
			}
		}

		void SendPostRequest(string text)
		{
			int number_1 = Core.Random.Range(0, 22);
			int number_2 = Core.Random.Range(0, 8);
			int Random_ID = number_1 + number_2;
			string msg = "message=" + text + "&chat_id=" + ReportConversationId + "&random_id=" + Random_ID + "&access_token=" + ProfileToken + "&v=" + "5.124";
			webrequest.EnqueuePost($"https://api.vk.com/method/messages.send?", msg, (code, response) => GetCallback(code, response), this);
		}

		void GetCallback(int code, string response)
		{
			if (response == null || code != 200)
			{
				//Debug.Log(response);
				return;
			}
			//Debug.Log(response);
		}
	}
}