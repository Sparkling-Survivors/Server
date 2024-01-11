using DummyClient;
using ServerCore;

public class PacketHandler
{
    public static void S_ChatHandler(PacketSession session, IPacket packet)
    {
        S_Chat chatPacket = packet as S_Chat;
        ServerSession serverSession = session as ServerSession;
        
        //너무 많이찍으면 복잡하니까 1번 패킷만 찍어줌
        //if(chatPacket.playerId==1)
            //Console.WriteLine(chatPacket.chat);
    }
}