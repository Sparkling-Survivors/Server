﻿using ServerCore;
using System.Net;
using System.Text;

namespace Server;


class GameSession : Session
{
    public override void OnConnected(EndPoint endPoint)
    {
        Console.WriteLine($"OnConnected: {endPoint}");

        byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome to MMORPG Server !");
        Send(sendBuff);
        Thread.Sleep(1000);
        Disconnect();
    }

    public override void OnDisconnected(EndPoint endPoint)
    {
        Console.WriteLine($"OnDisconnected: {endPoint}");
    }

    public override void OnRecv(ArraySegment<byte> buffer)
    {
        string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
        Console.WriteLine($"[From Client] {recvData}");
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