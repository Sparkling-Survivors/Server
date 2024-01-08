using ServerCore;

namespace Server;

public class PacketHandler
{
    public static void PlayerInfoReqHandler(PacketSession session, IPacket packet)
    {
        PlayerInfoReq p = packet as PlayerInfoReq;
        
        Console.WriteLine($"PlayerInfoReq: {p.playerId} {p.name}");
                
        foreach (PlayerInfoReq.Skill skill in p.skills)
        {
            Console.WriteLine($"Skill ({skill.id}) ({skill.level}) ({skill.duration})");
        } 
    }
}