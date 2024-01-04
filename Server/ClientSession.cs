using ServerCore;
using System.Net;

namespace Server;

class Packet
{
    public ushort size;
    public ushort packetId = 4;
}

class PlayerInfoReq : Packet //플레이어 정보를 알고싶어서 서버로 보내는 패킷 (request)
{
    public long playerId;
}

class PlayerInfoOk : Packet //플레이어 정보를 서버한테 받았을때의 정보 패킷
{
    public int hp;
    public int attack;
}

public enum PacketID
{
    PlayerInfoReq = 1,
    PlayerInfoOk = 2,
}

class ClientSession : PacketSession
{
    public override void OnConnected(EndPoint endPoint)
    {
        Console.WriteLine($"OnConnected: {endPoint}");

        /*Packet packet = new Packet() { size=4, packetId = 10};

        ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
        byte[] buffer = BitConverter.GetBytes(packet.size);
        byte[] buffer2 = BitConverter.GetBytes(packet.packetId);
        Array.Copy(buffer,0, openSegment.Array, openSegment.Offset, buffer.Length);
        Array.Copy(buffer2,0, openSegment.Array, openSegment .Offset+ buffer.Length,buffer2.Length);
        ArraySegment<byte> sendBuff =SendBufferHelper.Close(packet.size);


        Send(sendBuff);*/
        Thread.Sleep(5000);
        Disconnect();
    }

    public override void OnDisconnected(EndPoint endPoint)
    {
        Console.WriteLine($"OnDisconnected: {endPoint}");
    }

    public override void OnRecvPacket(ArraySegment<byte> buffer)
    {
        ushort count = 0;

        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        count += 2;
        ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + 2);
        count += 2;

        switch ((PacketID)id)
        {
            case PacketID.PlayerInfoReq:
            {
                long playerId = BitConverter.ToInt64(buffer.Array, buffer.Offset + count);
                count += 8;
                Console.WriteLine($"PlayerInfoReq: {playerId}");
            }
                break;
        }

        Console.WriteLine($"RecvPacketId: {id}. Size:{size}");
    }

    public override void OnSend(int numOfBytes)
    {
        Console.WriteLine($"Transferred bytes: {numOfBytes}");
    }
}