using System;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace Game_Caro
{
    class SocketManager
    {
        #region Client
        Socket client;
        public bool ConnectServer()
        {
            IPEndPoint iep = new IPEndPoint(IPAddress.Parse(IP), Port);
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                client.Connect(iep);
                return true;
            }                
            catch
            {
                return false;
            } 
        }
        #endregion

        #region Server
        Socket server;
        public void CreateServer()
        {
            IPEndPoint iep = new IPEndPoint(IPAddress.Parse(IP), Port);
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            server.Bind(iep);
            server.Listen(10); // Đợi kết nối client trong 10s nếu ko có thì bỏ

            Thread AcceptClient = new Thread(() => { try { client = server.Accept(); } catch { } });
            AcceptClient.IsBackground = true; // Để khi chương trình tắt ngang thì Thread cũng tự tắt
            AcceptClient.Start();
        }
        #endregion

        #region Both
        public string IP = "127.0.0.1";
        public int Port = 9999;
        public bool IsServer = true;
        public const int BUFFER = 1024;

        private bool SendData(Socket target, byte[] data)
        {
            return target.Send(data) == 1;
        }

        private bool ReceiveData(Socket target, byte[] data)
        {
            return target.Receive(data) == 1;
        }

        public bool Send(object data)
        {
            byte[] sendedData = SerializeData(data);
            return SendData(client, sendedData);
        }

        public object Receive()
        {
            byte[] receivedData = new byte[BUFFER]; // 1 lần nhận tin là cỡ bao nhiêu
            bool IsOk = ReceiveData(client, receivedData);
            return DeserializeData(receivedData);
        }

        /// <summary>
        /// Nén đối tượng thành mảng byte[]
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public byte[] SerializeData(Object o)
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf1 = new BinaryFormatter();
            bf1.Serialize(ms, o);
            return ms.ToArray();
        }

        /// <summary>
        /// Giải nén mảng byte[] thành đối tượng object
        /// </summary>
        /// <param name="theByteArray"></param>
        /// <returns></returns>
        public object DeserializeData(byte[] theByteArray)
        {
            MemoryStream ms = new MemoryStream(theByteArray);
            BinaryFormatter bf1 = new BinaryFormatter();
            ms.Position = 0;
            return bf1.Deserialize(ms);
        }

        /// <summary>
        /// Lấy ra IP V4 của card mạng đang dùng
        /// </summary>
        /// <param name="_type"></param>
        /// <returns></returns>
        public string GetLocalIPv4(NetworkInterfaceType _type)
        {
            string output = "";

            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
                if (item.NetworkInterfaceType == _type && item.OperationalStatus == OperationalStatus.Up)
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                            output = ip.Address.ToString();
            return output;
        }

        public void CloseConnect()
        {
            try
            {
                server.Close();
                client.Close();
            } catch { }
            
        }
        #endregion
    }
}
