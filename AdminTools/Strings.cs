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
            Strings.File.IniWriteValue("HLP", "AnnoyingLagHelp", "[lag,lg] -> Lag a player making it disable to move or chat.");
            Strings.File.IniWriteValue("HLP", "AnnoyingUnLagHelp", "[unlag,ulg] -> Unlag a player who was on lag command.");
            Strings.File.IniWriteValue("HLP", "AnnoyingHornHelp", "[horn,h] -> Play the sound of a horn.");
            Strings.File.IniWriteValue("HLP", "AnnoyingVanishHelp", "[vanish,vis] -> Make a player invisible / visible.");
            Strings.File.IniWriteValue("HLP", "AnnoyingGodHelp", "[god] -> God mode.");
                        
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
            Strings.File.IniWriteValue("HLP", "AutoSavesSaveHelp", "[save,saveserver] -> Save the server for your self.");
            
            Strings.File.IniWriteValue("MOD", "BansUnBanDone", "{0} was removed from the ban list.");
            Strings.File.IniWriteValue("MOD", "BansUnBanFailed", "Could not find {0} in the list of banned players.");
            Strings.File.IniWriteValue("HLP", "BansBanReasonHelp", "[reason,rban] -> Reason for banning {0} ?  /reason <reason> to ban.");
            Strings.File.IniWriteValue("HLP", "BansReloadBansHelp", "[reloadbans,lban] -> Reloads bans.");
            Strings.File.IniWriteValue("HLP", "BansBansHelp", "[ban] -> Ban a player.");
            Strings.File.IniWriteValue("HLP", "BansUnBansHelp", "[unban,uban] -> Unban a player.");

            Strings.File.IniWriteValue("MOD", "BasicsOnlinePlayers", "There are {0} players online.");
            Strings.File.IniWriteValue("MOD", "BasicsRealTime", "The time is {0}");
            Strings.File.IniWriteValue("MOD", "BasicsRealTime", "The real time is {0}");
            Strings.File.IniWriteValue("HLP", "BasicsClearChat", "[clear,cls] -> clear chat.");
            Strings.File.IniWriteValue("HLP", "BasicsOnlinePlayerHelp", "[online,players,who] -> Show a list of online players.");
            Strings.File.IniWriteValue("HLP", "BasicsGameTimeHelp", "[time] -> Display the game time.");
            Strings.File.IniWriteValue("HLP", "BasicsRealTimeHelp", "[now] -> Display the realworld time.");
            Strings.File.IniWriteValue("HLP", "BasicsRestartHelp", "[restart] -> Restart the server.");
            Strings.File.IniWriteValue("HLP", "BasicsAboutHelp", "[about] -> About.");

            Strings.File.IniWriteValue("MOD", "FreezesFreezes", "{0} players was frozen.");
            Strings.File.IniWriteValue("MOD", "FreezesUnFreezes", "{0} players was unfrozen.");
            Strings.File.IniWriteValue("HLP", "FreezeFreezeHelp", "[freeze,pf] -> Freeze all players online.");
            Strings.File.IniWriteValue("HLP", "FreezeUnFreezeHelp", "[unfreeze,puf] -> Unfreeze all players online.");

            Strings.File.IniWriteValue("HLP", "HomesSetHomesHelp", "[sethome,sh] -> Set a home.");
            Strings.File.IniWriteValue("HLP", "HomesHomeHelp", "[home, h] -> Teleport you to your home.");

            Strings.File.IniWriteValue("MOD", "ItemsReset", "{0} has respawned all items.");
            Strings.File.IniWriteValue("HLP", "ItemsSpawn", "[spawn,i,get] -> Spawn a item by id.");
            Strings.File.IniWriteValue("HLP", "ItemsSpawn", "[resetitems,ir] -> Respawn and reset all items on server.");
            Strings.File.IniWriteValue("HLP", "ItemsDelay", "[setitemsdelay] -> Change the delay on spawn items.");

            Strings.File.IniWriteValue("HLP", "KicksKick", "[kick, k] -> Kick a player.");

            Strings.File.IniWriteValue("MOD", "KitsUserKits", "You can use those kits: {0}. Use /kit <name>");
            Strings.File.IniWriteValue("MOD", "KitsNotSet", "You don't have kits yet");
            Strings.File.IniWriteValue("MOD", "KitsSet", "Kit '{0}' was set.");
            Strings.File.IniWriteValue("MOD", "KitsDelete", "Kit '{0}' was deleted.");
            Strings.File.IniWriteValue("MOD", "KitsAdd", "'{0}' was added to kit '{1}'.");
            Strings.File.IniWriteValue("HLP", "KitsList", "[getkits, ks, kits] -> Display a list of created kits.");
            Strings.File.IniWriteValue("HLP", "KitsGet", "[kit,k] -> Give you the desiree kit.");
            Strings.File.IniWriteValue("HLP", "KitsAdd", "[addkit,ka] -> Add items to a coustom kit.");
            Strings.File.IniWriteValue("HLP", "KitsSet", "[setkit,ks] -> Set a coustom kit.");

            Strings.File.IniWriteValue("MOD", "LocationsGet", "X:{0}, Y:{1}, Z:{2}");
            Strings.File.IniWriteValue("MOD", "LocationsLocationSave", "Locations saved.");
            Strings.File.IniWriteValue("HLP", "LocationsGetPublic", "[here,h] -> Tells to everyone the right place where you are.");
            Strings.File.IniWriteValue("HLP", "LocationsGetPrivate", "[where,w] -> Tells to you the location that you are");
            Strings.File.IniWriteValue("HLP", "LocationsGetGetExternal", "[at,@] -> Tell you the position and location of a player");
            // ----> Dev's Commands Be careful with /saveloc /sloc
            Strings.File.IniWriteValue("HLP", "LocationsGet", "[position,p] -> Give the X Y Z");
            Strings.File.IniWriteValue("HLP", "LocationsSet", "[saveloc,sloc] -> Save the location in the config file");







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
