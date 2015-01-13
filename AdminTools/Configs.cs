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

#if DEBUG
            Configs.Developer = true;
            Configs.Logging = true;
            Configs.Console = true;
            Save();
#endif

            Strings.Load();

            Directory.CreateDirectory(AdminTools.AdminPath);
            if (!System.IO.File.Exists(Configs.Source)) { Save(); }          

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
