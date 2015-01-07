using CommandHandler;
using System;

namespace Unturned
{
    public static class Kicks
    {
        #region TOP: global variables are initialized here

        #endregion

         internal static void GetCommands()
        {
            CommandList.add(new Command(PermissionLevel.Moderator.ToInt(), Kick, "kick", "k"));
        }


        internal static void Kick(CommandArgs args)
        {
            String name = args.ParametersAsString;
            kick(name, "You were kicked off the server");
        }

        internal static void kick(String name, String reason)
        {
            BetterNetworkUser user = UserList.getUserFromName(name);
            NetworkTools.kick(user.networkPlayer, reason);
        }
    }
}
