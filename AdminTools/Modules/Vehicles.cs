using CommandHandler;
using System;
using System.Timers;
using UnityEngine;

namespace Unturned
{
    public static class Vehicles
    {

        #region TOP: global variables are initialized here

        private static Timer VehiclesTimer;
        internal static int VehiclesInterval = 600;
        internal static bool RespawnVehicles = false;

        #endregion       

        internal static void GetCommands()
        {
            CommandList.add(new Command(PermissionLevel.Moderator.ToInt(), Spawn, "car", "v"));
            CommandList.add(new Command(PermissionLevel.Moderator.ToInt(), Respawn, "respawnvehicles", "vs"));
            CommandList.add(new Command(PermissionLevel.Moderator.ToInt(), Repair, "repairvehicles", "vr", "repair"));
            CommandList.add(new Command(PermissionLevel.SuperUser.ToInt(), Refuel, "refuelvehicles", "vf", "refuel"));
            CommandList.add(new Command(PermissionLevel.Admin.ToInt(), Destroy, "destroyvehicles", "vd"));
            CommandList.add(new Command(PermissionLevel.Level1.ToInt(), Sirens, "sirens")); // Use /sirens <[on,off]>
        }

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
            Reference.Tell(args.sender.networkPlayer, String.Format("Re-spawning {0} vehicles in 3 seconds...", Loot.getCars()));
            
            System.Threading.Timer timer;
            timer = new System.Threading.Timer(obj =>
            {
                RespawnVehicles = true;
            }, null, 3000, System.Threading.Timeout.Infinite);

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
        
        // Basic actions

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

    }
}
