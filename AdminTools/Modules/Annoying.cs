using CommandHandler;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unturned
{
    internal class Annoying : Module
    {

        #region TOP: global variables are initialized here

        internal static Dictionary<String, Vector3> VanishedPlayers = new Dictionary<String, Vector3>();
        internal static Dictionary<String, int> PunishedPlayers = new Dictionary<String, int>();

        #endregion

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
            return null;
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
                NetworkChat.sendAlert(String.Format("{0} is mortal again.", args.sender.name));
            }
            else
            {
                life.networkView.RPC("tellAllLife", RPCMode.All, new object[] { 10000, 100, 100, 100, false, false });
                life.damage(1, "poke");
                NetworkChat.sendAlert(String.Format("{0} is a God.", args.sender.name));
            }

        }

        internal static void Lag(CommandArgs args)
        {
            string naam = args.ParametersAsString;
            BetterNetworkUser user = UserList.getUserFromName(naam);

            Network.SetReceivingEnabled(user.networkPlayer, 0, false);
            Reference.Tell(args.sender.networkPlayer, String.Format("You are now lagging the shit out of {0}.", user.name));
        }
        internal static void UnLag(CommandArgs args)
        {
            string naam = args.ParametersAsString;
            BetterNetworkUser user = UserList.getUserFromName(naam);

            Network.SetReceivingEnabled(user.networkPlayer, 0, true);
            Reference.Tell(args.sender.networkPlayer, String.Format("You are no longer lagging the shit out of {0}.", user.name));
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
                //Reference.Tell(args.sender.networkPlayer, "You have vanished :D");
                NetworkChat.sendAlert(String.Format("{0} is vanished.", args.sender.name));
            }
            catch
            {
                VanishedPlayers.Remove(args.sender.steamid);
                //Reference.Tell(args.sender.networkPlayer, "You are visible again.");
                NetworkChat.sendAlert(String.Format("{0} is visible again.", args.sender.name));
            }
        }

        #endregion

    }
}
