using Google.Protobuf.Protocol;

namespace Server.Game;

public class RoomManager
{
    public static RoomManager Instance { get; } = new RoomManager();

    object _lock = new object();
    public Dictionary<int, GameRoom> _rooms = new Dictionary<int, GameRoom>();
    int _roomId = 1;
    int _maxPerRoomCount = 4; //한 방당 최대 인원수

    public GameRoom MakeRoom(string title, bool isPrivate = false, string password = "")
    {
        GameRoom gameRoom = new GameRoom();

        lock (_lock)
        {
            gameRoom.Info.RoomId = _roomId;
            gameRoom.Info.Title = title;
            gameRoom.Info.CurrentCount = 0;
            gameRoom.Info.MaxCount = _maxPerRoomCount;
            gameRoom.Info.IsPrivate = isPrivate;
            gameRoom.Password = password;
            gameRoom.Info.IsPlaying = false;
            _rooms.Add(_roomId, gameRoom);
            _roomId++;
        }

        return gameRoom;
    }

    public void EnterRoom(int roomId, string password, ClientSession session, string name)
    {
        GameRoom room = null;
        S_AllowEnterRoom allowEnterPacket = new S_AllowEnterRoom(){CanEnter = false};

        lock (_lock)    
        {
            if (!_rooms.ContainsKey(roomId)) //존재하지 않는 방이면 입장불가
            {
                session.Send(allowEnterPacket);
            }   
            else
            {
                room = _rooms[roomId];
                if (room.Info.IsPlaying) //게임중이면 입장불가
                    session.Send(allowEnterPacket);
                else if (room.Info.CurrentCount >= room.Info.MaxCount) //방이 꽉차면 입장불가
                    session.Send(allowEnterPacket);
                else if (room.Info.IsPrivate && room.Password != password) //비공개방이면서, 비밀번호가 틀리면 입장불가
                    session.Send(allowEnterPacket);
                else //비공개방이면서, 비밀번호가 맞은경우 입장 or 입장가능한 공개방
                {
                    //서버에서 방입장 처리
                    room.EnterRoom(session, name);
                }
            }
        }
    }

    public void LeaveRoom(int roomId, ClientSession session)
    {
        S_LeaveGame sendPacket = new S_LeaveGame();
        
        lock (_lock)
        {
            if (!_rooms.ContainsKey(roomId)) //존재하지 않는 방이면 퇴장 가능처리
            {
                session.Send(sendPacket);
            }
            else
            {
                GameRoom room = _rooms[roomId];
                room.LeaveRoom(session);
            }
        }
    }

    public bool Remove(int roomId)
    {
        lock (_lock)
        {
            return _rooms.Remove(roomId);
        }
    }

    public GameRoom Find(int roomId)
    {
        GameRoom room = null;
        if (_rooms.TryGetValue(roomId, out room))
            return room;

        return null;
    }
}