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
    public class AdminTools : MonoBehaviour
    {

        #region TOP: global variables are initialized here

        internal const string Path = "Unturned_Data/Managed/mods/Admin";
        internal static List<Module> Modules = new List<Module>();
        //public string lastUsedCommand = "none";

        #endregion

        public void Start()
        {

            Configs.Load();
            try
            {

                foreach (Type type in Assembly.GetAssembly(typeof(Module)).GetTypes())
                {
                    if (type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(Module)))
                    {
                        Modules.Add((Module)Activator.CreateInstance(type, null));
                    }
                }
                //Modules.Sort();

                foreach (Module item in Modules)
                {
                    foreach (Command citem in item.GetCommands())
                    {
                        CommandList.add(citem);
                    }
                    item.Load();
                }

            }
            catch (Exception ex)
            {
                Shared.Log(ex.ToString());
            }

            Shared.Log("Server started.");

        }

        public void Update()
        {
            if (Whitelists.UseWhitelist)
            {
                Whitelists.KickNonWhitelistedPlayers();
            }

            if (Vehicles.UseRespawnVehicles)
            {
                Vehicles.UseRespawnVehicles = false;
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