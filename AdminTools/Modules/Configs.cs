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

        internal static bool UsingGUI = true;
        internal static bool UsingDEV = false;

        #endregion

        internal static void GetCommands()
        {
            CommandList.add(new Command(PermissionLevel.Admin.ToInt(), RefreshConfigs, "refresh", "rst"));
        }

        internal static void RefreshConfigs(CommandArgs args)
        {
            ClearConfigs();
            ReadConfigs();
            NetworkChat.sendAlert("Config was reloaded.");
        }

        internal static void ClearConfigs()
        {
            Locations.MapLocations = null;
            Whitelists.WhitelistedSteamIDs = null;
            Homes.playerHomes = null;
            Kits.PlayerKits = null;
            Announces.AnnouncesMessages = null;
        }
        internal static void CreateConfigs()
        {
            string configFile = System.IO.Path.Combine(AdminTools.Path, "config.ini");
            IniFile tempIni = new IniFile(configFile);

            tempIni.IniWriteValue("Config", "Whitelist", "false");
            tempIni.IniWriteValue("Config", "WhitelistKickMessages", "true");
            tempIni.IniWriteValue("Config", "Homes", "false");
            tempIni.IniWriteValue("Config", "Kits", "false");
            tempIni.IniWriteValue("Config", "Locations", "false");
            tempIni.IniWriteValue("Config", "GUI", "true");
            tempIni.IniWriteValue("Config", "Dev", "false");

            tempIni.IniWriteValue("Security", "Console", "true");
            tempIni.IniWriteValue("Security", "Password", randomString(8));
            tempIni.IniWriteValue("Security", "Confirmation", "false");

            tempIni.IniWriteValue("Timers", "RespawnItemInverval", "2700");
            tempIni.IniWriteValue("Timers", "AnnouncesInterval", "600");
        }
        internal static void ReadConfigs()
        {
            Directory.CreateDirectory(AdminTools.Path);

            string configFile = System.IO.Path.Combine(AdminTools.Path, "config.ini");
            if (!File.Exists(configFile)) { CreateConfigs(); }

            IniFile ini = new IniFile(configFile);

            //things that need to be added to existing files
            if (ini.IniReadValue("Config", "WhitelistKickMessages").Equals(""))
            {
                ini.IniWriteValue("Config", "WhitelistKickMessages", "true");
            }
            if (ini.IniReadValue("Security", "Console").Equals(""))
            {
                ini.IniWriteValue("Security", "Console", "true");
            }
            if (ini.IniReadValue("Security", "Password").Equals(""))
            {
                ini.IniWriteValue("Security", "Password", randomString(8));
            }
            if (ini.IniReadValue("Security", "Confirmation").Equals(""))
            {
                ini.IniWriteValue("Security", "Confirmation", "false");
            }

            Whitelists.UsingWhitelist = Boolean.Parse(ini.IniReadValue("Config", "Whitelist"));
            UsingGUI = Boolean.Parse(ini.IniReadValue("Config", "GUI"));
            UsingDEV = Boolean.Parse(ini.IniReadValue("Config", "Dev"));
            Homes.usePlayerHomes = Boolean.Parse(ini.IniReadValue("Config", "Homes"));
            Kits.UsePlayerKits = Boolean.Parse(ini.IniReadValue("Config", "Kits"));
            Locations.UseLocations = Boolean.Parse(ini.IniReadValue("Config", "Locations"));
            Whitelists.ShowWhiteListKickMessages = Boolean.Parse(ini.IniReadValue("Config", "WhitelistKickMessages"));

            Items.itemsResetIntervalInSeconds = Int32.Parse(ini.IniReadValue("Timers", "RespawnItemInverval"));
            Announces.AnnouncesInterval = Int32.Parse(ini.IniReadValue("Timers", "AnnouncesInterval"));

            try
            {
                // Loading section
                Bans.Load();
                Whitelists.Load();

                Items.Load();
                Announces.Load();

                Homes.Load();
                Kits.Load();
                Locations.Load();
            }
            catch (Exception ex)
            {
                AdminTools.Log(ex);
            }

        }

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


    }
}
