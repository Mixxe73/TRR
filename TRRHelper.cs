using System;
using UnityEngine;
using Oxide.Core;
using System.Collections.Generic;
using RustExtended;
using Oxide.Core.Libraries;

namespace Oxide.Plugins
{
    [Info("TRRHelper", "Mixxe73", "1.0.0")]
    class TRRHelper : RustLegacyPlugin
    {
		public string chatName = "Информация";

		[ChatCommand("help")] //команда
		void cmdTRRPOS(NetUser netuser, string command, string[] args)
		{
            rust.SendChatMessage(netuser, chatName, "[COLOR#3CB371]Доступные команды:");
            rust.SendChatMessage(netuser, chatName, "[COLOR#3CB371]*/[COLOR#FFFFFF]admins ➭ Открыть список активности администрации!");
            rust.SendChatMessage(netuser, chatName, "[COLOR#3CB371]*/[COLOR#FFFFFF]report ➭ Отправить жалобу об нарушение правил игрового сервера!");
            rust.SendChatMessage(netuser, chatName, "[COLOR#3CB371]*/[COLOR#FFFFFF]unsoft ➭ Открыть информацию об интеграции аккаунта с форумом UnSOFT!");

            rust.SendChatMessage(netuser, chatName, "[COLOR#3CB371]Город мирных:");
            rust.SendChatMessage(netuser, chatName, "[COLOR#3CB371]*/[COLOR#FFFFFF]games ➭ Открыть список азартных игр!");
            //rust.SendChatMessage(netuser, chatName, "[COLOR#3CB371]*/[COLOR#FFFFFF]rec ➭ Открыть меню переработчика!");
            if (netuser.CanAdmin())
            {
            rust.SendChatMessage(netuser, chatName, "[COLOR#3CB371]Команды администратора:");
            rust.SendChatMessage(netuser, chatName, "[COLOR#3CB371]*/[COLOR#FFFFFF]app ➭ Передать координаты в консоль!");
            rust.SendChatMessage(netuser, chatName, "[COLOR#3CB371]*/[COLOR#FFFFFF]bl ➭ Заблокировать игрока с отправкой на форум!");
            }
            rust.SendChatMessage(netuser, chatName, "[COLOR#3CB371]ABOUT INFO![COLOR#FFFFFF] Данный сервер использует лицензионный игровой мод TRR!");
    }   

    void Loaded()
        {
            timer.Repeat(1200f, 0, () =>
            {
                Broadcast.MessageAll("[COLOR#00FA9A]Дорогой друг! [COLOR#FFFFFF]Не забудь заглянуть в нашу группу вконтакте!", chatName);
                Broadcast.MessageAll("[COLOR#00FA9A]Наше сообщество ВКонтакте - [COLOR#FFFFFF]vk.com/trr_rust", chatName);
            });

            timer.Repeat(6000f, 0, () =>
            {
                Broadcast.MessageAll("[COLOR#00FA9A]Вы играете на сервере [COLOR#ffffff]< The Revival Rust >", chatName);
                Broadcast.MessageAll("[COLOR#FFFFFF]Хорошего времяпровождения!", chatName);
            });

        
        }
	}
}