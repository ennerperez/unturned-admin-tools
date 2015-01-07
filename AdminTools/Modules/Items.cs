using CommandHandler;
using System;
using System.Timers;
using UnityEngine;

namespace Unturned
{
    public static class Items
    {
        #region TOP: global variables are initialized here

        internal static Timer itemsTimer;
        internal static int itemsResetIntervalInSeconds = 2700;

        #endregion

        internal static void Load()
        {
            if (itemsTimer != null)//&& itemsTimer.Enabled)
            {
                itemsTimer = new Timer(itemsResetIntervalInSeconds * 1000);
                itemsTimer.Elapsed += itemsTimer_Elapsed;
                itemsTimer.Enabled = true;
            }
        }

        internal static void GetCommands()
        {
            CommandList.add(new Command(PermissionLevel.Moderator.ToInt(), Spawn, "spawn", "i", "get")); // Use /i <itemid>
            CommandList.add(new Command(PermissionLevel.Moderator.ToInt(), Reset, "resetitems", "ir"));
            CommandList.add(new Command(PermissionLevel.Admin.ToInt(), SetDelay, "setitemsdelay"));
        }

        internal static void Reset(CommandArgs args)
        {
            SpawnItems.reset();
            NetworkChat.sendAlert(String.Format("{0} has respawned all items.", args.sender.name));
        }
        internal static void SetDelay(CommandArgs args)
        {
            String seconds = args.Parameters[0];
            setItemsDelay(Convert.ToInt32(seconds));
        }
        internal static void Spawn(CommandArgs args)
        {
            int itemid = Convert.ToInt32(args.Parameters[0]);
            int amount = 1;
            if (args.Parameters.Count == 2) { amount = Convert.ToInt32(args.Parameters[1]); }

            Vector3 location = args.sender.position;

            for (int i = 0; i < amount; i++)
                SpawnItems.spawnItem(itemid, 1, location);

        }

        private static void itemsTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            resetItems();
        }
        private static void setItemsDelay(int seconds)
        {
            if (itemsTimer != null && itemsTimer.Enabled)
            {
                itemsResetIntervalInSeconds = seconds;
                itemsTimer.Stop();
                itemsTimer.Interval = seconds * 1000;
                itemsTimer.Start();
            }
        }
        private static void resetItems()
        {
            SpawnItems.reset();
        }

    }
}
