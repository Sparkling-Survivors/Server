using System.Net;
using Google.Protobuf.Protocol;

namespace Server.Game;

public class DedicatedServerManager
{
    public static DedicatedServerManager Instance { get; } = new DedicatedServerManager();
    
    public SC_ConnectDedicatedServer CreateDediServer()
    {
        // DNS (Domain Name System)
        string host = Dns.GetHostName();
        IPHostEntry ipHost = Dns.GetHostEntry(host);
        IPAddress ipAddr = ipHost.AddressList[0];
        
        string ip = ipAddr.ToString();
        int port = 8888;

        SC_ConnectDedicatedServer sendPacket = new SC_ConnectDedicatedServer();
        
        if (ip == null || port == -1) //생성 실패일 경우
        {
            sendPacket.Ip = null;
            sendPacket.Port = -1;
        }
        else
        {
            sendPacket.Ip = ip;
            sendPacket.Port = port;
        }

        return sendPacket;
    }
}