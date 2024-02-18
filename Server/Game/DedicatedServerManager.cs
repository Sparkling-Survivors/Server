using System.Net;
using Google.Protobuf.Protocol;

namespace Server.Game;

public struct DedicatedServerInfo
{
    public string _ip;
    public int _port;
}

public class DedicatedServerManager
{
    public static DedicatedServerManager Instance { get; } = new DedicatedServerManager();

    /// <summary>
    /// 데디케이티드 서버를 생성하고 그 ip와 port번호를 담은 패킷을 반환 
    /// </summary>
    /// <returns>생성된 데디서버의 ip와 port번호가 담긴 패킷. ip가 null또는 port가 -1이면 생성실패</returns>
    public SC_ConnectDedicatedServer CreateDedicatedServer()
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

    /// <summary>
    /// 특정 게임룸에 있는 플레이어들에게 데디서버로 연결할 정보를 전송 + 게임룸에 데디서버 정보 등록
    /// </summary>
    /// <param name="room">데디서버로 연결할 게임룸</param>
    /// <param name="sendPacket">데디서버 ip와 port번호가 담긴 패킷</param>
    public void SendDedicatedServerConnectInfo(GameRoom room, SC_ConnectDedicatedServer sendPacket)
    {
        //게임룸에 데디서버 정보 등록
        DedicatedServerInfo dedicatedServerInfo = new DedicatedServerInfo();
        dedicatedServerInfo._ip = sendPacket.Ip;
        dedicatedServerInfo._port = sendPacket.Port;
        SaveDedicatedServerInfoInRoom(room, dedicatedServerInfo);
        
        //게임룸에 있는 플레이어들에게 데디서버로 연결할 정보 전송
        foreach (Player p in room._players)
        {
            p.Session.Send(sendPacket);
        }
    }
    
    /// <summary>
    /// 게임이 시작했을때, 게임룸에 데디서버 정보 저장
    /// </summary>
    /// <param name="room"></param>
    /// <param name="dedicatedServerInfo"></param>
    public void SaveDedicatedServerInfoInRoom(GameRoom room, DedicatedServerInfo dedicatedServerInfo)
    {
        room._dedicatedServerInfo = dedicatedServerInfo;
    }
}