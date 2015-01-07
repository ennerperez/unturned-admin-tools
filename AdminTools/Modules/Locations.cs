using CommandHandler;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Unturned
{
    internal class Locations : Module
    {

        #region TOP: global variables are initialized here

        internal static List<Location> MapLocations;
        internal static bool UseLocations = false;

        private static string fileSource = System.IO.Path.Combine(AdminTools.Path, "locations.txt");

        #endregion

        internal override void Load()
        {
            if (String.IsNullOrEmpty(Configs.File.IniReadValue("Config", "UseLocations")))
            {
                Configs.File.IniWriteValue("Config", "UseLocations", "false");
            }

            Locations.UseLocations = Boolean.Parse(Configs.File.IniReadValue("Config", "UseLocations"));

            if (Locations.UseLocations)
            {

                if (!File.Exists(fileSource)) { Create();}

                MapLocations = new List<Location>();
                string[] locations = System.IO.File.ReadAllLines(fileSource);
                foreach (string item in locations)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        try
                        {
                            string[] values = item.Split(',');
                            MapLocations.Add(new Location(values[0], new Vector2(float.Parse(values[1]), float.Parse(values[2])), float.Parse(values[3])));
                        }
                        catch (Exception ex)
                        {
                            Shared.Log(ex.ToString());
                        }
                    }
                }
            }
        }
        internal override void Create()
        {
            System.IO.StreamWriter file = new StreamWriter(fileSource, true);
            file.WriteLine("Cementery,62.90188,720.3353,6");
            file.WriteLine("Lighthouse,741.6324,-820.9641,20");
            file.WriteLine("Radio Anthena,-256.7602,-650.5743,20");
            file.WriteLine("Confederation Bridge,-690.8866,332.1692,60");
            file.WriteLine("Tignish Campground,154.197,692.9236,60");
            file.WriteLine("Kensington Campground,315.047,-507.9257,60");
            file.WriteLine("Summerside Peninsula,788.0297,-446.1592,60");
            file.WriteLine("Taylor Beach,58.82936,2.11446,60");
            file.WriteLine("O'Leary Military Base,-440.9788,594.9431,100");
            file.WriteLine("Burywood,-10.23493,651.5575,100");
            file.WriteLine("Belfast Airport,564.8459,410.3109,100");
            file.WriteLine("St. Peter's Island,-257.092,30.71979,100");
            file.WriteLine("Montague,236.5407,-101.0876,100");
            file.WriteLine("Holman Island,-754.6213,-758.9188,100");
            file.WriteLine("Holman Island,-766.0956,-578.6145,100");
            file.WriteLine("Holman Island,-718.1101,-471.4777,100");
            file.WriteLine("Oultons Isle,204.6362,-824.428,100");
            file.WriteLine("Fernwood Farm,-246.5482,-377.4702,80");
            file.WriteLine("Wiltshire Farm,-425.9268,-563.4899,80");
            file.WriteLine("Courtin Island,829.5908,330.0507,110");
            file.WriteLine("Golf Camp,759.8466,-619.4179,120");
            file.WriteLine("Charlottetown,24.07166,-419.7918,150");
            file.WriteLine("Alberton,-582.9,85.81968,180");
            file.Close();
        }
        internal override void Save()
        {
            File.Delete(fileSource);

            System.IO.StreamWriter file = new StreamWriter(fileSource, true);
            foreach (Location item in MapLocations)
            {
                string locName = item.Name;
                Vector2 pos = item.Point;
                float rad = item.Radius;
                file.WriteLine(String.Format("{0},{1},{2},{3}", locName, pos.x, pos.y, rad));
            }

            file.Close();
        }

        internal override IEnumerable<Command> GetCommands()
        {
            List<Command> _return = new List<Command>();
            _return.Add(new Command(PermissionLevel.All.ToInt(), GetPublic, "here", "h"));
            _return.Add(new Command(PermissionLevel.Moderator.ToInt(), GetPrivate, "where", "w"));
            _return.Add(new Command(PermissionLevel.Admin.ToInt(), GetExternal, "at", "@")); // Use /@ <playername>
            // ---> Dev's commands
            _return.Add(new Command(PermissionLevel.All.ToInt(), Get, "position", "p"));
            _return.Add(new Command(PermissionLevel.Admin.ToInt(), Set, "saveloc", "sloc")); // Use for remove a location /sloc -1
            return _return;
        }
        internal override String GetHelp()
        {
            return null;
        }
        
        #region Commands

        internal static void Get(CommandArgs args)
        {
            Vector3 _point = args.sender.position;
            NetworkChat.sendAlert(String.Format("X:{0}, Y:{1}, Z:{2}", _point.x.ToString(), _point.y.ToString(), _point.z.ToString()));
        }
        internal static void Set(CommandArgs args)
        {
            Vector2 _point = new Vector2(args.sender.position.x, args.sender.position.z);
            string locName = args.Parameters[0];
            float rad = 100;
            if (args.Parameters.Count > 1)
            {
                rad = float.Parse(args.Parameters[1]);
            }

            if (rad == -1)
            {
                List<Location> toremove = new List<Location>();
                foreach (Location item in MapLocations)
                {
                    if (item.Name == locName)
                    {
                        toremove.Add(item);
                    }
                }
                foreach (Location item in toremove)
                {
                    MapLocations.Remove(item);
                }

            }
            else
            {
                MapLocations.Add(new Location(locName, _point, rad));
                if (Configs.Developer) { Get(args); }
            }

            AdminTools.Modules.OfType<Locations>().First().Save();
            Reference.Tell(args.sender.networkPlayer, "Locations saved.");

        }

        internal static void GetPublic(CommandArgs args)
        {
            get(args);
        }
        internal static void GetPrivate(CommandArgs args)
        {
            get(args, 1);
        }
        internal static void GetExternal(CommandArgs args)
        {
            get(args, 2);
        }

        #endregion

        #region Private calls

        internal static void get(CommandArgs args, int mode = 0)
        {

            BetterNetworkUser user;

            if (mode == 2) // && !String.IsNullOrEmpty(args.ParametersAsString))
            {
                string name = args.ParametersAsString;
                user = UserList.getUserFromName(name);
                if (user == null) return;
            }
            else
            {
                user = args.sender;
            }


            Vector2 _point = new Vector2(args.sender.position.x, args.sender.position.z);

            foreach (Location item in MapLocations)
            {
                if (item.GetArea().Contains(_point) == true)
                {

                    switch (mode)
                    {
                        case 1:
                            Reference.Tell(args.sender.networkPlayer, String.Format("You are at {0} right now.", item.Name));
                            break;
                        case 2:
                            Reference.Tell(args.sender.networkPlayer, String.Format("{0} are at {1} right now.", user.name, item.Name));
                            break;
                        default:
                            NetworkChat.sendAlert(String.Format("{0} are at {1} right now.", args.sender.name, item.Name));
                            break;
                    }

                    return;
                }
            }

            Reference.Tell(args.sender.networkPlayer, "You are lost, naked and you will be raped.");
            if (Configs.Developer)
            {
                Inventory inventory = args.sender.player.gameObject.GetComponent<Inventory>();
                if (!inventory.Find(28000).IsXYValid())
                {
                    inventory.tryAddItem(28000, 1);
                }
            }
        }

        #endregion

    }

    #region Structures

    public class Rectangle
    {
        public float X1 { get; set; }
        public float Y1 { get; set; }
        public float X2 { get; set; }
        public float Y2 { get; set; }

        public Rectangle()
        {
        }

        public Rectangle(float x1, float y1, float x2, float y2)
        {
            this.X1 = x1;
            this.Y1 = y1;
            this.X2 = x2;
            this.Y2 = y2;
        }

        public bool Contains(Vector2 p)
        {
            return this.X1 <= p.x && p.x <= this.X2 &&
                this.Y2 <= p.y && p.y <= this.Y1;
        }

    }

    public class Location
    {
        public string Name { get; set; }
        public Vector2 Point { get; set; }
        public float Radius { get; set; }

        public Location()
        { }

        public Location(string name, Vector2 point, float radius = 100)
        {
            this.Name = name;
            this.Point = point;
            this.Radius = radius;
        }

        public Rectangle GetArea()
        {
            Rectangle _return = new Rectangle();//(this.Point.x - this.Radius, this.Point.y + this.Radius, this.Radius * 2, this.Radius * 2);
            _return.X1 = this.Point.x - this.Radius;
            _return.Y1 = this.Point.y + this.Radius;
            _return.X2 = this.Point.x + this.Radius;
            _return.Y2 = this.Point.y - this.Radius;
            return _return;
        }

    }

    #endregion

}
