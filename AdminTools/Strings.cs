using Ini;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unturned
{
    public static class Strings
    {

        private static string Source = System.IO.Path.Combine(AdminTools.AdminPath, "strings.ini");
        internal static IniFile File = new IniFile(Strings.Source);

        private static Boolean IsLoaded;

        internal static void Load()
        {

            if (Strings.IsLoaded) { return; }

#if DEBUG
            Save();
#endif

            if (!System.IO.File.Exists(Strings.Source)) { Save(); }

            Strings.IsLoaded = true;

        }
        internal static void Save()
        {

            Strings.File.IniWriteValue("GUI", "WelcomeMessage", "Welcome back to Unturned Server!");
            Strings.File.IniWriteValue("GUI", "RunningVersion", "Running with Admin Tools v{0}");
            Strings.File.IniWriteValue("GUI", "LoadedModules", "[{0} modules loaded]");
            Strings.File.IniWriteValue("LOG", "ServerStarted", "Server started.");
            Strings.File.IniWriteValue("CFG", "ConfigsReloaded", "Configs was reloaded.");
            
            Strings.File.IniWriteValue("MOD", "AnimalsKills", "{0} has killed {1} animals.");
            Strings.File.IniWriteValue("MOD", "AnimalsRespawn", "{0} has re-spawned {1} animals.");
            Strings.File.IniWriteValue("MOD", "AnimalsWild", "{0} opened the zoo's gate.");
            Strings.File.IniWriteValue("HLP", "AnimalHelp", "[animal,a] -> Spawn a random animal.");
            Strings.File.IniWriteValue("HLP", "AnimalsKillsHelp", "[killanimals,ak] -> Kills animals.");
            Strings.File.IniWriteValue("HLP", "AnimalsWildHelp", "[wild,aw] -> Spawn all the animals.");

            Strings.File.IniWriteValue("HLP", "AnnounceHelp", "[repeat,say,announce] <message> -> Send a message to the server.");
            Strings.File.IniWriteValue("HLP", "AnnounceDelayHelp", "[setannouncedelay, adelay] <seconds> -> Set the announcement delay.");








            Strings.File.IniWriteValue("MOD", "AnnoyingGodOn", "{0} is a God.");
            Strings.File.IniWriteValue("MOD", "AnnoyingGodOff", "{0} is mortal again.");

            Strings.File.IniWriteValue("MOD", "AnnoyingVanishOn", "{0} is vanished.");
            Strings.File.IniWriteValue("MOD", "AnnoyingVanishOff", "{0} is visible again.");

            Strings.File.IniWriteValue("MOD", "AnnoyingLag", "You are now lagging the shit out of {0}.");
            Strings.File.IniWriteValue("MOD", "AnnoyingUnLag", "You are no longer lagging the shit out of {0}.");

            Strings.File.IniWriteValue("MOD", "AutoSavesSaving", "Saving world...");
            Strings.File.IniWriteValue("MOD", "AutoSavesFailed", "Auto-save failed to save correctly.");
            Strings.File.IniWriteValue("MOD", "AutoSavesError", "An error happened while saving.");
            Strings.File.IniWriteValue("MOD", "AutoSavesRetrying", "Retrying...");
            Strings.File.IniWriteValue("MOD", "AutoSavesSavingTryManual", "Will try to continue saving manually...");
            Strings.File.IniWriteValue("MOD", "AutoSavesSavedStructures", "Structures saved.");
            Strings.File.IniWriteValue("MOD", "AutoSavesSavedVehicles", "Vehicles saved.");
            Strings.File.IniWriteValue("MOD", "AutoSavesSavedPlayerPrefs", "PlayerPrefs saved.");
            Strings.File.IniWriteValue("MOD", "AutoSavesSaved", "Saved successfully.");
            Strings.File.IniWriteValue("MOD", "AutoSavesDone", "Done.");

            Strings.File.IniWriteValue("MOD", "AutoSavesStuckStructures", "Stuck at getting structure list!");
            Strings.File.IniWriteValue("MOD", "AutoSavesFailedStructures", "Could not get field number {0} of structure number {1}.  Failed to get {2} structures.");
            Strings.File.IniWriteValue("MOD", "AutoSavesCancelling", "Canceling...");
            Strings.File.IniWriteValue("MOD", "AutoSavesSavedManual", "Manually saved {0} structures. ({1} structures failed to save)");

            Strings.File.IniWriteValue("MOD", "BansUnBanDone", "{0} was removed from the ban list.");
            Strings.File.IniWriteValue("MOD", "BansUnBanFailed", "Could not find {0} in the list of banned players.");
            Strings.File.IniWriteValue("HLP", "BansBanReasonHelp", "Reason for banning {0} ?  /reason <reason> to ban.");

        }

        public static string Get(string section, string key)
        {
            return File.IniReadValue(section, key);
        }

    }
}
