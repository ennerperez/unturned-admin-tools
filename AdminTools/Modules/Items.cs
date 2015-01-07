using CommandHandler;
using System;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

namespace Unturned
{
    internal class Items : Module
    {

        #region TOP: global variables are initialized here

        internal static Timer Timer;
        internal static int Interval = 2700;

        internal static bool UseRespawnItems = false;

        #endregion

        internal override void Load()
        {

            if (String.IsNullOrEmpty(Configs.File.IniReadValue("Config", "UseRespawnItems")))
            {
                Configs.File.IniWriteValue("Config", "UseRespawnItems", "true");
                Configs.File.IniWriteValue("Timers", "RespawnItems", "2700");
            }

            Items.UseRespawnItems = Boolean.Parse(Configs.File.IniReadValue("Config", "UseRespawnItems"));
            Items.Interval = Int32.Parse(Configs.File.IniReadValue("Timers", "RespawnItems"));

            if (Items.UseRespawnItems)
            {
                if (Timer != null)
                {
                    Timer = new Timer(Interval * 1000);
                    Timer.Elapsed += itemsTimer_Elapsed;
                    Timer.Enabled = true;
                }
            }
        }

        internal override IEnumerable<Command> GetCommands()
        {
            List<Command> _return = new List<Command>();
            _return.Add(new Command(PermissionLevel.Moderator.ToInt(), Spawn, "spawn", "i", "get")); // Use /i <itemid>
            _return.Add(new Command(PermissionLevel.Moderator.ToInt(), Reset, "resetitems", "ir"));
            _return.Add(new Command(PermissionLevel.Admin.ToInt(), SetDelay, "setitemsdelay"));
            return _return;
        }
        internal override String GetHelp()
        {
            return null;
        }

        #region Commands

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

        #endregion

        #region Private calls

        private static void itemsTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            resetItems();
        }
        private static void setItemsDelay(int seconds)
        {
            if (Timer != null && Timer.Enabled)
            {
                Interval = seconds;
                Timer.Stop();
                Timer.Interval = seconds * 1000;
                Timer.Start();
            }
        }
        private static void resetItems()
        {
            SpawnItems.reset();
        }

        #endregion

    }
}
