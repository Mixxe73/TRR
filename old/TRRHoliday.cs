using System;
using UnityEngine;
using Oxide.Core;
using System.Collections.Generic;
using RustExtended;
using Oxide.Core.Libraries;

namespace Oxide.Plugins
{
    [Info("TRRHoliday", "Mixxe73", "1.0.0")]
    class TRRHoliday : RustLegacyPlugin
    {
		string chatName = "TRR";
        int Holiday = 2; // Текущий праздник (0 - ОТКЛЮЧЕНО 1 - ОТКРЫТИЕ СЕРВЕРА | 2 - НОВЫЙ ГОД |)
        bool Announce = true; // Включена ли функция поздравления в чат
        float AnnounceDelay = 1600f; // Задержка на отправку поздравлений в чат

        string GiftName = "Armor Part 1"; // Имя предмета (Большой подарок)

        #region Data(Save/Load)
        public List<ulong> HolidayActivated;
        void OnServerSave() => Interface.GetMod().DataFileSystem.WriteObject("HolidayDB", HolidayActivated);
        #endregion
        
        void Loaded()
        {
        AnnounceChat(); // Подключает вывод в чат ко кд
        HolidayActivated = Interface.GetMod().DataFileSystem.ReadObject<List<ulong>>("HolidayDB"); //Сохраняет стим ид в дату
        }

        void AnnounceChat()
        {
            if (Announce && Holiday != 0)
            {

            if (Holiday == 1) //Если сейчас праздник - открытия сервера
            {
            timer.Repeat(AnnounceDelay, 0, () =>
            {
                Broadcast.MessageAll("[COLOR#00FA9A]Выжившие, [COLOR#ffffff]мы очень рады что вы зашли именно сегодня! В наш [COLOR #C8FE2E]день рождения.", chatName);
                Broadcast.MessageAll("[COLOR#00FA9A]В честь запуска проекта, администрация проекта дарит вам подарок [COLOR#FFFFFF]</holiday>", chatName);
                Broadcast.MessageAll("[COLOR#FFFFFF]Хороших вам рейдов, выжившие!", chatName);
            });
            }

            if (Holiday == 2) //Если сейчас праздник - новый год
            {
            timer.Repeat(AnnounceDelay, 0, () =>
            {
                Broadcast.MessageAll("[COLOR#00FA9A]Дорогой друг! [COLOR#FFFFFF]Спасибо что ты зашел в такой праздничный день!", chatName);
                Broadcast.MessageAll("[COLOR#00FA9A]В честь наступающего нового года, администрация проекта дарит вам подарок [COLOR#FFFFFF]</holiday>", chatName);
                Broadcast.MessageAll("[COLOR#FFFFFF]С наступающим новым годом ❆", chatName);
            });
            }
            }
        }

        void HolidayOpen(Inventory inventory)
        {
            inventory.AddItemAmount(DatablockDictionary.GetByName(GiftName), 1); //выдает больШой подарок
        }

        void HolidayNewYear(Inventory inventory)
        {
            inventory.AddItemAmount(DatablockDictionary.GetByName(GiftName), 2); //выдает больШой подарок
        }

        [ChatCommand("holiday")] //команда
		void cmdTRRPOS(NetUser netuser, string command, string[] args)
		{
            if (HolidayActivated.Contains(netuser.userID))
            {
                rust.SendChatMessage(netuser, chatName, "[color red]Вы уже использовали праздничный подарок");
                return;
            }
            HolidayActivated.Add(netuser.userID);

            Inventory inventory = netuser.playerClient.controllable.GetComponent<Inventory>();

            if(Holiday != 0)
            {
                if(Holiday == 1) //Если сейчас праздник - открытия сервера
                HolidayOpen(inventory);
                if(Holiday == 2) //Если сейчас праздник - новый год
                HolidayNewYear(inventory);
                rust.SendChatMessage(netuser, chatName, "Вы успешно получили праздничный набор!"); //Вывод в чат если челик получил набор
                Broadcast.MessageAll("[COLOR#00FA9A]Выживший [COLOR#FFFFFF] " + netuser.displayName + " [COLOR#00FA9A]получил праздничный подарок!", chatName);
                Debug.Log("Выживший " + netuser.displayName + " получил праздничный подарок!"); //вывод в консоль
            }
            else
            {
                rust.SendChatMessage(netuser, chatName, "Сейчас не проходит какого либо праздника!"); //Вывод в чат если нет праздница
            }
        }




	}
}