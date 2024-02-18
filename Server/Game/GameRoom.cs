using Google.Protobuf.Collections;
using Google.Protobuf.Protocol;

namespace Server.Game;

public class GameRoom
{
    object _lock = new object();
    public RoomInfo Info { get; set; } = new RoomInfo();
    public string Password { get; set; }

    public List<Player> _players = new List<Player>(); //방에 있는 플레이어 리스트

    public DedicatedServerInfo _dedicatedServerInfo;  //해당 방에 연결된 데디케이티드 서버 정보
    

    public void EnterRoom(ClientSession session, string name)
    {
        if (session == null)
            return;

        SC_AllowEnterRoom allowEnterPacket = new SC_AllowEnterRoom();
        SC_InformNewFaceInRoom informNewFaceInRoomPacket = new SC_InformNewFaceInRoom();

        lock (_lock)    
        {
            Player newPlayer = new Player();
            newPlayer.Info.PlayerId = session.SessionId;
            newPlayer.Info.Name = name;
            newPlayer.Info.Transform = null;
            newPlayer.Room = this;
            newPlayer.Session = session;
            session.MyPlayer= newPlayer; //세션에 플레이어 정보 저장
            
            //현재 방의 인원수 업데이트
            _players.Add(newPlayer);
            Info.CurrentCount = _players.Count;
               
            //본인한테 입장 허용 패킷 보냄
            allowEnterPacket.CanEnter = true;
            allowEnterPacket.MyPlayerId = newPlayer.Info.PlayerId;
            allowEnterPacket.Room = Info;
            allowEnterPacket.Players.AddRange(_players.ConvertAll(player => player.Info));
            newPlayer.Session.Send(allowEnterPacket);

            //다른 사람들한테 새로운 유저 입장 정보 전송
            informNewFaceInRoomPacket.RoomId = Info.RoomId;
            informNewFaceInRoomPacket.CurrentCount = Info.CurrentCount;
            informNewFaceInRoomPacket.NewPlayer = newPlayer.Info;
            foreach (Player p in _players)
            {
                if(p!=newPlayer)
                    p.Session.Send(informNewFaceInRoomPacket);
            }
        }
    }

    public void LeaveRoom(ClientSession session)
    {
        if (session == null)
            return;

        lock (_lock)
        {
            Player player = _players.Find(x => x.Session == session);
            if (player == null)
                return;

            SC_LeaveRoom leavePacket = new SC_LeaveRoom();
            leavePacket.PlayerId = player.Info.PlayerId;
            
            //본인 포함 방 인원 모두한테 나갔다는 정보 전송
            foreach (Player p in _players)
                p.Session.Send(leavePacket);

            _players.Remove(player);
            player.Room = null;
            
            //현재 방의 인원수 업데이트
            Info.CurrentCount = _players.Count;
            
            //해당 클라세션에서 플레이어 정보 삭제
            session.MyPlayer = null;
            
            if( _players.Count <= 0)
                RoomManager.Instance.Remove(Info.RoomId);
        }
    }
    
}
