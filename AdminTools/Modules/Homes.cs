﻿using CommandHandler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Unturned
{
    internal class Homes : Module
    {

        #region TOP: global variables are initialized here

        internal static Dictionary<String, Vector3> PlayerHomes = new Dictionary<String, Vector3>();
        internal static bool UsePlayerHomes = false;

        private static string Source = System.IO.Path.Combine(AdminTools.AdminPath, "homes.txt");

        #endregion

        internal override void Save()
        {
            Configs.File.IniWriteValue("Modules", "Homes", (UsePlayerHomes) ? "true" : "false");
        }
        internal override void Load()
        {

            if (String.IsNullOrEmpty(Configs.File.IniReadValue("Modules", "Homes")))
            {
                this.Save();
            }

            Homes.UsePlayerHomes = Boolean.Parse(Configs.File.IniReadValue("Modules", "Homes"));

            if (Homes.UsePlayerHomes)
            {

                if (!File.Exists(Source)) { Create(); }

                PlayerHomes = new Dictionary<String, Vector3>();
                string[] homes = System.IO.File.ReadAllLines(Source);
                foreach (string item in homes)
                {
                    try
                    {
                        string[] values = item.Split(':');
                        String id = values[0];
                        String[] location = values[1].Split(',');
                        String x = location[0];
                        String y = location[1];
                        String z = location[2];
                        Vector3 loc = new Vector3(Convert.ToSingle(x), Convert.ToSingle(y), Convert.ToSingle(z));
                        PlayerHomes.Add(id, loc);
                    }
                    catch (Exception ex)
                    {
                        Shared.Log(ex.ToString());
                    }
                }

            }

        }
        internal override void Create()
        {
            System.IO.StreamWriter file = new StreamWriter(Source, true);
            file.Close();
        }
        internal override void Clear()
        {
            Homes.PlayerHomes = null;
        }

        internal override IEnumerable<Command> GetCommands()
        {
            List<Command> _return = new List<Command>();
            _return.Add(new Command(PermissionLevel.Level1.ToInt(), SetHome, "sethome", "sh"));
            _return.Add(new Command(PermissionLevel.Level1.ToInt(), Home, "home", "h"));
            return _return;
        }
        internal override String GetHelp()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(Strings.Get("HLP", "HomesSetHomeHelp"));
            sb.AppendLine(Strings.Get("HLP", "HomesHomeHelp"));

            return sb.ToString();
        }

        #region Commands

        internal static void SetHome(CommandArgs args)
        {
            if (UsePlayerHomes)
            {
                Vector3 location = NetworkUserList.getModelFromPlayer(args.sender.networkPlayer).transform.position;
                setHome(args.sender.steamid, location);
                Reference.Tell(args.sender.networkPlayer, "Home set.");
            }
        }
        internal static void Home(CommandArgs args)
        {
            if (UsePlayerHomes)
            {
                home(args.sender);
                Reference.Tell(args.sender.networkPlayer, "Teleported home. Don't move for 5 seconds if you don't wanna get kicked");
            }
        }

        #endregion

        #region Private calls

        private static void setHome(string steamID, Vector3 location)
        {
            string fileSource = System.IO.Path.Combine(AdminTools.AdminPath, "homes.txt");
            if (PlayerHomes.ContainsKey(steamID))
            {
                string[] lines = System.IO.File.ReadAllLines(fileSource);
                File.Delete(fileSource);

                System.IO.StreamWriter file = new StreamWriter(fileSource, true);

                foreach (string item in lines)
                {
                    if (!item.StartsWith(steamID))
                    {
                        file.WriteLine(item);
                    }
                }

                file.Close();
            }
            System.IO.StreamWriter file2 = new StreamWriter(fileSource, true);
            file2.WriteLine(String.Format("{0}:{1},{2},{3}", steamID, location.x, location.y, location.z));
            file2.Close();
            PlayerHomes[steamID] = location;
        }
        private static void home(BetterNetworkUser user)
        {
            userto(user, PlayerHomes[user.steamid]);
        }

        #endregion

        private static void userto(BetterNetworkUser user, Vector3 target)
        {
            try
            {
                user.position = target;
                user.player.gameObject.GetComponent<Life>().networkView.RPC("tellStatePosition", RPCMode.All, new object[] { target, user.rotation });
                user.player.gameObject.GetComponent<NetworkInterpolation>().tellStatePosition_Pizza(target, user.rotation);

                Network.SetReceivingEnabled(user.networkPlayer, 0, false);

                System.Threading.Timer timer;
                timer = new System.Threading.Timer(obj =>
                {
                    Network.SetReceivingEnabled(user.networkPlayer, 0, true);
                }, null, 2000, System.Threading.Timeout.Infinite);
            }
            catch (Exception ex)
            {
                Shared.Log(ex.Message);
            }
        }
        private static void userto(BetterNetworkUser user, Vector3 targetposition, Quaternion targetrotation)
        {
            try
            {
                user.position = targetposition;
                user.player.gameObject.GetComponent<Life>().networkView.RPC("tellStatePosition", RPCMode.All, new object[] { targetposition, targetrotation });
                user.player.gameObject.GetComponent<NetworkInterpolation>().tellStatePosition_Pizza(targetposition, targetrotation);

                Network.SetReceivingEnabled(user.networkPlayer, 0, false);

                System.Threading.Timer timer;
                timer = new System.Threading.Timer(obj =>
                {
                    Network.SetReceivingEnabled(user.networkPlayer, 0, true);
                }, null, 2000, System.Threading.Timeout.Infinite);
            }
            catch (Exception ex)
            {
                Shared.Log(ex.Message);
            }
        }

    }
}
