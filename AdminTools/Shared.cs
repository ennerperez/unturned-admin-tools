using System;
using System.IO;
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
                string dir = System.IO.Path.Combine(AdminTools.AdminPath, "logs");
                System.IO.Directory.CreateDirectory(dir);

                string logFile = System.IO.Path.Combine(dir, String.Format("{0}.log", DateTime.Now.Date.Ticks.ToString()));
                System.IO.StreamWriter file = new StreamWriter(logFile, true);
                file.WriteLine(String.Format("{0} - {1}", DateTime.Now.ToString(), p.ToString()));
                file.Close();
                file.Dispose();
            }
        }

        private static System.Random Random = new System.Random((int)DateTime.Now.Ticks);
        internal static String RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * Random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }

    }
}
