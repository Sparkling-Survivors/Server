using ServerCore;
using System.Net;
using System.Text;

namespace Server;

class Packet
{
    public int size;
    public int packetId;
}



class GameSession : PacketSession
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
        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        ushort id = BitConverter.ToUInt16(buffer.Array,buffer.Offset+2);
        Console.WriteLine($"RecvPacketId: {id}. Size:{size}");
    }

    public override void OnSend(int numOfBytes)
    {
        Console.WriteLine($"Transferred bytes: {numOfBytes}");
    }
}

class Program
{
    private static Listener _listener = new Listener();

    static void Main(string[] args)
    {
        //DNS
        string host = Dns.GetHostName();
        IPHostEntry ipHost = Dns.GetHostEntry(host);
        IPAddress ipAddr = ipHost.AddressList[0];
        IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);


        _listener.Init(endPoint, () => { return new GameSession(); }); //누가 들어오면 OnAcceptHandler로 알려줘
        Console.WriteLine("Listening...");

        while (true)
        {
            //프로그램 종료 방지용 임시
        }
    }
}