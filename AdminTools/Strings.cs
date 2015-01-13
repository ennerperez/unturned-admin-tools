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

            Strings.File.IniWriteValue("MOD", "PlayerNotFound", "Player was not found.");
            Strings.File.IniWriteValue("MOD", "PlayerRespawnDefault", "You has been re-spawned.");
            Strings.File.IniWriteValue("MOD", "PlayersRespawn", "{0} has re-spawned {0} payers.");
            Strings.File.IniWriteValue("MOD", "PlayerSkillDefault", "You has been blessed, you learned {0} skill points.");
            Strings.File.IniWriteValue("MOD", "PlayerSkilled", "You skilled {0}.");
            Strings.File.IniWriteValue("MOD", "PlayersBlessed", "Players has been blessed, {0} players was learned {1} skill points.");
            Strings.File.IniWriteValue("MOD", "PlayerKillDefault", "You died of retardedness.");
            Strings.File.IniWriteValue("MOD", "PlayerKilled", "You killed {0}.");
            Strings.File.IniWriteValue("MOD", "PlayersGenocide", "This was a genocide, {0} players was killed.");
            Strings.File.IniWriteValue("MOD", "PlayersSacrifice", "This was a sacrifice, {0} players was killed.");
            Strings.File.IniWriteValue("MOD", "PlayerHealDefault", "Lord Jesus Christ heal you body and soul.");
            Strings.File.IniWriteValue("MOD", "PlayerHealed", "You healed {0}.");
            Strings.File.IniWriteValue("MOD", "PlayersMiracle", "This was a miracle!, {0} players was healed.");
            Strings.File.IniWriteValue("MOD", "PlayerPromoted", "You have been promoted to level {0}.");
            Strings.File.IniWriteValue("MOD", "PlayerDemoted", "You have been demoted to level {0}.");
            Strings.File.IniWriteValue("MOD", "PlayersPromote", "{0} has been promoted to level {1}");
            Strings.File.IniWriteValue("MOD", "PlayersDemote", "{0} has been demoted to level {1}");
            Strings.File.IniWriteValue("HLP", "PlayersRespawnHelp", "[respawn,pr] <[playername,all,others]> -> Re-spawn specific player, all or only others.");
            Strings.File.IniWriteValue("HLP", "PlayersSkillHelp", "[skill,ps] <[playername,all,others]> <[amount]> -> Learn experience points to specific player, all or only others.");
            Strings.File.IniWriteValue("HLP", "PlayersKillHelp", "[kill,pk] <[playername,all,others]> -> Kill specific player, all or only others.");
            Strings.File.IniWriteValue("HLP", "PlayersHealHelp", "[heal,ph] <[playername,all,others]> -> Heal specific player, all or only others.");
            Strings.File.IniWriteValue("HLP", "PlayersPromoteHelp","[promote] <playername> -> Promote player level.");
            Strings.File.IniWriteValue("HLP", "PlayersDemoteHelp", "[demote] <playername> -> Demote player level.");

            Strings.File.IniWriteValue("MOD", "WarComing", "War is coming...");
            Strings.File.IniWriteValue("MOD", "WarCount", "War start in {0}.");
            Strings.File.IniWriteValue("MOD", "WarStart", "This is Sparta!.");
            Strings.File.IniWriteValue("MOD", "WarOver", "War is over.");
            Strings.File.IniWriteValue("HLP", "WarHelp", "[war] <[ninja,green]> -> Unturned war mode.");

            Strings.File.IniWriteValue("HLP", "TeleportsToHelp", "[tpcoord,tc,tpto] <x> <y> <[z]> -> Teleports to coordinate based point.");
            Strings.File.IniWriteValue("HLP", "TeleportsToPlayerHelp", "[tplayer,tp] <playername> -> Teleports to player location.");
            Strings.File.IniWriteValue("HLP", "TeleportsToMeHelp", "[tptome,tm] <playername> -> Teleports player to you location.");
            Strings.File.IniWriteValue("HLP", "TeleportsToMeAllHelp", "[tpall,ta] -> Teleports all players to you location.");

            Strings.File.IniWriteValue("MOD", "Vehicle", "Creating {0}.");
            Strings.File.IniWriteValue("MOD", "VehiclesRespawn", "Re-spawning {0} vehicles in 3 seconds...");
            Strings.File.IniWriteValue("MOD", "VehiclesRepairs", "{0} has repaired {1} vehicles.");
            Strings.File.IniWriteValue("MOD", "VehiclesRefuel", "{0} has refueled {1} vehicles.");
            Strings.File.IniWriteValue("MOD", "VehiclesDestroy", "{0} has destroyed {1} vehicles.");
            Strings.File.IniWriteValue("HLP", "Vehicle", "[vehicle,car,v] <[cartype]> -> Spawn a vehicle nearby you location.");
            Strings.File.IniWriteValue("HLP", "VehiclesRespawn", "[respawnvehicles,vs] -> Re-spawn vehicles.");
            Strings.File.IniWriteValue("HLP", "VehiclesRepair", "[repairvehicles,vr,repair] -> Repair vehicles.");
            Strings.File.IniWriteValue("HLP", "VehiclesRefuel", "[refuelvehicles,vf,refuel] -> Refuel vehicles.");
            Strings.File.IniWriteValue("HLP", "VehiclesDestroy", "[destroyvehicles,vd] -> Destroy vehicles.");
            Strings.File.IniWriteValue("HLP", "VehiclesSirens", "[sirens] <[on,off]> -> Turn on/off vehicles sirens.");
                        
            Strings.File.IniWriteValue("MOD", "WhitelistOn", "Whitelist enabled.");
            Strings.File.IniWriteValue("MOD", "WhitelistOff", "Whitelist disabled.");
            Strings.File.IniWriteValue("MOD", "WhitelistPlayerNotFound", "That player is either not on-line or you provided a bad SteamID.");
            Strings.File.IniWriteValue("MOD", "WhitelistAdded", "Added {0} ({1}) to the whitelist.");
            Strings.File.IniWriteValue("MOD", "WhitelistRemoved", "Removed {0} ({1}) from the whitelist.");
            Strings.File.IniWriteValue("MOD", "WhitelistNotWhitelisted", "You are not whitelisted on this server!");
            Strings.File.IniWriteValue("HLP", "WhitelistHelp", "[whitelist] [on,off,add,del] <playername> ->Turn on/off and add/delete players from whitelist.");

            Strings.File.IniWriteValue("MOD", "ZombiesRespawn", "{0} has re-spawned all zombies.");
            Strings.File.IniWriteValue("MOD", "ZombiesKills", "{0} has killed {1} zombies.");
            Strings.File.IniWriteValue("MOD", "ZombieLand", "{0} opened the hell's gates.");
            Strings.File.IniWriteValue("HLP", "ZombieHelp", "[zombie,z] -> Spawn a random zombie.");
            Strings.File.IniWriteValue("HLP", "ZombiesRespawnHelp", "[respawnzombies,zr] -> Re-spawn zombies.");
            Strings.File.IniWriteValue("HLP", "ZombiesKillsHelp", "[killzombies,zk] -> Kills zombies.");
            Strings.File.IniWriteValue("HLP", "ZombieLandHelp", "[zombieland,zl] <[#]> -> Spawn an amount of zombies nearby every player. By default 20.");

        }

        public static string Get(string section, string key)
        {
            return File.IniReadValue(section, key);
        }

    }
}
