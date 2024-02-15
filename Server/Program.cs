using System.Diagnostics;
using ServerCore;
using System.Net;
using System.Text;
using Server.Game;

namespace Server;

class Program
{
    static Listener _listener = new Listener();

    static void CheckSessionNum()
    {
        Console.WriteLine($"session num : {SessionManager.Instance._sessions.Count}");
        JobTimer.Instance.Push(CheckSessionNum,1000); //1초 간격으로 
    }

    static void Main(string[] args)
    {
        // DNS (Domain Name System)
        string host = Dns.GetHostName();
        IPHostEntry ipHost = Dns.GetHostEntry(host);
        IPAddress ipAddr = ipHost.AddressList[0];
        IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

        _listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); });
        Console.WriteLine("Listening...");
        
        JobTimer.Instance.Push(CheckSessionNum); //세션 갯수 1초마다 확인
        while (true)
        {
            JobTimer.Instance.Flush();
        }
    }
}