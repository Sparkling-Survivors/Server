
using ServerCore;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DummyClient
{
    

    class Program
    {
        static void Main(string[] args)
        {
            //DNS
            string host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777);

            Connector connector = new Connector();
            //현재 10개의 클라이언트를 시뮬레이팅한다는 설정(count)
            connector.Connect(endPoint, () => { return SessionManager.Instance.Generate();},500);
            while (true)
            {
                try
                {
                    //모든 세션들이 다 채팅메시지를 서버 쪽으로 날려줌
                    SessionManager.Instance.SendForEach();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                //일반적으로 MMO 만들때 이동 패킷을 대충 1초에 4번정도 보내기 때문에 250ms마다 1번씩 보내도록 설정
                Thread.Sleep(250);
            }
            


        }
    }
}