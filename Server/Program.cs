using System.Diagnostics;
using ServerCore;
using System.Net;
using System.Text;
using Server.Game;

namespace Server;

class Program
{
    static Listener _listener = new Listener();

    static void PingPong() //주기적으로 클라이언트와 연결이 끊겼나 확인. 끊겼다면 리소스 절약을 위해 관련된 내용을 제거해주고 끊음.
    {
        //Console.WriteLine("PingPong");
        JobTimer.Instance.Push(PingPong, 20000); //2초 간격으로 확인
    }

    static void CheckSessionNum()
    {
        //Console.WriteLine($"session num : {SessionManager.Instance._sessions.Count}");
        JobTimer.Instance.Push(CheckSessionNum,10000);
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
        
        
        JobTimer.Instance.Push(PingPong); //PingPong 함수를 5초 간격으로 실행시키기 위해서 잡타이머에 등록
        JobTimer.Instance.Push(CheckSessionNum); //세션 갯수 1초마다 확인

        while (true)
        {
            JobTimer.Instance.Flush();
        }
    }
}