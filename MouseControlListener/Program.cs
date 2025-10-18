using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
//using System.Windows.Shapes;
using System.Drawing;
using System.Drawing.Imaging;

namespace MouseControlListener
{
    internal class Program
    {

        // Egérműveletek
        [DllImport("user32.dll", SetLastError = true)]
        static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);

        // Egér események flag-jei
        const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        const uint MOUSEEVENTF_LEFTUP = 0x0004;
        const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
        const uint MOUSEEVENTF_RIGHTUP = 0x0010;


        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);
        static string instruct = "getres";
        static async Task Main(string[] args)
        {
            
            int PORT = 6667;
            var udp = new UdpClient();
            udp.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            udp.Client.Bind(new IPEndPoint(IPAddress.Any, PORT));

            udp.EnableBroadcast = true;

            var broadCastEP = new IPEndPoint(IPAddress.Broadcast, PORT);
            byte[] data = Encoding.UTF8.GetBytes($"");
            await udp.SendAsync(data, data.Length, broadCastEP);

            //MessageBox.Show("elindultam");
            
            //_ = KepetCsinalKuld();

            while (true)
            {
                var result = await udp.ReceiveAsync();
                byte[] message = result.Buffer;
                string utasitas = Encoding.UTF8.GetString(message);
                if (utasitas=="off")
                {
                    Environment.Exit(0);
                }
                if (utasitas != "ROffC" && utasitas != "LOnC" && utasitas != "ROnC" && utasitas != "LOffC" && utasitas != "")
                {
                    try
                    {
                        //Console.WriteLine((Convert.ToInt32(utasitas.Split('-')[0]).ToString() + Convert.ToInt32(utasitas.Split('-')[1])).ToString());
                        SetCursorPos(Convert.ToInt32(utasitas.Split('-')[0]), Convert.ToInt32(utasitas.Split('-')[1])); 
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
                Console.WriteLine(utasitas);
                try
                {
                    if (utasitas.StartsWith("keystroke"))
                    {
                        string[] darabolt = utasitas.Split(' ').Skip(1).ToArray();
                        StringBuilder asd = new StringBuilder("");
                        foreach (string a in darabolt)
                        {
                            asd.Append(a);
                            asd.Append(' ');
                        }
                        asd.Remove(asd.Length-1, asd.Length-1);
                        System.Windows.Forms.SendKeys.SendWait(asd.ToString());
                    }

                    switch (utasitas)
                    {
                        case "ROnC":
                            Console.WriteLine(utasitas);
                            mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, UIntPtr.Zero);
                            break;
                        case "ROffC":
                            Console.WriteLine(utasitas);
                            mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, UIntPtr.Zero);
                            break;
                        case "LOnC":
                            Console.WriteLine(utasitas);
                            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, UIntPtr.Zero);
                            break;
                        case "LOffC":
                            Console.WriteLine(utasitas);
                            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, UIntPtr.Zero);
                            break;

                        default:
                            break;
                    }
                }
                catch (Exception e)
                {

                    Console.WriteLine(e);
                }

            }
        }

        static async Task KepetCsinalKuld()
        {

            /*
                         ezeket importáltam:
                        using System.Drawing;
                        using System.Windows.Forms;
                        using System.Windows.Shapes;
                        using System.Drawing.Imaging;
                         */


            //screenshot csinálás : 
            // képernyő mérete

            //MessageBox.Show("elindultam aa");

            var udp = new UdpClient();
            int PORT = 6680;
            udp.EnableBroadcast = true;
            var broadCastEP = new IPEndPoint(IPAddress.Broadcast, PORT);
            byte[] data = Encoding.UTF8.GetBytes($"");

            //await udp.SendAsync(data, data.Length, broadCastEP);

            while (true)
            {

                Rectangle virtualScreen = SystemInformation.VirtualScreen;


                
                // bitmap a képernyő méretével
                using (Bitmap bitmap = new Bitmap(virtualScreen.Width, virtualScreen.Height))
                {
                    
                    try
                    {
                        // "lemásolja" a képernyő tartalmát a bitmapra
                        using (Graphics g = Graphics.FromImage(bitmap))
                        {
                            g.CopyFromScreen(virtualScreen.Location, Point.Empty, virtualScreen.Size);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($" nem jo a copyfromscreen : {e}");
                    }

                    // elmenti a screenshotot
                    string path = "screenshot.png";
                    try
                    {
                        bitmap.Save(path, ImageFormat.Png);
                    }
                    catch (Exception e)
                    {

                        Console.WriteLine("nem jo a kep save : " + e);
                    }


                    



                    /*
                    régi fájlkuldés logika:
                    data = File.ReadAllBytes("screenshot.png");

                    await udp.SendAsync(data, data.Length, broadCastEP);
                    //MessageBox.Show(data.Length.ToString());
                    Console.WriteLine("elkuldve");*/
                }
                Console.WriteLine($"Screenshot mentvexd");

                data = File.ReadAllBytes("screenshot.png");
                //fajldarabolos
                // Küldés előtt:
                int packetSize = 1024;
                int packetCount = (int)Math.Ceiling((double)data.Length / packetSize);

                // "Fejléc" csomag létrehozása
                string header = $"FGS.png {data.Length} {packetCount}";
                byte[] headerBytes = Encoding.UTF8.GetBytes(header);

                // Elküldöd először a header-t
                udp.Send(headerBytes, headerBytes.Length, broadCastEP);



                for (int i = 0; i < data.Length; i += packetSize)
                {
                    int size = Math.Min(packetSize, data.Length - i);
                    byte[] packet = new byte[size];
                    Array.Copy(data, i, packet, 0, size);
                    udp.Send(packet, packet.Length, broadCastEP);
                    Console.WriteLine($"Broadcast elküldve [{broadCastEP}]: :{i}");
                    await Task.Delay(1);
                }



                await Task.Delay(10000);

            }
        }
    }
}
