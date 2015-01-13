using CommandHandler;
using System;
using System.Collections.Generic;
using System.Text;
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

        internal override void Save()
        {
            Configs.File.IniWriteValue("Modules", "RespawnVehicles", (UseRespawnVehicles) ? "true" : "false");
            Configs.File.IniWriteValue("Timers", "RespawnVehicles", Interval.ToString());
        }
        internal override void Load()
        {
            if (String.IsNullOrEmpty(Configs.File.IniReadValue("Modules", "RespawnVehicles")))
            {
                this.Save();
            }

            Vehicles.UseRespawnVehicles = Boolean.Parse(Configs.File.IniReadValue("Modules", "RespawnVehicles"));
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
        internal override void Refresh()
        {

            if (Vehicles.UseRespawnVehicles)
            {
                Vehicles.UseRespawnVehicles = false;
                SpawnVehicles spawnveh = UnityEngine.Object.FindObjectOfType<SpawnVehicles>();
                spawnveh.onReady();
            }

        }

        internal override IEnumerable<Command> GetCommands()
        {
            List<Command> _return = new List<Command>();
            _return.Add(new Command(PermissionLevel.Moderator.ToInt(), Spawn, "vehicle", "car", "v"));
            _return.Add(new Command(PermissionLevel.Moderator.ToInt(), Respawn, "respawnvehicles", "vs"));
            _return.Add(new Command(PermissionLevel.Moderator.ToInt(), Repair, "repairvehicles", "vr", "repair"));
            _return.Add(new Command(PermissionLevel.SuperUser.ToInt(), Refuel, "refuelvehicles", "vf", "refuel"));
            _return.Add(new Command(PermissionLevel.Admin.ToInt(), Destroy, "destroyvehicles", "vd"));
            _return.Add(new Command(PermissionLevel.Level1.ToInt(), Sirens, "sirens"));
            return _return;
        }
        internal override String GetHelp()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(Strings.Get("HLP", "Vehicle"));
            sb.AppendLine(Strings.Get("HLP", "VehiclesRespawn"));
            sb.AppendLine(Strings.Get("HLP", "VehiclesRepair"));
            sb.AppendLine(Strings.Get("HLP", "VehiclesRefuel"));
            sb.AppendLine(Strings.Get("HLP", "VehiclesDestroy"));
            sb.AppendLine(Strings.Get("HLP", "VehiclesSirens"));

            return sb.ToString();
        }

        #region Commands

        internal static void Spawn(CommandArgs args)
        {
            Vector3 location = args.sender.position;
            Quaternion rotation = args.sender.rotation;
            Vector3 newPos = new Vector3(location[0] + 5, location[1] + 50, location[2]);

            if (args.Parameters.Count == 0)
            {
                spawn(newPos, rotation, (args.Parameters.Count == 0));
            }
            else
            {
                String cartype = args.Parameters[0].Trim();
                spawn(newPos, rotation, false, cartype);
                Reference.Tell(args.sender.networkPlayer, String.Format(Strings.Get("MOD", "Vehicle"), cartype));
            }
        }
        internal static void Respawn(CommandArgs args)
        {
            NetworkChat.sendAlert(String.Format(Strings.Get("MOD", "VehiclesRespawn"), Loot.getCars()));
            respawn();
        }
        internal static void Repair(CommandArgs args)
        {
            int repairs = repair();
            NetworkChat.sendAlert(String.Format(Strings.Get("MOD", "VehiclesRepairs"), args.sender.name, repairs));
        }
        internal static void Refuel(CommandArgs args)
        {
            int refuels = refuel();
            NetworkChat.sendAlert(String.Format(Strings.Get("MOD", "VehiclesRefuel"), args.sender.name, refuels));
        }
        internal static void Destroy(CommandArgs args)
        {
            int kills = destroy();
            NetworkChat.sendAlert(String.Format(Strings.Get("MOD", "VehiclesDestroy"), args.sender.name, kills));
        }

        internal static void Sirens(CommandArgs args)
        {
            if (args.Parameters.Count > 0)
            {
                switch (args.Parameters[0].ToLower())
                {
                    case "on":
                        sirens(true);
                        break;
                    case "off":
                        sirens(false);
                        break;
                    default:
                        break;
                }

                
            }
        }

        #endregion

        private void vehiclesTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            respawn();
        }

        private static void spawn(Vector3 pos, Quaternion rot, bool rand = false, string typ = null)
        {
            try
            {

                if (rand)
                {
                    Vehicle[] mapVehicles = UnityEngine.Object.FindObjectsOfType(typeof(Vehicle)) as Vehicle[];

                    int random = UnityEngine.Random.Range(0, mapVehicles.Length);
                    Vehicle randomVehicle = mapVehicles[random];

                    randomVehicle.updatePosition(pos, rot);
                    randomVehicle.transform.position = pos;
                }
                else
                {

                    if (!typ.Contains("_")) { typ += "_0"; }
                    GameObject model = (GameObject)typeof(SpawnVehicles).GetFields()[0].GetValue(null);

                    try
                    {
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
                        SpawnVehicles.create(typ, 100, 100, pos, rot * Quaternion.Euler(-90f, 0f, 0f), new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f)));
                        SpawnVehicles.save();
                    }, null, 400, System.Threading.Timeout.Infinite);

                }


            }
            catch (Exception ex)
            {
                Shared.Log(ex.Message);
            }

        }
        private static void respawn()
        {
            try
            {
                GameObject model = (GameObject)typeof(SpawnVehicles).GetFields()[0].GetValue(null);

                try
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
            catch (Exception ex)
            {
                Shared.Log(ex.Message);
            }
        }
        private static int repair()
        {
            try
            {
                Vehicle[] mapVehicles = UnityEngine.Object.FindObjectsOfType(typeof(Vehicle)) as Vehicle[];
                int counter = 0;
                foreach (Vehicle vehicle in mapVehicles)
                {
                    vehicle.networkView.RPC("tellExploded", RPCMode.All, new object[] { false });
                    vehicle.networkView.RPC("tellWrecked", RPCMode.All, new object[] { false });
                    vehicle.heal(1000);
                    counter++;
                }
                return counter;
            }
            catch (Exception ex)
            {
                Shared.Log(ex.Message);
                return 0;
            }
        }
        private static int refuel()
        {
            try
            {
                Vehicle[] mapVehicles = UnityEngine.Object.FindObjectsOfType(typeof(Vehicle)) as Vehicle[];
                int counter = 0;
                foreach (Vehicle vehicle in mapVehicles)
                {
                    vehicle.fill(1000);
                    counter++;
                }
                return counter;
            }
            catch (Exception ex)
            {
                Shared.Log(ex.Message);
                return 0;
            }
        }
        private static int destroy()
        {
            try
            {
                Vehicle[] vehicles = UnityEngine.Object.FindObjectsOfType(typeof(Vehicle)) as Vehicle[];
                int counter = 0;
                foreach (Vehicle vehicle in vehicles)
                {
                    vehicle.networkView.RPC("tellExploded", RPCMode.All, new object[] { false });
                    vehicle.networkView.RPC("tellWrecked", RPCMode.All, new object[] { false });
                    vehicle.damage(1000);
                    counter++;
                }
                return counter;
            }
            catch (Exception ex)
            {
                Shared.Log(ex.Message);
                return 0;
            }
        }

        private static void sirens(bool val)
        {
            try
            {
                Vehicle[] vehicles = UnityEngine.Object.FindObjectsOfType(typeof(Vehicle)) as Vehicle[];
                foreach (Vehicle vehicle in vehicles)
                {
                    vehicle.networkView.RPC("tellSirens", RPCMode.All, new object[] { val });
                }
            }
            catch (Exception ex)
            {
                Shared.Log(ex.Message);
            }
        }

    }
}
