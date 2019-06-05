using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TeamViewer
{
    public partial class MainForm : Form
    {
        private TcpListener listener;
        private NetworkStream serverStream;
        private Socket clientSocket;
        public MainForm()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;

            Thread thread = new Thread(ServerThread);
            thread.Start();
        }

        private void ServerThread()
        {
            // Each client start tcplistener
            IPAddress ip = GetLocalHost();
            // For test in local
            ip = IPAddress.Parse("127.0.0.1");
            int port = 8080;
            try
            { 
                listener = new TcpListener(ip, port);
                listener.Start();
            }
            catch
            {
                // For test in local
                port = 13000;
                listener = new TcpListener(ip, port);
                listener.Start();
            }

            lblAddress.Text = $"Address: {ip}:{port}";
            clientSocket = listener.AcceptSocket();
            // If connect will send image
            Thread thread = new Thread(SendScreenThread);
            thread.Start();
        }

        private void SendScreenThread()
        {
            FrameManager frame = new FrameManager(60);
            while(true)
            {
                frame.Start();
                byte[] byteSend;
                Image bitmap = takeScreenShoot();
                using (MemoryStream ms = new MemoryStream())
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(ms, bitmap);
                    byteSend = ms.ToArray();
                }
                clientSocket.Send(byteSend);
                frame.StopAndDelay();
            }
        }

        private void ReceiveScreenThread()
        {
            while (true)
            {
                BinaryFormatter formatter = new BinaryFormatter();
                Image bitmap = (Image)formatter.Deserialize(serverStream);
                ptbScreenShot.Image = bitmap;
            }
            
        }

        private Image takeScreenShoot()
        {
            Bitmap bitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.CopyFromScreen(0, 0, 0, 0, Screen.PrimaryScreen.Bounds.Size);
            return bitmap;
            
        }

        private IPAddress GetLocalHost()
        {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                return endPoint.Address;
            }
        }

        private void BtnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                TcpClient client = new TcpClient();
                client.Connect(txtIP.Text, int.Parse(txtPort.Text));
                serverStream = client.GetStream();
                (sender as Button).Enabled = false;

                Thread thread = new Thread(ReceiveScreenThread);
                thread.Start();
            }
            catch (Exception)
            {
                MessageBox.Show("Check ip or port.");
            }
        }
    }
}
