using CommandHandler;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Unturned
{
    internal class Teleports : Module
    {

        #region TOP: global variables are initialized here

        #endregion

        internal override IEnumerable<Command> GetCommands()
        {
            List<Command> _return = new List<Command>();
            _return.Add(new Command(PermissionLevel.User.ToInt(), ToPoint, "tpcoord", "tc", "tpto"));
            _return.Add(new Command(PermissionLevel.SuperUser.ToInt(), ToPlayer, "tplayer", "tp")); // Use /tp <playername>
            _return.Add(new Command(PermissionLevel.Moderator.ToInt(), ToMe, "tptome", "tm")); // Use /tm <playername>
            _return.Add(new Command(PermissionLevel.Owner.ToInt(), ToMeAll, "tpall", "ta"));
            return _return;
        }
        internal override String GetHelp()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(Strings.Get("HLP", "TeleportsToHelp"));
            sb.AppendLine(Strings.Get("HLP", "TeleportsToPlayerHelp"));
            sb.AppendLine(Strings.Get("HLP", "TeleportsToMeHelp"));
            sb.AppendLine(Strings.Get("HLP", "TeleportsToMeAllHelp"));

            return sb.ToString();
        }

        #region Commands

        internal static void ToPlayer(CommandArgs args)
        {
            String name = args.ParametersAsString;
            BetterNetworkUser user;
            int number = 0;

            if (name.Length < 3 && Int32.TryParse(name, out number))
            {
                user = UserList.users[number];
            }
            else
            {
                user = UserList.getUserFromName(name);
            }
            if (user != null)
            {
                userto(args.sender, user.position, user.rotation);
            }

        }
        internal static void ToMe(CommandArgs args)
        {
            String name = args.ParametersAsString;
            BetterNetworkUser user;
            int number = 0;

            if (name.Length < 3 && Int32.TryParse(name, out number))
            {
                user = UserList.users[number];
            }
            else
            {
                user = UserList.getUserFromName(name);
            }

            userto(user, args.sender.position, args.sender.rotation);


        }
        internal static void ToPoint(CommandArgs args)
        {
            float x = float.Parse(args.Parameters[0]);
            float y = 100;
            float z = float.Parse(args.Parameters[1]);

            if (args.Parameters.Count > 2)
            {
                y = float.Parse(args.Parameters[1]);
                z = float.Parse(args.Parameters[2]);
            }

            userto(args.sender, new Vector3(x, y, z));

        }
        internal static void ToMeAll(CommandArgs args)
        {
            Vector3 location = args.sender.position;
            Quaternion rotation = args.sender.player.gameObject.transform.rotation;

            foreach (BetterNetworkUser user in UserList.users)
            {
                //UserList.users.IndexOf(user)
                // TODO: Create a circle of users =O
                userto(user, location, rotation);
            }
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
