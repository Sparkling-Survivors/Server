using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server;
using Server.Game;
using ServerCore;

public class PacketHandler
{
    
    //서버가 들고있는 방 리스트를 클라이언트에게 보냄
    public static void CS_RoomListHandler(PacketSession session, IMessage packet)
    {
        CS_RoomList roomListPacket = packet as CS_RoomList;
        ClientSession clientSession = session as ClientSession;
        
        SC_RoomList sendPacket = new SC_RoomList();
        
        //RoomManager의 _rooms 딕셔너리에 들어있는 GameRoom들을 sendPacket에 추가해준다.
        foreach (GameRoom room in RoomManager.Instance._rooms.Values)
        {
            sendPacket.Rooms.Add(room.Info);
        }
        clientSession.Send(sendPacket);
    }
    
    //클라이언트가 요청한 정보대로 방을 만들어서, 그 정보를 클라이언트에게 보냄
    public static void CS_MakeRoomHandler(PacketSession session, IMessage packet)
    {
        CS_MakeRoom makeRoomPacket = packet as CS_MakeRoom;
        ClientSession clientSession = session as ClientSession;
        
        SC_MakeRoom sendPacket = new SC_MakeRoom();
        GameRoom newRoom = RoomManager.Instance.MakeRoom(makeRoomPacket.Title, makeRoomPacket.IsPrivate, makeRoomPacket.Password, clientSession.SessionId);

        sendPacket.Room = newRoom.Info;
        sendPacket.Password = makeRoomPacket.Password;
        clientSession.Send(sendPacket);
    }
    
    //클라이언트가 특정 방 입장을 요청하면, 입장가능 여부를 판단해서, 입장가능하면 그 방에 입장시키고, 방 정보를 클라이언트에게 보냄
    public static void CS_EnterRoomHandler(PacketSession session, IMessage packet)
    {
        CS_EnterRoom enterRoomPacket = packet as CS_EnterRoom;
        ClientSession clientSession = session as ClientSession;
        
        RoomManager.Instance.EnterRoom(enterRoomPacket.RoomId, enterRoomPacket.Password, clientSession, enterRoomPacket.Name);
    }
    
    public static void CS_LeaveRoomHandler(PacketSession session, IMessage packet)
    {
        CS_LeaveRoom leaveRoomPacket = packet as CS_LeaveRoom;
        ClientSession clientSession = session as ClientSession;
        
        RoomManager.Instance.LeaveRoom(leaveRoomPacket.RoomId, clientSession);
    }
    
    //클라이언트 헬스체크용 핑퐁 처리
    public static void CS_PingPongHandler(PacketSession session, IMessage packet)
    {
        CS_PingPong pingPongPacket = packet as CS_PingPong;
        ClientSession clientSession = session as ClientSession;
        
        clientSession.PingPong._isPong = true;
    }
    
    //클라이언트에서 방에 사람이 모여서 게임시작 요청을 했을때, 해당 방 사람들에게 데디서버 정보를 알려줘서 접속시킴
    //아마 나중에 방장만 해당 패킷을 보내도록 처리할 예정
    //***********   여기서 새로운 데디서버 프로세스를 생성하고 해당 정보를 넘겨주어야함 !! ******************
    public static void CS_ConnectDedicatedServerHandler(PacketSession session, IMessage packet)
    {
        CS_ConnectDedicatedServer connectDedicatedServerPacket = packet as CS_ConnectDedicatedServer;
        ClientSession clientSession = session as ClientSession; //방장 클라이언트가 될 예정
        
        //방장일 경우에만 데디서버 생성 및 정보 뿌리기를 허용
        if (RoomManager.Instance.IsMaster(connectDedicatedServerPacket.RoomId, clientSession))
        {
            //TODO : 데디서버 프로세스 생성. (지금은 임의로 켜놓은 서버로 테스트)
            SC_ConnectDedicatedServer sendPacket =DedicatedServerManager.Instance.CreateDedicatedServer();
        
            //임시로 데디서버 정보를 만들어서 클라이언트에게 보냄
            //해당 방장의 room을 찾은뒤, 그 방에 있는 모든 클라이언트에게 데디서버 정보를 보냄
            //room의 상태를 게임중으로 바꿔줌
            GameRoom room = RoomManager.Instance.FindRoomByClientSession(clientSession);
            if(room!=null)
            {
                DedicatedServerManager.Instance.SendDedicatedServerConnectInfo(room, sendPacket);
                RoomManager.Instance.SetRoomPlaying(room.Info.RoomId);
            }
        }
        
    }
    
}