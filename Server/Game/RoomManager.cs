using System.ComponentModel.Design;
using Google.Protobuf.Protocol;

namespace Server.Game;

public class RoomManager
{
    public static RoomManager Instance { get; } = new RoomManager();

    object _lock = new object();
    public Dictionary<int, GameRoom> _rooms = new Dictionary<int, GameRoom>();
    int _roomId = 1;
    int _maxPerRoomCount = 8; //한 방당 최대 인원수

    public GameRoom MakeRoom(string title, bool isPrivate = false, string password = "", int clientSessionSessionId = -1)
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
            gameRoom._dedicatedServerInfo._ip = null;
            gameRoom._dedicatedServerInfo._port = -1;
            gameRoom.Info.RoomMasterPlayerId = clientSessionSessionId; //최초 방장은 방 생성한 플레이어의 sessionID임.
            _rooms.Add(_roomId, gameRoom);
            _roomId++;
        }

        return gameRoom;
    }

    public void EnterRoom(int roomId, string password, ClientSession session, string name)
    {
        GameRoom room = null;
        SC_AllowEnterRoom allowEnterPacket = new SC_AllowEnterRoom(){CanEnter = false};

        lock (_lock)    
        {
            if (!_rooms.ContainsKey(roomId)) //존재하지 않는 방이면 입장불가
            {
                allowEnterPacket.ReasonRejected = ReasonRejected.RoomNotExist;
                session.Send(allowEnterPacket);
            }   
            else
            {
                room = _rooms[roomId];
                if (room.Info.IsPlaying) //게임중이면 입장불가
                {
                    allowEnterPacket.ReasonRejected = ReasonRejected.CurrentlyPlaying;
                    session.Send(allowEnterPacket);
                }
                else if (room.Info.CurrentCount >= room.Info.MaxCount) //방이 꽉차면 입장불가
                {
                    allowEnterPacket.ReasonRejected = ReasonRejected.RoomIsFull;
                    session.Send(allowEnterPacket);
                }
                else if (room.Info.IsPrivate && room.Password != password) //비공개방이면서, 비밀번호가 틀리면 입장불가
                {
                    allowEnterPacket.ReasonRejected = ReasonRejected.WrongPassword;
                    session.Send(allowEnterPacket);
                }
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
        SC_LeaveRoom sendPacket = new SC_LeaveRoom();
        sendPacket.PlayerId = session.SessionId; //현재 플레이어id는 세션아이디와 같음
        
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
    
    public void SetRoomPlaying(int roomId)
    {
        lock (_lock)
        {
            if (_rooms.ContainsKey(roomId))
            {
                _rooms[roomId].Info.IsPlaying = true;
            }
        }
    }

    public GameRoom Find(int roomId)
    {
        GameRoom room = null;
        if (_rooms.TryGetValue(roomId, out room))
            return room;

        return null;
    }
    
    /// <summary>
    /// 해당 클라이언트 세션이 속한 방을 찾아서 반환
    /// </summary>
    /// <param name="clientSessionSessionId"></param>
    /// <returns>속한 방이 없다면 null을 반환함</returns>
    public GameRoom FindRoomByClientSession(ClientSession clientSession)
    {
        if(clientSession!=null && clientSession.MyPlayer!=null && clientSession.MyPlayer.Room!=null)
        {
            GameRoom room = clientSession.MyPlayer.Room;
            return room;
        }
        else
        {
            return null;
        }
    }
    
    /// <summary>
    /// 특정 roomid의 방장이 특정 clientsession과 동일한지 확인
    /// </summary>
    /// <param name="roomId">방 번호</param>
    /// <param name="clientSession">클라이언트 세션</param>
    /// <returns></returns>
    public bool IsMaster(int roomId, ClientSession clientSession)
    {
        if (!_rooms.ContainsKey(roomId))
            return false;

        GameRoom gameRoom = _rooms[roomId];
        return gameRoom.Info.RoomMasterPlayerId == clientSession.SessionId;
    }
}