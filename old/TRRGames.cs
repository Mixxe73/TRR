using System;
using UnityEngine;
using RustExtended;
using Oxide.Core;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;

namespace Oxide.Plugins
{
    [Info("TRRGames", "Mixxe73", "1.2.0")]
    class TRRGames : RustLegacyPlugin
    {
        string servername = "TRR";
        string chatName = "Азартные Игры";

        #region [VARS]Color
        const string errorColor = "[COLOR #FE2E2E]";
        const string selectColor = "[COLOR #FFA500]";
        const string mainColor = "[COLOR #FFFFFF]";
        const string doubleColor = "[COLOR #C8FE2E]";
        #endregion

        #region [VARS][COINFLIP]
        bool CommandCoinflip = true;
        ulong minCoinflipAmount = 50;
        string CoinflipNPCName = "Шулер";
        string CoinflipName = "Орел & Решка";
        #endregion

        #region MSG

        /* Список всех сообщений */
        static Dictionary<string, string> messages = new Dictionary<string, string>()
            {
                /* Errors */
                { "bad_params",         errorColor + "Неправильные параметры команды! /games - список команд" },
                { "bad_amount",         errorColor + "Ошибка! Минимальная сумма ставки составляет {0}$" },
                { "not_act_command",    errorColor + "Данная команда отключена в параметрах сервера!" },
                { "not_enough_money",   errorColor + "У вас недостаточно средств на балансе! Ваш баланс: [color white]{0}$" },
                { "numbers_only",       errorColor + "Последний параметр должен быть числом!" },


                { "you_win",            selectColor + "Вы выйграли " + mainColor + " {0}$"},
                { "you_lose",            errorColor + "Вы проиграли:" + mainColor + " {0}$"},

            };

            static string[] helpMsg = new string[]
            {
                $"{doubleColor}Команды игровго казино:",
                "/games info - получить информацию о команде",
                "/games coinflip <Сумма> - сыграть в монетку"
            };

            static string[] infoMsg = new string[]
            {
                $"{doubleColor}Информация:",
                "Все данные команды доступны только в городе мирных!",
                $"1. Coinflip - это азартная игра, в которую можно сыграть находясь возле <Шулера>!",
            };

            static string[] adminMsg = new string[]
            {
                $"{errorColor}*/games",
                $"{errorColor}*/games",
                $"{errorColor}*/games"
            };

        #endregion

        [ChatCommand("games")]
        void cmdGames(NetUser netUser, string command, string[] args)
        {
            string text = $"Command [{netUser.displayName}:{netUser.userID}] /" + command;
            foreach (string s in args) text += " " + s;
            Helper.LogChat(text, true);

            if (args.Length == 0)
            {
                foreach (string msg in helpMsg)
                {
                    rust.SendChatMessage(netUser, chatName, msg);
                }

                if (netUser.CanAdmin())
                {
                    foreach (string msg in adminMsg)
                    {
                        rust.SendChatMessage(netUser, chatName, msg);
                    }
                }
                return;
            }

            switch (args[0])
            {
                case "info":
                cmdInfo(netUser, args);
                    return;
                case "coinflip":
                cmdCoinflip(netUser, args);
                    return;
            }
            rust.SendChatMessage(netUser, chatName, messages["bad_params"]);
        }

        #region info
        void cmdInfo(NetUser netUser, string[] args)
        {
            foreach (string msg in infoMsg)
            {
                rust.SendChatMessage(netUser, chatName, msg);
            }
        }
        #endregion

        #region coinflip
        void cmdCoinflip(NetUser netUser, string[] args)
        {
            if (CommandCoinflip)
            {
            /* Проверка баланса */
            ulong amount = 1000;
            try
            {
                amount = ulong.Parse(args[1]);
            }
            catch
            {
                rust.SendChatMessage(netUser, CoinflipName, messages["numbers_only"]);
                return;
            }

            if (amount < minCoinflipAmount)
            {
                rust.SendChatMessage(netUser, CoinflipName, string.Format(messages["bad_amount"], minCoinflipAmount));
                return;
            }

            ulong balance = Economy.GetBalance(netUser.userID);
            if (balance < amount)
            {
                rust.SendChatMessage(netUser, CoinflipName, string.Format(messages["not_enough_money"], balance));
                return;
            }

            int NpcSide = Core.Random.Range(0, 2);
            int Winner = Core.Random.Range(0, 2);

            rust.SendChatMessage(netUser, CoinflipNPCName, errorColor + "Играем по моим правилам!" + mainColor + "Я" + ((NpcSide == 1) ? " Орел" : " Решка" + "!"));
            rust.SendChatMessage(netUser, CoinflipName, doubleColor + "Победитель: " + mainColor + ((Winner == 1) ? "Орел" : "Решка" + "!"));

            if (NpcSide != Winner)
            {
                rust.SendChatMessage(netUser, CoinflipName, string.Format(messages["you_win"], amount));
                RustExtended.Economy.BalanceAdd(netUser.userID, amount);
            }
            else
            {
                rust.SendChatMessage(netUser, CoinflipName, string.Format(messages["you_lose"], amount));
                RustExtended.Economy.BalanceSub(netUser.userID, amount);
            }
            }
            else
            {
                rust.SendChatMessage(netUser, servername, string.Format(messages["not_act_command"]));
            }

            
        }
        #endregion
    }
}
