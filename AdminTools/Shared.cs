using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Unturned
{
    internal static class Shared
    {

        //internal static void Log(string p)
        //{
        //    string logFile = System.IO.Path.Combine(AdminTools.Path, "autosave.log");
        //    System.IO.StreamWriter file = new StreamWriter(logFile, true);
        //    file.WriteLine(String.Format("{0} - {1}", DateTime.Now.ToString(), p.ToString()));
        //    file.Close();
        //    file.Dispose();
        //}

        internal static void Log(string p)
        {
            if (Configs.Logging)
            {
                string dir = System.IO.Path.Combine(AdminTools.Path, "logs");
                System.IO.Directory.CreateDirectory(dir);

                string logFile = System.IO.Path.Combine(dir, String.Format("{0}.log", DateTime.Now.Date.Ticks.ToString()));
                System.IO.StreamWriter file = new StreamWriter(logFile, true);
                file.WriteLine(String.Format("{0} - {1}", DateTime.Now.ToString(), p.ToString()));
                file.Close();
                file.Dispose();
            }
        }

    }
}
