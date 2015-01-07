using CommandHandler;
using System;
using UnityEngine;

namespace Unturned
{
    public static class Teleports
    {
        #region TOP: global variables are initialized here

        #endregion

        internal static void GetCommands()
        {
            CommandList.add(new Command(PermissionLevel.User.ToInt(), To, "tpcoord", "tc", "tpto")); // Use /tc <x> <y> <[z]>
            CommandList.add(new Command(PermissionLevel.SuperUser.ToInt(), ToPlayer, "tplayer", "tp")); // Use /tp <playername>
            CommandList.add(new Command(PermissionLevel.Moderator.ToInt(), ToMe, "tptome", "tm")); // Use /tm <playername>
            CommandList.add(new Command(PermissionLevel.Owner.ToInt(), All, "tpall", "ta"));
        }

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
                userTo(args.sender, user.position, user.rotation);
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

            userTo(user, args.sender.position, args.sender.rotation);


        }
        internal static void To(CommandArgs args)
        {
            float x = float.Parse(args.Parameters[0]);
            float y = 100;
            float z = float.Parse(args.Parameters[1]);

            if (args.Parameters.Count > 2)
            {
                y = float.Parse(args.Parameters[1]);
                z = float.Parse(args.Parameters[2]);
            }
                        
            userTo(args.sender, new Vector3(x, y, z));

        }
        internal static void All(CommandArgs args)
        {
            Vector3 location = args.sender.position;
            Quaternion rotation = args.sender.player.gameObject.transform.rotation;

            foreach (BetterNetworkUser user in UserList.users)
            {
                //UserList.users.IndexOf(user)
                // TODO: Create a circle of users =O
                userTo(user, location, rotation);
            }
        }

        internal static void userTo(BetterNetworkUser user, Vector3 target)
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
        internal static void userTo(BetterNetworkUser user, Vector3 targetposition, Quaternion targetrotation)
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

    }
}
