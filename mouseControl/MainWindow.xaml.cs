using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Windows.Threading;
using System.Net;
using System.Net.Sockets;
using System.IO;


namespace mouseControl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static int PORT = 6667;
        UdpClient udp = new UdpClient();

        public static bool LeftOnClick = false;
        public static bool RightOnClick = false;
        public static bool LeftOffClick = false;
        public static bool RightOffClick = false;

        public static int elozoX = 0;
        public static int elozoY = 0;



        public static bool kuldeUzenet = false;
        public static string uzenet = "";
        public MainWindow()
        {
            InitializeComponent();

            //hatter.Source = new BitmapImage(new Uri("FGS.png"));

            Rect asd = System.Windows.SystemParameters.WorkArea;


            var vS = SystemInformation.VirtualScreen;




            this.Width = vS.Width;
            this.Height = vS.Height;   
            this.Top = vS.Top;
            this.Left = vS.Left;
            

            
            udp.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            udp.Client.Bind(new IPEndPoint(IPAddress.Any, PORT));

            udp.EnableBroadcast = true;

            
            ideKattints.MouseLeftButtonDown += IdeKattints_MouseLeftButtonDown;
            ideKattints.MouseLeftButtonUp += IdeKattints_MouseLeftButtonUp;
            ideKattints.MouseRightButtonDown += IdeKattints_MouseRightButtonDown;
            ideKattints.MouseRightButtonUp += IdeKattints_MouseRightButtonUp;

            this.PreviewTextInput += MainWindow_PreviewTextInput;
            this.KeyDown += MainWindow_KeyDown;

            this.Loaded += (s, e) => this.Focus();

            this.Closing += MainWindow_Closing;


            _ = broadCast(udp);
            //_ = MegkapniAzUzenetet();
            //Task.Run(() => broadCast(udp));
            //Task.Run(() => MegkapniAzUzenetet());

        }

        private void MainWindow_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            uzenet = e.Text;
            kuldeUzenet = true;
            //System.Windows.MessageBox.Show(uzenet);
        }

        private void MainWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter) uzenet = "{ENTER}";
            if (e.Key == Key.Escape) uzenet = "{ESC}";
            if (e.Key == Key.Return) uzenet = "{BS}";
            if (e.Key == Key.Tab) uzenet = "{TAB}";

            if (e.Key == Key.Up) { uzenet = "{UP}";};
            if (e.Key == Key.Down) uzenet = "{DOWN}";
            if (e.Key == Key.Left) uzenet = "{LEFT}";
            if (e.Key == Key.Right) uzenet = "{RIGHT}";
            kuldeUzenet = true;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            UdpClient udp = new UdpClient();
            var pos = System.Windows.Forms.Cursor.Position;
            var broadCastEP = new IPEndPoint(IPAddress.Broadcast, PORT);
            byte[] data;
            data = Encoding.UTF8.GetBytes($"off");
            udp.Send(data, data.Length, broadCastEP);
        }

        private void IdeKattints_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            RightOffClick = true;
        }

        private void IdeKattints_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            RightOnClick = true;
        }

        private void IdeKattints_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            LeftOffClick = true;
        }

        private void IdeKattints_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //System.Windows.MessageBox.Show("asd");
            LeftOnClick = true;
        }

        static async Task broadCast(UdpClient udp)
        {
            
            while (true)
            {
                await Task.Delay(20);
                var pos = System.Windows.Forms.Cursor.Position;
                var broadCastEP = new IPEndPoint(IPAddress.Broadcast, PORT);
                byte[] data;
                data = Encoding.UTF8.GetBytes($"{pos.X}-{pos.Y}");

                if (kuldeUzenet)
                {
                    data = Encoding.UTF8.GetBytes($"keystroke {uzenet}");
                    await udp.SendAsync(data, data.Length, broadCastEP);
                    kuldeUzenet = false;
                }





                if (RightOnClick)
                {
                    data = Encoding.UTF8.GetBytes($"ROnC");
                    RightOnClick = false;
                    await udp.SendAsync(data, data.Length, broadCastEP);
                }

                if (RightOffClick)
                {
                    data = Encoding.UTF8.GetBytes($"ROffC");
                    RightOffClick = false;
                    await udp.SendAsync(data, data.Length, broadCastEP);
                }

                if (LeftOnClick)
                {
                    data = Encoding.UTF8.GetBytes($"LOnC");
                    LeftOnClick = false;
                    await udp.SendAsync(data, data.Length, broadCastEP);
                }
                if (LeftOffClick)
                {
                    data = Encoding.UTF8.GetBytes($"LOffC");
                    LeftOffClick = false;
                    await udp.SendAsync(data, data.Length, broadCastEP);
                }

                if (elozoX!=pos.X || elozoY != pos.Y)
                {
                    await udp.SendAsync(data, data.Length, broadCastEP);
                }
                elozoX = pos.X;
                elozoY = pos.Y;


            }
        }
        

        public async Task MegkapniAzUzenetet()
        {
            /*
            System.Diagnostics.Debug.WriteLine($"asdasd");
            try
            {
                string path = System.IO.Path.GetFullPath("FGS.png");
                hatter.Source = new BitmapImage(new Uri(path, UriKind.Absolute));
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.ToString());
                //throw;
            }*/
            
            int PORT = 6680;
            UdpClient udp = new UdpClient();
            udp.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            udp.Client.Bind(new IPEndPoint(IPAddress.Any, PORT));
            //Console.WriteLine("asd");
            //udp.EnableBroadcast = true;

            var broadCastEP = new IPEndPoint(IPAddress.Broadcast, PORT);

            //byte[] data = Encoding.UTF8.GetBytes($"");


            while (true)
            {
                var result = await udp.ReceiveAsync();
                byte[] message = result.Buffer;
                

                string utasitas = Encoding.UTF8.GetString(message);
                //System.Diagnostics.Debug.WriteLine(utasitas);


                int packetSize = Convert.ToInt32(utasitas.Split(' ')[1]);
                int packetCount = Convert.ToInt32(utasitas.Split(' ')[2]);

                string krealtExefajlNeve = utasitas.Split(' ')[0];

                // csomagok várása, és másolása egymás után:(fubazdmeg chat gpt)
                try
                {
                    using (FileStream fs = new FileStream("FGS.png", FileMode.Create, FileAccess.Write))
                    {
                        for (int i = 0; i < packetCount; i++)
                        {
                            //System.Windows.MessageBox.Show("teszt" + utasitas);

                            result = await udp.ReceiveAsync();
                            byte[] buffer = result.Buffer;
                            //byte[] buffer = udp.Receive(ref broadCastEP);
                            fs.Write(buffer, 0, buffer.Length);
                            System.Diagnostics.Debug.WriteLine("teszt");
                        }
                    }

                    //Process.Start(krealtExefajlNeve);//inditas
                }
                catch (Exception)
                {
                    //System.Windows.MessageBox.Show("NYÖNYÖNÖY");
                }




                /*
                 * 
                 * régi várakozás logika:
                 * 
                 * 
                //System.Windows.MessageBox.Show("melegvagy");
                var result = await udp.ReceiveAsync();
                byte[] message = result.Buffer;
                //System.Windows.MessageBox.Show("melegvagy");
                
                //string utasitas = Encoding.UTF8.GetString(message);

                System.Windows.MessageBox.Show(message.ToString()) ;

                File.WriteAllBytes("FGS.png", message);*/

            }

            




        }


    }
}
