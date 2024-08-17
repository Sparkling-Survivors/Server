using System.Diagnostics;
using ServerCore;
using System.Net;
using System.Text;
using Google.Protobuf;
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
        IPAddress ipAddr = IPAddress.Any;
        IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

        _listener.Init(endPoint, () => { return SessionManager.Instance.Generate(); });
        Console.WriteLine("Listening...");
        
        JobTimer.Instance.Push(CheckSessionNum); //세션 갯수 1초마다 확인
        /*while (true)
        {
            //패킷 처리
            List<PacketMessage> list = PacketQueue.Instance.PopAll();
            foreach (PacketMessage packet in list)
            {
                Action<PacketSession, IMessage> handler = PacketManager.Instance.GetPacketHandler(packet.Id);
                if (handler != null)
                    handler.Invoke(packet.Session, packet.Message);
            }
            
            
            JobTimer.Instance.Flush();
            Thread.Sleep(10);
        }*/
    }
}