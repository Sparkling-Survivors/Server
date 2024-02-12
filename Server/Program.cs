using ServerCore;
using System.Net;
using System.Text;
using Server.Game;

namespace Server;

class Program
{
    static Listener _listener = new Listener();

    static void FlushRoom()
    {
        JobTimer.Instance.Push(FlushRoom, 250);
    }

    static void Main(string[] args)
    {
        //모든 서버 정보 초기화
        
        
        // DNS (Domain Name System)
        string host = Dns.GetHostName();
        IPHostEntry ipHost = Dns.GetHostEntry(host);
        IPAddress ipAddr = ipHost.AddressList[0];
        IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

        _listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); });
        Console.WriteLine("Listening...");

        //FlushRoom();
        JobTimer.Instance.Push(FlushRoom);

        while (true)
        {
            JobTimer.Instance.Flush();
        }
    }
}