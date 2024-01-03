using ServerCore;
using System.Net;
using System.Text;

namespace DummyClient;

class Packet
{
    public int size;
    public int packetId = 4;
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

class ServerSession : Session
{
    public override void OnConnected(EndPoint endPoint)
    {
        Console.WriteLine($"OnConnected: {endPoint}");

        Packet packet = new Packet() { size = 4, packetId = 7 };

        //보낸다
        for (int i = 0; i < 5; i++)
        {
            ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
            byte[] buffer = BitConverter.GetBytes(packet.size);
            byte[] buffer2 = BitConverter.GetBytes(packet.packetId);
            Array.Copy(buffer, 0, openSegment.Array, openSegment.Offset, buffer.Length);
            Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer.Length, buffer2.Length);
            ArraySegment<byte> sendBuff = SendBufferHelper.Close(packet.size);

            Send(sendBuff);
        }
    }

    public override void OnDisconnected(EndPoint endPoint)
    {
        Console.WriteLine($"OnDisconnected: {endPoint}");
    }

    public override int OnRecv(ArraySegment<byte> buffer)
    {
        string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
        Console.WriteLine($"[From Server] {recvData}");
        return buffer.Count;
    }

    public override void OnSend(int numOfBytes)
    {
        Console.WriteLine($"Transferred bytes: {numOfBytes}");
    }
}