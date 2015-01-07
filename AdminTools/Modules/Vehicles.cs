using CommandHandler;
using System;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

namespace Unturned
{
    internal class Vehicles : Module
    {

        #region TOP: global variables are initialized here

        private static Timer Timer;
        internal static int Interval = 600;
        internal static bool UseRespawnVehicles = false;

        #endregion       

        internal override void Load()
        {
            if (String.IsNullOrEmpty(Configs.File.IniReadValue("Config", "UseRespawnVehicles")))
            {
                Configs.File.IniWriteValue("Config", "UseRespawnVehicles", "false");
                Configs.File.IniWriteValue("Timers", "RespawnVehicles", "1350");
            }

            Vehicles.UseRespawnVehicles = Boolean.Parse(Configs.File.IniReadValue("Config", "UseRespawnVehicles"));
            Vehicles.Interval = Int32.Parse(Configs.File.IniReadValue("Timers", "RespawnVehicles"));

            if (Vehicles.UseRespawnVehicles)
            {

                if (Timer == null)
                {
                    Timer = new Timer(Interval * 1000);
                    Timer.Elapsed += vehiclesTimer_Elapsed;
                    Timer.Enabled = true;
                }

            }
            
        }

        internal override IEnumerable<Command> GetCommands()
        {
            List<Command> _return = new List<Command>();
            _return.Add(new Command(PermissionLevel.Moderator.ToInt(), Spawn, "car", "v"));
            _return.Add(new Command(PermissionLevel.Moderator.ToInt(), Respawn, "respawnvehicles", "vs"));
            _return.Add(new Command(PermissionLevel.Moderator.ToInt(), Repair, "repairvehicles", "vr", "repair"));
            _return.Add(new Command(PermissionLevel.SuperUser.ToInt(), Refuel, "refuelvehicles", "vf", "refuel"));
            _return.Add(new Command(PermissionLevel.Admin.ToInt(), Destroy, "destroyvehicles", "vd"));
            _return.Add(new Command(PermissionLevel.Level1.ToInt(), Sirens, "sirens")); // Use /sirens <[on,off]>
            return _return;
        }
        internal override String GetHelp()
        {
            return null;
        }

        #region Commands

        internal static void Destroy(CommandArgs args)
        {
            int vehicles = UnityEngine.Object.FindObjectsOfType(typeof(Vehicle)).Length;
            destroy();
            NetworkChat.sendAlert(String.Format("{0} has destroyed {1} vehicles.", args.sender.name, vehicles));
        }

        internal static void Repair(CommandArgs args)
        {
            Vehicle[] vehicles = UnityEngine.Object.FindObjectsOfType(typeof(Vehicle)) as Vehicle[];
            foreach (Vehicle vehicle in vehicles)
            {
                vehicle.networkView.RPC("tellExploded", RPCMode.All, new object[] { false });
                vehicle.networkView.RPC("tellWrecked", RPCMode.All, new object[] { false });
                vehicle.heal(1000);
            }
            NetworkChat.sendAlert(String.Format("{0} has repaired {1} vehicles.", args.sender.name, vehicles.Length));
        }
        internal static void Refuel(CommandArgs args)
        {
            Vehicle[] vehicles = UnityEngine.Object.FindObjectsOfType(typeof(Vehicle)) as Vehicle[];
            foreach (Vehicle vehicle in vehicles)
            {
                vehicle.fill(1000);
            }
            NetworkChat.sendAlert(string.Format("{0} has refueled {1} vehicles.", args.sender.name, vehicles.Length));
        }
        
        internal static void Respawn(CommandArgs args)
        {
            
            Reference.Tell(args.sender.networkPlayer, String.Format("Re-spawning {0} vehicles in 3 seconds...", Loot.getCars()));
            respawn();

        }
        internal static void Spawn(CommandArgs args)
        {
            Vector3 location = args.sender.position;
            Quaternion rotation = args.sender.rotation;
            Vector3 newPos = new Vector3(location[0] + 5, location[1] + 50, location[2]);

            if (args.Parameters.Count == 0) //Spawn random vehicle
            {
                Vehicle[] mapVehicles = UnityEngine.Object.FindObjectsOfType(typeof(Vehicle)) as Vehicle[];

                int random = UnityEngine.Random.Range(0, mapVehicles.Length);
                Vehicle randomVehicle = mapVehicles[random];

                randomVehicle.updatePosition(newPos, rotation);
                randomVehicle.transform.position = newPos;
            }

            else //create vehicle and destroy another one
            {
                String cartype = args.Parameters[0];
                if (!cartype.Contains("_"))
                {
                    cartype += "_0";
                }
                GameObject model = (GameObject)typeof(SpawnVehicles).GetFields()[0].GetValue(null);

                try
                {//destroy random car.. lol
                    if (model.transform.FindChild("models").childCount >= Loot.getCars() - 1)
                    {
                        int number = UnityEngine.Random.Range(0, Loot.getCars());
                        Transform child = model.transform.FindChild("models").GetChild(number);
                        Network.RemoveRPCs(child.networkView.viewID);
                        Network.Destroy(child.networkView.viewID);
                    }
                }
                catch { }
                System.Threading.Timer timer;
                timer = new System.Threading.Timer(obj =>
                {
                    SpawnVehicles.create(cartype, 100, 100, newPos, rotation * Quaternion.Euler(-90f, 0f, 0f), new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f)));
                    SpawnVehicles.save();
                }, null, 400, System.Threading.Timeout.Infinite);

                Reference.Tell(args.sender.networkPlayer, String.Format("Creating {0}.", cartype));

            }
        }

        internal static void Sirens(CommandArgs args)
        {
            bool val = args.Parameters[1].ToLower() == "on";
            Vehicle[] vehicles = UnityEngine.Object.FindObjectsOfType(typeof(Vehicle)) as Vehicle[];
            foreach (Vehicle vehicle in vehicles)
            {
                vehicle.networkView.RPC("tellSirens", RPCMode.All, new object[] { val });
            }
        }

        #endregion

        #region Private calls

        private void vehiclesTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            respawn();
        }

        private static void respawn()
        {
            GameObject model = (GameObject)typeof(SpawnVehicles).GetFields()[0].GetValue(null);

            try   //Destroy all cars
            {
                destroy();

                int amount = model.transform.FindChild("models").childCount;
                for (int i = 0; i < amount; i++)
                {
                    Transform child = model.transform.FindChild("models").GetChild(i);
                    Network.RemoveRPCs(child.networkView.viewID);
                    Network.Destroy(child.networkView.viewID);
                }
            }
            catch { }

            SpawnVehicles.save();
            
            System.Threading.Timer timer;
            timer = new System.Threading.Timer(obj =>
            {
                UseRespawnVehicles = true;
            }, null, 3000, System.Threading.Timeout.Infinite);
        }
        private static void destroy()
        {
            Vehicle[] vehicles = UnityEngine.Object.FindObjectsOfType(typeof(Vehicle)) as Vehicle[];
            foreach (Vehicle vehicle in vehicles)
            {
                vehicle.networkView.RPC("tellExploded", RPCMode.All, new object[] { false });
                vehicle.networkView.RPC("tellWrecked", RPCMode.All, new object[] { false });
                vehicle.damage(1000);
            }
        }

        #endregion

    }
}
