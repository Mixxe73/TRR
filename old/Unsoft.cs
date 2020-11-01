using System;
using System.Collections.Generic;
using System.Linq;
using RustExtended;
using UnityEngine;
using Newtonsoft.Json;
using Oxide.Core.Libraries;


namespace Oxide.Plugins
{
    [Info("UnSOFT", "Mixxe73(Дефектный) & Romanchik34", "1.2.2")]
    [Description("Данный плагин позволяет взаимодействовать с форумом <UNIVERSESOFT.SPACE> ")]
    class Unsoft : RustLegacyPlugin
    {
        #region config
        private string dataPath = "Unsoft_Data";
        private string chatname = "UniverseSOFT";
        private string unicon = "ʊs"; //НЕ ТРОГАТЬ, Это своеобразный копирайт плагина 

        private const string color_main = "[COLOR #FFFFFF]";
        private const string color_double = "[COLOR #C8FE2E]";
        private const string color_error = "[color red]";
        private bool AuthGR = true; // Выдавать ли предметы за регистрацию на форуме
        private bool Blocker = true; // ВКЛ/ВЫКЛ Команду </bl> (Значение "true" - Включает) (Значение "false" - Выключает)
        private bool BlockerNotice = true; // ВКЛ/ВЫКЛ отображение на экране об блокировке (Значение "true" - Включает) (Значение "false" - Выключает)
        private bool BlockerNoticeForum = true; // ВКЛ/ВЫКЛ отоправку данных об заблокированном пользователе на форум (Значение "true" - Включает) (Значение "false" - Выключает)
        private int BlockerMethod = 3; // Метод блокировки пользователя (Значение "1" - serv.ban) (Значение "2" - serv.block) (Значение "3" - serv.ban & serv.block)
        private int BlockerRank = 0;  // Тот кому разрешена команда блокировки и отправки на форум!

        private string apiPath = "https://universesoft.space/api/";

        private string apikey = "bGAdekzwEUAMDK1vrBr5TPREEn_lk7NA"; // Ключ через который будут выполняться действия
        private string apiuser = "2"; // От имени какого пользотвателя будут выполняться запросы! (Значение "1" - от имени Дефектный) (Значение "2" - от имени ROOT).
        private string thread_id = "28"; // Тема куда будут публиковаться записи после BLOCKER'a


        /* Что выдавать после регистрации */
        static object[][] GiveResorseRegistr = new object[][]
        {
            //            Тип ячейки инвентаря          Название предмета       Количество
            //           [Belt, Armor, Default]                 -                 1-250

            new object[]{ Inventory.Slot.Kind.Default,     "P250",                1   },
            new object[]{ Inventory.Slot.Kind.Default,     "9mm Ammo",            100  },
            new object[]{ Inventory.Slot.Kind.Default,     "Small Medkit",        2   }
        };


        #endregion
        #region Data (Save & Load)
        Data data;
        class Data
        {
            public List<ForumData> dataBase = new List<ForumData>();
        }
        class ForumData
        {
            public string Username;
            public string EMail;
            public ulong OwnerID;
            public bool BonusGived;
        }
        private ForumData GetPlayerForumData(object parameter)
        {
            if (parameter is ulong)
            {
                foreach (ForumData data in data.dataBase)
                {
                    if (data.OwnerID == (ulong)parameter)
                        return data;
                }
            }
            else if (parameter is string && ((string)parameter).Contains("@"))
            {
                foreach (ForumData data in data.dataBase)
                {
                    if (data.EMail == (string)parameter)
                        return data;
                }
            }
            else if (parameter is string)
            {
                foreach (ForumData data in data.dataBase)
                {
                    if (data.Username == (string)parameter)
                        return data;
                }
            }
            return null;
        }
        void OnServerSave() => Core.Interface.GetMod().DataFileSystem.WriteObject(dataPath, data);
        void Loaded() => data = Core.Interface.GetMod().DataFileSystem.ReadObject<Data>(dataPath);
        #endregion

        void Init()
        {
            Helper.Log("============================\nPowered by ʊs (UNIVERSESOFT.SPACE) \nНе забудьте оставить отзыв в теме с данным плагином!\n========================================================" , true);
        }

        void CMD(string Command)
        {
            rust.RunServerCommand(Command);
        }

        void BlockerInformer(string unicon, string playerNick, string playerBLReason,string blockerNickName, PlayerClient playerClient, NetUser netuser)
        {
            string textBAN = playerNick + " Забанен!";
            
            if (BlockerNotice == true) {rust.InventoryNotice(netuser, textBAN);}
            
            
            string BNF = BlockerNoticeForum ? "Да" : "Нет";

            Helper.LogError("[UnSOFT][BLOCKER] NICKNAME - \"" + playerNick + "\" blocked because pidoras.", true);
            Helper.LogError("[UnSOFT][BLOCKER] STEMID -\"" + playerClient.userID + "\" ", true);
            Helper.LogError("[UnSOFT][BLOCKER] REASON -\"" + playerBLReason + "\" ", true);
            Helper.LogError("[UnSOFT][BLOCKER] Posted on forum -\"" + BNF + "\" ", true);

            var headers = new Dictionary<string, string>
                {
                    { "XF-Api-Key", apikey },
                    { "XF-Api-User", apiuser }
                };

            if (BlockerNoticeForum == true || thread_id != null)
            {
                //string msgforsend = $"[COLOR=rgb(226, 80, 65)]1.[/COLOR]Ник нарушителя: [COLOR=rgb(26, 188, 156)]{playerNick}[/COLOR] \n [COLOR=rgb(226, 80, 65)]2.[/COLOR]STEAMID нарушителя: [COLOR=rgb(26, 188, 156)]{playerClient.userID}[/COLOR]\n [COLOR=rgb(226, 80, 65)]3.[/COLOR]IP ADRESS нарушителя: [COLOR=rgb(26, 188, 156)]{playerClient.netPlayer.externalIP}[/COLOR] \n [COLOR=rgb(226, 80, 65)]4.[/COLOR]Причина блокировки: [COLOR=rgb(26, 188, 156)]{playerBLReason}[/COLOR] \n [COLOR=rgb(226, 80, 65)]5.[/COLOR]Блокировку произвел: [COLOR=rgb(26, 188, 156)]{blockerNickName}[/COLOR] \n [COLOR=rgb(226, 80, 65)]6.[/COLOR]Сервер на котором произошла блокировка: [COLOR=rgb(26, 188, 156)]{RustExtended.Core.ServerName}[/COLOR] \n [COLOR=rgb(226, 80, 65)]7.[/COLOR]IP ADRESS Сервера на котором произошла блокировка: [COLOR=rgb(26, 188, 156)]{RustExtended.Core.ServerIP}:{server.port}[/COLOR] \n----------------------------------\nВерсия плагина: 1.2.2(Alpha)";
                string msgforsend = $"[COLOR=rgb(226, 80, 65)]1.[/COLOR]Ник нарушителя: [COLOR=rgb(26, 188, 156)]{playerNick}[/COLOR] \n [COLOR=rgb(226, 80, 65)]2.[/COLOR]STEAMID нарушителя: [COLOR=rgb(26, 188, 156)]{playerClient.userID}[/COLOR]\n [COLOR=rgb(226, 80, 65)]3.[/COLOR]IP ADRESS нарушителя: [COLOR=rgb(26, 188, 156)]{playerClient.netPlayer.externalIP}[/COLOR] \n [COLOR=rgb(226, 80, 65)]4.[/COLOR]Причина блокировки: [COLOR=rgb(26, 188, 156)]{playerBLReason}[/COLOR] \n [COLOR=rgb(226, 80, 65)]5.[/COLOR]Блокировку произвел: [COLOR=rgb(26, 188, 156)]{blockerNickName}[/COLOR] \n [COLOR=rgb(226, 80, 65)]6.[/COLOR]Сервер на котором произошла блокировка: [COLOR=rgb(26, 188, 156)]{server.hostname}[/COLOR] \n [COLOR=rgb(226, 80, 65)] \n----------------------------------\nВерсия плагина: 1.2.2(Alpha)";

                string textforsend = "thread_id=" + thread_id + "&message=" + msgforsend;

                webrequest.EnqueuePost(apiPath + "posts/", textforsend, (code, response) => GetCallback(code, response), this, headers);
            }
            else
            {
                Helper.LogChat(messages["no_blocker_send_f"], true);
            }
        }

        void GiveResorseReg(NetUser netuser)
        {
            var inv = netuser.playerClient.controllable.controller.character.GetComponent<PlayerInventory>();
            foreach (object[] arr in GiveResorseRegistr)
            {
                if (arr == null || arr.Length != 3) continue;
                ItemDataBlock item = DatablockDictionary.GetByName((string)arr[1]);
                Inventory.Slot.Preference slot = Inventory.Slot.Preference.Define((Inventory.Slot.Kind)arr[0]);
                Helper.GiveItem(inv, item, slot, (int)arr[2], -1);
            }
        }

        #region messages
        static Dictionary<string, string> messages = new Dictionary<string, string>()
        {
            /* Errors */
            { "bad_command",                color_error + "Неверная команда! "+ color_main + "Воспользуйтесь командой \"/unsoft\" для получения информации"},
            { "bad_params_bl",              color_error + "Неправильные параметры команды! "+ color_main + "Правильно писать \"/bl <NICK> <REASON>\""},
            { "bad_params",                 color_error + "Неправильные параметры команды!" },
            { "bad_lengtharg",              color_error + "Веденные вами аргументы больше или меньше чем" },
            { "no_reason",                  color_error + "Вы не ввели причины блокировки" },
            { "no_permissions",             color_error + "У вас нет прав для использования данной комманды!" },
            { "no_forum_account",           color_error + "У вас ещё нет аккаунта на форуме!" },
            { "no_turn_command",            color_error + "Данная команда отключена в настройках плагина" },
            { "no_blocker_method",          color_error + "Не указан метод блокировки пользователя в настройках плагина" },
            { "no_blocker_send_f",          color_error + "Команда выполнена, но данные на форум отправлены не были! Т.к отправка отключена или вы не указали тему для отправки" },
            { "impossible_action",          color_error + "Данное действие невозможно!" },
            { "unknown_user",               color_error + "Запрашиваемый пользователь не найден" },
            { "already_has_account",        color_error + "Вы уже вошли на форуме" },
            { "error_cant_check_mail",      color_error + "Произошла ошибка в ходе проверки почты" },
            { "email_not_same",             color_error + "Неверная почта для этого аккаунта" },

            /* Msg */
            { "my_profile_info",            color_main + $"Информация о вашем аккаунте:|Ваш ник на форуме: {color_double}%USERNAME%|{color_main}Бонус выдан: {color_double}%BONUSGIVED%" },
            { "forum_add_account",          color_main + "Используйте " + color_double + "\"/unsoft add <ник на форуме> <email на форуме>\" " + color_main +  "чтобы привязать аккаунт" },
            { "forum_info_reg_authgr",      color_main + "Выдаются ли веще при регистрации:"},
            { "forum_create_account",       color_main + "Для его регистрации перейдите по ссылке -"+ color_double + "universesoft.space" },
            { "forum_success_reg",          color_main + "Вы успешно привязали аккаунт на форуме" },
            { "forum_success_reg_authgr",   color_main + "Вам был выдан бонус за регистрацию!" + color_double +" Проверьте ваш инвентарь" },
            { "forum_success_reg_no_authgr",color_main + "Огромное спасибо за регистрацию!" }
        };
        #endregion
        private void RegisterProfile(string userName, string EMail, ulong userID)
        {
            if (GetPlayerForumData(userID) != null)
                return;
            data.dataBase.Add(new ForumData()
            {
                Username = userName,
                EMail = EMail,
                OwnerID = userID,
                BonusGived = AuthGR
            });
        }

        [ChatCommand("unsoft")]
        void CMD_UnSoft(NetUser netuser, string command, string[] args)
        {
            ForumData playerData = GetPlayerForumData(netuser.userID);
            if (playerData == null && (args.Length == 0 || args[0].ToLower() != "add"))
            {
                string AuthR = AuthGR ? "Да" : "Нет";
                rust.SendChatMessage(netuser, chatname, messages["no_forum_account"]);
                rust.SendChatMessage(netuser, chatname, messages["forum_create_account"]);
                rust.SendChatMessage(netuser, chatname, messages["forum_add_account"]);
                rust.SendChatMessage(netuser, chatname, messages["forum_info_reg_authgr"] + AuthR);
                return;
            }
            if (args.Length == 0)
            {
                string bonusGived = playerData.BonusGived ? "Да" : "Нет";
                foreach (string message in messages["my_profile_info"].Split(new string[] { "|" }, StringSplitOptions.None))
                    rust.SendChatMessage(netuser, chatname, message
                        .Replace("%BONUSGIVED%", bonusGived)
                        .Replace("%USERNAME%", playerData.Username));
                return;
            }
            if (args[0].ToLower() == "add")
            {
                if (playerData != null)
                {
                    rust.SendChatMessage(netuser, chatname, messages["already_registered"]);
                    return;
                }
                if (args.Length < 2)
                {
                    rust.SendChatMessage(netuser, chatname, messages["bad_params"]);
                    rust.SendChatMessage(netuser, chatname, messages["bad_lengtharg"] + " 3");
                    return;
                }
                string username = args[1];
                string email = args[2];
                var headers = new Dictionary<string, string>
                {
                    { "XF-Api-Key", apikey },
                    { "XF-Api-User", apiuser }
                };
                webrequest.EnqueueGet(apiPath + "users/find-name?username=" + username, (code, response) =>
                {
                    if (response == null || code != 200)
                    {
                        Helper.LogError(response, true);
                        return;
                    }
                    Dictionary<string, object> jsonMain = JsonConvert.DeserializeObject<Dictionary<string, object>>(response);
                    if (jsonMain["exact"] == null || jsonMain["exact"].ToString() == "Null")
                    {
                        rust.SendChatMessage(netuser, chatname, messages["unknown_user"]); // Если по нику пользователя не нашло
                        Helper.LogChat("[UnSOFT][AUTH] Error! Unknown User", true);
                        return;
                    }
                    Dictionary<string, object> userInfo = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonMain["exact"].ToString());
                    if (userInfo == null || userInfo["email"] == null)
                    {
                        rust.SendChatMessage(netuser, chatname, messages["error_cant_check_mail"]); // Если вдруг сайт поляжет и проверить не сможет
                        Helper.LogChat("[UnSOFT][AUTH] Error! No connection forum", true);
                        return;
                    }
                    if (email != userInfo["email"].ToString())
                    {
                        rust.SendChatMessage(netuser, chatname, messages["email_not_same"]); // Если почта не совпадает, бросает в ошибку
                        Helper.LogChat("[UnSOFT][AUTH] Error! Еmail not same", true);
                        return;
                    }
                    RegisterProfile(userInfo["username"].ToString(), userInfo["email"].ToString(), netuser.userID); // Регает
                    rust.SendChatMessage(netuser, chatname, messages["forum_success_reg"]);
                    if (AuthGR == true)
                    {
                        //ЗДЕСЬ ДОЛЖЕН БЫТЬ КОД, КОТОРЫЙ МЕНЯЕТ ЗНАЧЕНИЕ BonusGived
                        GiveResorseReg(netuser);
                        rust.SendChatMessage(netuser, chatname, messages["forum_success_reg_authgr"]);
                    }
                    else
                    {
                        rust.SendChatMessage(netuser, chatname, messages["forum_success_reg_no_authgr"]);
                    }
                    Helper.LogChat("[UnSOFT][AUTH] Success! Player register", true);
                }, this, headers);
            }
            else
            {
                rust.SendChatMessage(netuser, chatname, messages["bad_command"]);
                return;
            }
        }

        [ChatCommand("bl")]
        void CMD_bl(NetUser netuser, string cmdd, string[] args)
        {
            if (Blocker == true)
            {
            RustExtended.UserData userData = Users.GetBySteamID(netuser.playerClient.userID);
            if (userData.Rank < BlockerRank)
            {
                rust.SendChatMessage(netuser, chatname, messages["no_permission"]);
                Helper.LogChat("[UnSOFT][BLOCKER] Error! No Permissions", true);
                return;
            }

            if (args.Length == 0 || args.Length > 2)
            {
                rust.SendChatMessage(netuser, chatname, messages["bad_params_bl"]);
                rust.SendChatMessage(netuser, chatname, messages["bad_lengtharg"] + " 2" );
                Helper.LogChat("[UnSOFT][BLOCKER] Error! No ARGS", true);
                return;
            }

            string blockerNickName = userData.Username;
            string playerNick = args[0];
            string playerBLReason = args[1];

            PlayerClient playerClient = Helper.GetPlayerClient(playerNick);

            if (playerClient == null)
            {
                rust.SendChatMessage(netuser, chatname, $"{color_main}Игрок с ником {color_double}\"" + playerNick + $"\" {color_main}не найден!");
                return;
            }

            if (playerClient.userName != "")
                playerNick = playerClient.userName;
            if (blockerNickName == "")
                blockerNickName = "Администратор";

            if (blockerNickName == playerNick)
            {
                rust.SendChatMessage(netuser, chatname, messages["impossible_action"]);
                return;
            }
            else
            {
                    if (BlockerMethod < 1 ||  BlockerMethod > 3)
                    { 
                        rust.SendChatMessage(netuser, chatname, messages["bad_params"]);
                        rust.SendChatMessage(netuser, chatname, messages["no_blocker_method"]);
                        Helper.LogChat("[UnSOFT][BLOCKER] Error! No blocker method", true);
                    }
                    else
                    { 

                        if (BlockerMethod == 1) { CMD("serv.ban \"" + playerClient.userID + "\""); BlockerInformer(unicon, playerNick, playerBLReason, blockerNickName, playerClient, netuser); }
                        if (BlockerMethod == 2) { CMD("serv.block \"" + playerClient.userID + "\""); BlockerInformer(unicon, playerNick, playerBLReason, blockerNickName, playerClient, netuser); }
                        if (BlockerMethod == 3) { CMD("serv.ban \"" + playerClient.userID + "\""); CMD("serv.block \"" + playerClient.userID + "\""); BlockerInformer(unicon, playerNick, playerBLReason, blockerNickName, playerClient, netuser); }

                    }
                }
            }
            else
            {
                rust.SendChatMessage(netuser, chatname, messages["no_turn_command"]);
                Helper.LogChat("[UnSOFT][BLOCKER] Error! No turn bloker", true);
            }

        }

        void GetCallback(int code, string response)
        {
            if (response == null || code != 200)
            {
				Helper.LogError(response, true);
                return;
            }
            //Helper.LogChat(response, true);
        }
    }
}
