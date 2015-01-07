using CommandHandler;
using Ini;
using System;
using System.IO;
using System.Text;

namespace Unturned
{
    public static class Configs
    {

        #region TOP: global variables are initialized here

        private static Boolean isLoaded;

        internal static bool Developer = false;
        internal static bool DeathMessage = false;

        internal static bool Logging = false;

        internal static string FileSource = System.IO.Path.Combine(AdminTools.Path, "config.ini");
        internal static IniFile File =  new IniFile(Configs.FileSource);

        #endregion

        internal static void Load()
        {

            if (Configs.isLoaded) { return; }

            Directory.CreateDirectory(AdminTools.Path);
            if (!System.IO.File.Exists(Configs.FileSource)) { Create(); }
                      

            ////things that need to be added to existing files
            //if (Configs.File.IniReadValue("Config", "WhitelistKickMessages").Equals(""))
            //{
            //    Configs.File.IniWriteValue("Config", "WhitelistKickMessages", "true");
            //}
            //if (Configs.File.IniReadValue("Security", "Console").Equals(""))
            //{
            //    Configs.File.IniWriteValue("Security", "Console", "true");
            //}
            //if (Configs.File.IniReadValue("Security", "Password").Equals(""))
            //{
            //    Configs.File.IniWriteValue("Security", "Password", randomString(8));
            //}
            //if (Configs.File.IniReadValue("Security", "Confirmation").Equals(""))
            //{
            //    Configs.File.IniWriteValue("Security", "Confirmation", "false");
            //}

            Configs.Developer = Boolean.Parse(Configs.File.IniReadValue("Config", "Developer"));
            Configs.Logging = Boolean.Parse(Configs.File.IniReadValue("Config", "Logging"));

            Configs.isLoaded = true;

        }
        internal static void Create()
        {

            Configs.File.IniWriteValue("Config", "Developer", "false");
            Configs.File.IniWriteValue("Config", "Logging", "true");

            Configs.File.IniWriteValue("Security", "Console", "true");
            Configs.File.IniWriteValue("Security", "Password", randomString(8));
            Configs.File.IniWriteValue("Security", "Confirmation", "false");                      

        }
        internal static void Clear()
        {
            Locations.MapLocations = null;
            Whitelists.WhitelistedSteamIDs = null;
            Homes.playerHomes = null;
            Kits.PlayerKits = null;
            Announces.Messages = null;
        }

        internal static void GetCommands()
        {
            CommandList.add(new Command(PermissionLevel.Admin.ToInt(), Refresh, "refresh", "rst"));
        }

        #region Commands

        internal static void Refresh(CommandArgs args)
        {
            Clear();
            Load();
            NetworkChat.sendAlert("Config was reloaded.");
        }

        #endregion

        #region Private calls

        private static System.Random random = new System.Random((int)DateTime.Now.Ticks);
        private static string randomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }

        #endregion

    }
}
