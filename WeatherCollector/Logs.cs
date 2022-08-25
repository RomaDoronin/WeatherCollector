using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherCollector
{
    internal class Logs
    {
        private static readonly string path = "logs.txt";

        public static void WriteLine(string text)
        {
            File.AppendAllText(path, text + '\n');
        }

        public static void ClearAll()
        {
            if (File.Exists(path))
            {
                File.WriteAllText(path, string.Empty);
            }
        }

        public static void WriteMetaData()
        {
            if (!File.Exists(path))
            {
                File.Create(path);
            }
            WriteLine("====================================");
            WriteLine(DateTime.Now.ToString());
            WriteLine("====================================");
        }
    }
}
