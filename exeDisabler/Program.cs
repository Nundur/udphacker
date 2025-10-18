using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace exeDisabler
{
    internal class Program
    {
        static void Main(string[] args)
        {

            StringBuilder builder = new StringBuilder(); 
            Console.WriteLine("The following arguments are passed:");

            foreach (var arg in args)
            {
                builder.Append($"{arg}");
            }

            string exeName = "";
            if (!string.IsNullOrEmpty(builder.ToString()))
            {
                exeName = builder.ToString();
            }
            Console.WriteLine(builder.ToString());

            if (exeName != "")
            {
                while (true)
                {
                    if (Process.GetProcessesByName(exeName).Length > 0)
                    {
                        foreach (Process a in Process.GetProcessesByName(exeName))
                        {
                            try
                            {
                                a.Kill();
                            }
                            catch (Exception e)
                            {

                                Console.WriteLine(e);
                            }

                        }
                    }

                }
            }


            


        }
    }
}
