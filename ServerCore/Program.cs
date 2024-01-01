using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
    class Program
    {
        private static Listener _listener = new Listener();

        static void OnAcceptHandler(Socket clientSocket)
        {
            try
            {
                byte[] sendBuffer = Encoding.UTF8.GetBytes("Welcome to MMORPG Server !");

                Session session = new Session();
                session.Start(clientSocket);

                Thread.Sleep(1000);

                session.Disconnect();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        static void Main(string[] args)
        {
            //DNS
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            _listener.Init(endPoint, OnAcceptHandler); //누가 들어오면 OnAcceptHandler로 알려줘
            Console.WriteLine("Listening...");

            while (true)
            {
                //프로그램 종료 방지용 임시
            }
        }
    }
}