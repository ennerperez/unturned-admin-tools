using CommandHandler;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Unturned
{
    internal class Annoying : Module
    {

        #region TOP: global variables are initialized here

        internal static Dictionary<String, Vector3> VanishedPlayers = new Dictionary<String, Vector3>();
        internal static Dictionary<String, int> PunishedPlayers = new Dictionary<String, int>();

        internal static bool IsPublic = true;

        #endregion

        internal override void Load()
        {
            if (String.IsNullOrEmpty(Configs.File.IniReadValue("Config", "PublicAnnoying")))
            {
                Configs.File.IniWriteValue("Config", "PublicAnnoying", (Annoying.IsPublic) ? "true" : "false");
            }

            Annoying.IsPublic = Boolean.Parse(Configs.File.IniReadValue("Config", "PublicAnnoying"));

        }

        internal override void Refresh()
        {

            if (Annoying.VanishedPlayers.Count > 0)
            {
                foreach (KeyValuePair<String, Vector3> entry in Annoying.VanishedPlayers)
                {
                    BetterNetworkUser user = UserList.getUserFromSteamID(entry.Key);
                    if (user != null)
                    {
                        foreach (NetworkPlayer networkplayer in Network.connections)
                        {
                            if (Network.player != networkplayer && networkplayer != user.networkPlayer)
                            {
                                user.player.networkView.RPC("tellStatePosition", networkplayer, new object[] { new Vector3(0, 0, 0), user.rotation });

                            }
                        }

                    }

                }
            }

        }

        internal override IEnumerable<Command> GetCommands()
        {
            List<Command> _return = new List<Command>();
            _return.Add(new Command(PermissionLevel.Admin.ToInt(), Lag, "lag", "lg"));
            _return.Add(new Command(PermissionLevel.Admin.ToInt(), UnLag, "unlag", "ulg"));
            _return.Add(new Command(PermissionLevel.Owner.ToInt(), Horn, "horn", "h"));
            _return.Add(new Command(PermissionLevel.Owner.ToInt(), Vanish, "vanish", "vis"));
            _return.Add(new Command(PermissionLevel.Owner.ToInt(), God, "god"));
            return _return;
        }
        internal override String GetHelp()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(Strings.Get("HLP", "AnnoyingUnlagHelp"));
            sb.AppendLine(Strings.Get("HLP", "AnnoyingHorn"));
            sb.AppendLine(Strings.Get("HLP", "AnnoyingVanish"));
            sb.AppendLine(Strings.Get("HLP", "AnnoyingGod"));


            return sb.ToString();
        }

        #region Commands

        internal static void God(CommandArgs args)
        {
            Life life = args.sender.player.gameObject.GetComponent<Life>();
            //int health = life.get_health();
            //if (health > 100)
            //{
            //    life.networkView.RPC("tellAllLife", RPCMode.All, new object[] { 100, 100, 100, 100, false, false });
            //    life.damage(1, "poke");
            //    NetworkChat.sendAlert(String.Format("{0} is mortal again.", args.sender.name));
            //}
            //else
            //{
            //    life.networkView.RPC("tellAllLife", RPCMode.All, new object[] { 10000, 100, 100, 100, false, false });
            //    life.damage(1, "poke");
            //    NetworkChat.sendAlert(String.Format("{0} is a God.", args.sender.name));
            //}

            if (args.Parameters != null && args.Parameters.Count > 0 && args.ParametersAsString.ToLower().Equals("off"))
            {
                life.networkView.RPC("tellAllLife", RPCMode.All, new object[] { 100, 100, 100, 100, false, false });
                life.damage(1, "poke");
                NetworkChat.sendAlert(String.Format(Strings.Get("MOD", "AnnoyingGodOff"), args.sender.name));
            }
            else
            {
                life.networkView.RPC("tellAllLife", RPCMode.All, new object[] { 10000, 100, 100, 100, false, false });
                life.damage(1, "poke");
                NetworkChat.sendAlert(String.Format(Strings.Get("MOD", "AnnoyingGodOn"), args.sender.name));
            }

        }

        internal static void Lag(CommandArgs args)
        {
            string naam = args.ParametersAsString;
            BetterNetworkUser user = UserList.getUserFromName(naam);

            Network.SetReceivingEnabled(user.networkPlayer, 0, false);
            Reference.Tell(args.sender.networkPlayer, String.Format(Strings.Get("MOD", "AnnoyingLag"), user.name));
        }
        internal static void UnLag(CommandArgs args)
        {
            string naam = args.ParametersAsString;
            BetterNetworkUser user = UserList.getUserFromName(naam);

            Network.SetReceivingEnabled(user.networkPlayer, 0, true);
            Reference.Tell(args.sender.networkPlayer, String.Format(Strings.Get("MOD", "AnnoyingUnLag"), user.name));
        }
        internal static void Horn(CommandArgs args)
        {
            string naam = args.ParametersAsString;
            BetterNetworkUser user = UserList.getUserFromName(naam);
            Vector3 location = user.position;

            for (int i = 0; i < 100; i++)
            {
                NetworkSounds.askSound("Sounds/Vehicles/horn", location, 50f, 1F, 20f);
            }

        }
        internal static void Vanish(CommandArgs args)
        {
            try
            {
                VanishedPlayers.Add(args.sender.steamid, new Vector3(0, 0, 0));
                if (Annoying.IsPublic)
                {
                    NetworkChat.sendAlert(String.Format(Strings.Get("MOD", "AnnoyingVanishOn"), args.sender.name));
                }
                else
                {
                    Reference.Tell(args.sender.networkPlayer, String.Format(Strings.Get("MOD", "AnnoyingVanishOn"), args.sender.name));
                }               
                
            }
            catch
            {
                VanishedPlayers.Remove(args.sender.steamid);
                if (Annoying.IsPublic)
                {
                    NetworkChat.sendAlert(String.Format(Strings.Get("MOD", "AnnoyingVanishOff"), args.sender.name));
                }
                else
                {
                    Reference.Tell(args.sender.networkPlayer, String.Format(Strings.Get("MOD", "AnnoyingVanishOff"), args.sender.name));
                }
                
            }
        }

        #endregion

    }
}
