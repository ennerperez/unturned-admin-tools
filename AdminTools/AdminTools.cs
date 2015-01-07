using CommandHandler;
using Ini;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Unturned
{
    public partial class AdminTools : MonoBehaviour
    {
        #region TOP: global variables are initialized here

        public const string Path = "Unturned_Data/Managed/mods/Admin";
        //public string lastUsedCommand = "none";

        #endregion
        
        public void Start()
        {

            // Commands are being added here.
            // the 0's you see mean that these commands are usable by ALL players

            // A higher number means you need a higher permission rank to do said command
            // Individual player permission levels can be found in /mods/UserPermissionLevels.ini

            Configs.ReadConfigs();

            List<Module> modules = new List<Module>();
            foreach (Type type in Assembly.GetAssembly(typeof(Module)).GetTypes())
            {
                if (type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(Module)))
                {
                    modules.Add((Module)Activator.CreateInstance(type, null));
                }              
            }
            modules.Sort();

            foreach (Module item in modules)
            {
                item.GetCommands();
                item.Load();
            }
            
            ////List<Module> modules;
            ////modules = System.Reflection.ReflectiveEnumerator.GetEnumerableOfType<Module>();
            
            ////foreach (Module item in .GetEnumerator())
            ////{

            ////}

            ////foreach (Module item in modules)
            ////{
            ////    item.a
            ////}

            //// Basic commands
            //Basics.GetCommands();

            //// Location commands
            //Locations.GetCommands();

            //// Home commands
            //Homes.GetCommands();

            //// Teleport commands
            //Teleports.GetCommands();

            //// Ban, Kick, Whitelist commands
            //Bans.GetCommands();
            //Kicks.GetCommands();
            //Whitelists.GetCommands();

            //// General moderation commands
            //Announces.GetCommands();

            //// Items & Kits commands
            //Items.GetCommands();
            //Kits.GetCommands();

            //// Vehicles commands
            //Vehicles.GetCommands();

            //// Zombies commands
            //Zombies.GetCommands();

            //// Animals commands
            //Animals.GetCommands();

            //// Players commands
            //Players.GetCommands();
            //Freezes.GetCommands();
            //Annoying.GetCommands();

            //// System commands
            //Configs.GetCommands();

            //// Gameplay commands
            //Specials.GetCommands();

            //Configs.ReadConfigs();

        }

        internal static void Log(Exception p)
        {
            string logFile = System.IO.Path.Combine(Path, "errors.log");
            System.IO.StreamWriter file = new StreamWriter(logFile, true);
            file.WriteLine(p.ToString());
            file.Close();
        }

        public void Update()
        {
            Whitelists.KickNonWhitelistedPlayers();

            if (Vehicles.RespawnVehicles)
            {
                Vehicles.RespawnVehicles = false;
                SpawnVehicles spawnveh = UnityEngine.Object.FindObjectOfType<SpawnVehicles>();
                spawnveh.onReady();
            }
            if (Freezes.frozenPlayers.Count > 0)
            {
                foreach (KeyValuePair<String, Vector3> entry in Freezes.frozenPlayers)
                {
                    BetterNetworkUser user = UserList.getUserFromSteamID(entry.Key);
                    if (user != null)
                    {
                        user.player.transform.position = entry.Value;

                        user.player.networkView.RPC("tellStatePosition", user.player.networkView.owner, new object[] { entry.Value, user.player.transform.rotation });
                    }

                }
            }
            if (Annoying.VanishedPlayers.Count > 0)
            {
                foreach (KeyValuePair<String, Vector3> entry in Annoying.VanishedPlayers)
                {
                    BetterNetworkUser user = UserList.getUserFromSteamID(entry.Key);
                    if (user != null)
                    {
                        foreach (NetworkPlayer networkplayer in Network.connections)
                        {
                            if (Network.player != networkplayer && networkplayer != user.networkPlayer)
                            {
                                user.player.networkView.RPC("tellStatePosition", networkplayer, new object[] { new Vector3(0, 0, 0), user.rotation });

                            }
                        }

                    }

                }
            }
        }

        public void OnGUI()
        {
            
                List<string> console = new List<string>();
                console.Add(" ");
                console.Add("Welcome back to Unturned Server!");
                //console.Add(Unturned.Strings.Welcome);
                                
                console.Add(" ");

                foreach (BetterNetworkUser item in UserList.users)
                {
                    if (!string.IsNullOrEmpty(item.networkPlayer.ipAddress))
                    {
                        console.Add(string.Format("{0}    {1}     {2}     {3}",
                        item.steamid, item.reputation, item.networkPlayer.ipAddress, item.name));
                    }
                }

                GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));

                foreach (string item in console)
                {
                    GUILayout.Label(item);
                }
                GUILayout.EndArea();
            
        }

    }

}