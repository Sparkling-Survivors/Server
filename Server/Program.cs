using ServerCore;
using System.Net;
using System.Text;

namespace Server;



class Program
{
    private static Listener _listener = new Listener();
    public static GameRoom Room = new GameRoom();

    static void Main(string[] args)
    {
        PacketManager.Instance.Register();
        
        //DNS
        string host = Dns.GetHostName();
        IPHostEntry ipHost = Dns.GetHostEntry(host);
        IPAddress ipAddr = ipHost.AddressList[0];
        IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);


        _listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); }); //누가 들어오면 OnAcceptHandler로 알려줘
        Console.WriteLine("Listening...");

        while (true)
        {
            //프로그램 종료 방지용 임시
        }
    }
}