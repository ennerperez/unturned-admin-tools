using CommandHandler;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Unturned
{
    internal class Freezes : Module
    {

        #region TOP: global variables are initialized here

        internal static Dictionary<String, Vector3> FrozenPlayers = new Dictionary<String, Vector3>();

        #endregion

        internal override void Refresh()
        {

            if (Freezes.FrozenPlayers.Count > 0)
            {
                foreach (KeyValuePair<String, Vector3> entry in Freezes.FrozenPlayers)
                {
                    BetterNetworkUser user = UserList.getUserFromSteamID(entry.Key);
                    if (user != null)
                    {
                        user.player.transform.position = entry.Value;

                        user.player.networkView.RPC("tellStatePosition", user.player.networkView.owner, new object[] { entry.Value, user.player.transform.rotation });
                    }

                }
            }

        }

        internal override IEnumerable<Command> GetCommands()
        {
            List<Command> _return = new List<Command>();
            _return.Add(new Command(PermissionLevel.Moderator.ToInt(), Freeze, "freeze", "pf")); 
            _return.Add(new Command(PermissionLevel.Moderator.ToInt(), UnFreeze, "unfreeze", "puf")); 
            return _return;
        }
        internal override String GetHelp()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(Strings.Get("HLP", "FreezeFreezeHelp"));
            sb.AppendLine(Strings.Get("HLP", "FreezeUnFreezeHelp"));


            return sb.ToString();
        }

        #region Commands

        internal static void Freeze(CommandArgs args)
        {
            string naam = args.ParametersAsString;

            if (naam.ToLower() == "all")
            {
                int cdUsers = 0;
                foreach (BetterNetworkUser item in UserList.users)
                {
                    Player player = item.player;
                    FrozenPlayers.Add(item.steamid, player.transform.position);
                    //Reference.Tell(item.networkPlayer, msg);
                    cdUsers++;
                }
                Reference.Tell(args.sender.networkPlayer, String.Format(Strings.Get("MOD","FreezesFreezes"), cdUsers));
            }
            else
            {
                BetterNetworkUser user = UserList.getUserFromName(naam);
                if (user != null)
                {
                    Player player = user.player;
                    FrozenPlayers.Add(user.steamid, player.transform.position);
                }
                Reference.Tell(args.sender.networkPlayer, String.Format("Froze the player {0}.", user.name));

            }

        }
        internal static void UnFreeze(CommandArgs args)
        {
            string naam = args.ParametersAsString;

            if (naam.ToLower() == "all")
            {
                int cdUsers = FrozenPlayers.Count;
                FrozenPlayers.Clear();

                Reference.Tell(args.sender.networkPlayer, String.Format(Strings.Get("MOD", "FreezesUnFreezes"), cdUsers));
            }
            else
            {
                BetterNetworkUser user = UserList.getUserFromName(naam);
                if (user != null)
                {
                    FrozenPlayers.Remove(user.steamid);
                }
                Reference.Tell(args.sender.networkPlayer, String.Format("Unfroze the player {0}.", user.name));
            }
        }

        #endregion

    }
}
