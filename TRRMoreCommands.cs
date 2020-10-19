using System;
using System.Collections.Generic;
using System.Diagnostics;
using RustExtended;
using UnityEngine;
using Oxide.Core;
using Oxide.Core.Plugins;
using System.Text;
using System.Linq;
using System.Data;
using Oxide.Core.Libraries;
using Newtonsoft.Json;
//+79603692516:Cto_7163(Кристинка Карпина | https://vk.com/victoriy5632) -- cfe84ca6e948212cd4f3d885f759ff57a1faac0b3c1c78d9f1472599e95b4653c92dc2804db051895d4f2

namespace Oxide.Plugins
{
    [Info("TRRMoreCommands", "Mixxe73", "1.2.1")]
    class TRRMoreCommands : RustLegacyPlugin
    {
        static JsonSerializerSettings jsonsettings;
        
        string chatName = "TRR";
    
        #region  [VARS] COLORS
        const string errorColor = "[COLOR #FE2E2E]";
        const string selectColor = "[COLOR #FFA500]";
        const string mainColor = "[COLOR #FFFFFF]";
        const string doubleColor = "[COLOR #C8FE2E]";
        #endregion

        #region [VARS] VK
        bool CommandReport = true;
        string ReportName = "Жалобы";
        int ReportConversationId = 15;
        string ProfileToken = "cfe84ca6e948212cd4f3d885f759ff57a1faac0b3c1c78d9f1472599e95b4653c92dc2804db051895d4f2";
        #endregion

        #region [VARS] ADMINS
        bool CommandAdminsView = true;
        string AdminName = "Администраторы";
        string[] admins = { "Mixxe73", "Freezak"};
        #endregion

        #region Message
        /* Список всех сообщений */
        static Dictionary<string, string> messages = new Dictionary<string, string>()
        {
            /* Errors */
            { "bad_params",         errorColor + "Неправильные параметры команды! /games - список команд" },
            { "no_turn_command",    errorColor + "Данная команда отключена параметрами сервера!" },
            { "report_netuser",     errorColor + "Вы не можете отправить жалобу на самого себя" },
            { "report_badsearch",   errorColor + "Игрок под именем" + mainColor+ "{0}" + errorColor + "не найден!" },

            /* Message */
            { "player_connected",     "{0} Подключился к серверу({1})" },
            { "player_disconnected",  "{0} Покинул сервер" },

        };
        #endregion

        #region Admins
        [ChatCommand("admins")]
        void CMD_admins(NetUser netUser, string command, string[] args)
        {
            if(CommandAdminsView)
            {
            foreach (string admin in admins)
            {
                rust.SendChatMessage(netUser, AdminName, "Администратор " + admin + ((Helper.GetPlayerClient(admin) != null) ? $"{doubleColor} (В сети)" : $"{errorColor} (Не в сети)")) ;
            }

            }
            else
            {
                rust.SendChatMessage(netUser, chatName, messages["no_turn_command"]);
                return;
            }
        }
        #endregion

        #region  report

        [ChatCommand("report")]
        void cmdReport(NetUser netUser, string command, string[] args)
        {
            if(CommandReport)
            {
            if (args.Length >= 2)
            {
                PlayerClient target = Helper.GetPlayerClient(args[0]);
                if (target == null)
                {
                    rust.SendChatMessage(netUser, ReportName, string.Format(messages["report_badsearch"], args[0]));
                    return;
                }

               RustExtended.UserData userData = Users.GetBySteamID(netUser.playerClient.userID);
                if (args[0] == userData.Username)
                {
                    rust.SendChatMessage(netUser, ReportName, messages["report_netuser"]);
                    return;
                }
                
                NetUser targetUser = target.netUser;
                
                string text = "";
                for (int i = 1; i < args.Length; i++)
                {
                    if (i != 1) text += " ";
                    text += args[i];
                }

                string report_text = $"Игрок {netUser.displayName} [{netUser.userID}] отправил жалобу на игрока {targetUser.displayName} [{target.userID}]{Environment.NewLine}Причина: {text}{Environment.NewLine}Сервер:{server.hostname}";
                SendPostRequest(netUser,report_text);
                Helper.LogChat(report_text, true);
                rust.SendChatMessage(netUser, ReportName, $"{doubleColor}Вы отправили жалобу на игрока {targetUser.displayName} Причина: {mainColor}{text}");
            }
            else
            {
                rust.Notice(netUser, "Используйте /report \"Никнейм\" \"Причина\"");
            }
            }
            else
            {
                rust.SendChatMessage(netUser, chatName, messages["no_turn_command"]);
            }
        }

        void SendPostRequest(NetUser netUser,string text)
        {
            int number_1 = Core.Random.Range(0, 22);
            int number_2 = Core.Random.Range(0, 8);
            int Random_ID = number_1 + number_2;
            string msg = "message=" + text + "&chat_id=" + ReportConversationId + "&random_id=" + Random_ID + "&access_token=" + ProfileToken + "&v=" + "5.124";
            webrequest.EnqueuePost($"https://api.vk.com/method/messages.send?", msg, (code, response) => GetCallback(code, response), this);
        }

        #endregion


        void GetCallback(int code, string response)
        {
            if (response == null || code != 200)
            {
				Helper.LogError(response, true);
                return;
            }
            Helper.LogChat(response, true);
        }





    }
}