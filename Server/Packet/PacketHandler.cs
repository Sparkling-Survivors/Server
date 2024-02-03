using Google.Protobuf;
using Google.Protobuf.Protocol;
using Server;
using Server.Game;
using ServerCore;

public class PacketHandler
{
    
    //서버가 들고있는 방 리스트를 클라이언트에게 보냄
    public static void C_RoomListHandler(PacketSession session, IMessage packet)
    {
        C_RoomList roomListPacket = packet as C_RoomList;
        ClientSession clientSession = session as ClientSession;
        
        S_RoomList sendPacket = new S_RoomList();
        
        //RoomManager의 _rooms 딕셔너리에 들어있는 GameRoom들을 sendPacket에 추가해준다.
        foreach (GameRoom room in RoomManager.Instance._rooms.Values)
        {
            sendPacket.Rooms.Add(room.Info);
        }
        clientSession.Send(sendPacket);
    }
    
    //클라이언트가 요청한 정보대로 방을 만들어서, 그 정보를 클라이언트에게 보냄
    public static void C_MakeRoomHandler(PacketSession session, IMessage packet)
    {
        C_MakeRoom makeRoomPacket = packet as C_MakeRoom;
        ClientSession clientSession = session as ClientSession;
        
        S_MakeRoom sendPacket = new S_MakeRoom();
        GameRoom newRoom = RoomManager.Instance.MakeRoom(makeRoomPacket.Title, makeRoomPacket.IsPrivate, makeRoomPacket.Password);

        sendPacket.Room = newRoom.Info;
        clientSession.Send(sendPacket);
    }
    
    //클라이언트가 특정 방 입장을 요청하면, 입장가능 여부를 판단해서, 입장가능하면 그 방에 입장시키고, 방 정보를 클라이언트에게 보냄
    public static void C_EnterRoomHandler(PacketSession session, IMessage packet)
    {
        C_EnterRoom enterRoomPacket = packet as C_EnterRoom;
        ClientSession clientSession = session as ClientSession;
        
        RoomManager.Instance.EnterRoom(enterRoomPacket.RoomId, enterRoomPacket.Password, clientSession, enterRoomPacket.Name);
    }
    
    public static void C_LeaveRoomHandler(PacketSession session, IMessage packet)
    {
        C_LeaveRoom leaveRoomPacket = packet as C_LeaveRoom;
        ClientSession clientSession = session as ClientSession;
        
        RoomManager.Instance.LeaveRoom(leaveRoomPacket.RoomId, clientSession);
    }
    
    public static void C_MoveHandler(PacketSession session, IMessage packet)
    {
        C_Move movePacket = packet as C_Move;
        ClientSession clientSession = session as ClientSession;
        
    }
}