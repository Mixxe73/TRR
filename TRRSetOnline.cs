using System;
using UnityEngine;
using RustExtended;

namespace Oxide.Plugins
{
    [Info("SetOnline", "Romanchik34", "1.0.0")]
    class TRRSetOnline : RustLegacyPlugin
    {
        void Loaded()
        {
            timer.Repeat(10f, 0, () =>
            {
                foreach (PlayerClient player in PlayerClient.All)
                {
                    player.networkView.RPC("SetOnline", player.netPlayer, "TRRMod.ShowOnline.SetOnline", PlayerClient.All.Count, server.maxplayers);
                }
            });
        }
    }
}
