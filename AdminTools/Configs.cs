using CommandHandler;
using Ini;
using System;
using System.IO;

namespace Unturned
{
    public static class Configs
    {

        #region TOP: global variables are initialized here

        private static Boolean IsLoaded;

        internal static bool Developer = false;
        internal static bool Logging = true;

        internal static bool Console = true;
        internal static bool Confirmation = true;

        private static string Source = System.IO.Path.Combine(AdminTools.AdminPath, "config.ini");
        internal static IniFile File = new IniFile(Configs.Source);

        #endregion

        internal static void Load()
        {

            if (Configs.IsLoaded) { return; }

            Strings.Load();

            Directory.CreateDirectory(AdminTools.AdminPath);
            if (!System.IO.File.Exists(Configs.Source)) { Save(); }


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

            Configs.IsLoaded = true;

        }
        internal static void Save()
        {

            Configs.File.IniWriteValue("Config", "Developer", (Configs.Developer) ? "true" : "false");
            Configs.File.IniWriteValue("Config", "Logging", (Configs.Logging) ? "true" : "false");
            

            Configs.File.IniWriteValue("Security", "Console", (Configs.Console) ? "true" : "false");
            Configs.File.IniWriteValue("Security", "Password", Shared.RandomString(8));
            Configs.File.IniWriteValue("Security", "Confirmation", (Configs.Confirmation) ? "true" : "false");

        }
        internal static void Clear()
        {
            foreach (Module item in AdminTools.Modules)
            {
                item.Clear();
            }
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
            NetworkChat.sendAlert(Strings.Get("CFG", "ConfigsReloaded"));
        }

        #endregion

    }
}
