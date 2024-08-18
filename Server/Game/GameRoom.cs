using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.Protocol;

namespace Server.Game;

public class GameRoom
{
    //object _lock = new object(); 패킷큐 사용으로 인해 불필요
    public RoomInfo Info { get; set; } = new RoomInfo();
    public string Password { get; set; }

    public List<Player> _players = new List<Player>(); //방에 있는 플레이어 리스트
    public List<int> _readyPlayerId = new List<int>(); //레디 완료한 플레이어id 리스트

    public DedicatedServerInfo _dedicatedServerInfo;  //해당 방에 연결된 데디케이티드 서버 정보
    
    public void BroadCast(IMessage packet)
    {
        foreach (Player player in _players)
        {
            player.Session.Send(packet);
        }
    }
    public void EnterRoom(ClientSession session, string name)
    {
        if (session == null)
            return;

        //해당 세션이 이미 다른방에 있는 경우, 그 방에서 먼저 나가게 함
        if (session.MyPlayer != null)
            session.MyPlayer.Room.LeaveRoom(session);

        SC_AllowEnterRoom allowEnterPacket = new SC_AllowEnterRoom();
        SC_InformNewFaceInRoom informNewFaceInRoomPacket = new SC_InformNewFaceInRoom();

        Player newPlayer = new Player();
        newPlayer.Info.PlayerId = session.SessionId;
        newPlayer.Info.Name = name;
        newPlayer.Room = this;
        newPlayer.Session = session;
        session.MyPlayer = newPlayer; //세션에 플레이어 정보 저장

        //현재 방의 인원수 업데이트
        _players.Add(newPlayer);
        Info.CurrentCount = _players.Count;

        //본인한테 입장 허용 패킷 보냄
        allowEnterPacket.CanEnter = true;
        allowEnterPacket.MyPlayerId = newPlayer.Info.PlayerId;
        allowEnterPacket.Room = Info;
        allowEnterPacket.Players.AddRange(_players.ConvertAll(player => player.Info));
        allowEnterPacket.Password = Password;
        newPlayer.Session.Send(allowEnterPacket);

        //다른 사람들한테 새로운 유저 입장 정보 전송
        informNewFaceInRoomPacket.RoomId = Info.RoomId;
        informNewFaceInRoomPacket.CurrentCount = Info.CurrentCount;
        informNewFaceInRoomPacket.NewPlayer = newPlayer.Info;
        foreach (Player p in _players)
        {
            if (p != newPlayer)
                p.Session.Send(informNewFaceInRoomPacket);
        }

        //준비 관련 정보 패킷 본인(방금 들어온)한테 보냄
        newPlayer.Session.Send(MakeReadyRoomPacket());

    }

    public void LeaveRoom(ClientSession session)
    {
        if (session == null)
            return;

        Player player = _players.Find(x => x.Session == session);
        if (player == null)
            return;

        SC_LeaveRoom leavePacket = new SC_LeaveRoom();
        leavePacket.PlayerId = player.Info.PlayerId;

        //만약 방장이 나간거면, 방장을 다른 사람에게 넘겨줌
        if (player.Info.PlayerId == Info.RoomMasterPlayerId && _players.Count > 1)
        {
            Info.RoomMasterPlayerId = _players.Find(x => x.Info.PlayerId != Info.RoomMasterPlayerId).Info.PlayerId;
            leavePacket.RoomMasterPlayerId = Info.RoomMasterPlayerId;
        }
        //방장이 아닌 사람이 나갔을때는 방장이 바뀌지 않음 or 1명남았는데 그 사람이 나갔을때 
        else
        {
            leavePacket.RoomMasterPlayerId = Info.RoomMasterPlayerId;
        }

        //본인 포함 방 인원 모두한테 나갔다는 정보 전송
        BroadCast(leavePacket);

        //나간 사람이 레디 목록에 있었다면 제거
        if (_readyPlayerId.Contains(player.Info.PlayerId))
            _readyPlayerId.Remove(player.Info.PlayerId);

        _players.Remove(player);
        player.Room = null;

        //방장이 레디 목록에 있었다면 제거
        if (_readyPlayerId.Contains(Info.RoomMasterPlayerId))
            _readyPlayerId.Remove(Info.RoomMasterPlayerId);
        //본인 포함 방 인원 모두한테 레디 정보 패킷을 전송
        BroadCast(MakeReadyRoomPacket());

        //현재 방의 인원수 업데이트
        Info.CurrentCount = _players.Count;

        //해당 클라세션에서 플레이어 정보 삭제
        session.MyPlayer = null;

        //만약에 방에 아무도 없게된다면 방을 삭제함
        if (_players.Count <= 0)
            RoomManager.Instance.Remove(Info.RoomId);

    }
    public void ProcessReady(int playerId, bool isReady)
    {
        if (isReady)
        {
            if (!_readyPlayerId.Contains(playerId))
                _readyPlayerId.Add(playerId);
        }
        else
        {
            if (_readyPlayerId.Contains(playerId))
                _readyPlayerId.Remove(playerId);
        }

        //본인 포함 방 인원 모두한테 레디 정보 패킷을 전송
        BroadCast(MakeReadyRoomPacket());
    }

    /// <summary>
    /// SC_ReadyRoom 패킷을 현재 정보를 바탕으로 만드는 함수
    /// </summary>
    /// <returns>완성된 패킷</returns>
    public SC_ReadyRoom MakeReadyRoomPacket()
    {
        SC_ReadyRoom readyRoomPacket = new SC_ReadyRoom();
        readyRoomPacket.RoomId = Info.RoomId;
        
        //_players에 있는 playerid들을 map의 key로하고, 해당 key값이 _readyPlayerId에 존재하면 valu로 true, 아니면 false를 넣어서 만들음
        foreach (Player player in _players)
        {
            readyRoomPacket.ReadyPlayerInfo.Add(player.Info.PlayerId, _readyPlayerId.Contains(player.Info.PlayerId));
        }

        return readyRoomPacket;
    }
    
    public bool IsAllReady()
    {
        if (_players.Count <= 1) //1인으로는 시작 못하게 방지
            return false;

        //방장 빼고 나머지는 다 isReady가 true인지 확인
        return _players.Count(x => x.Info.PlayerId != Info.RoomMasterPlayerId && _readyPlayerId.Contains(x.Info.PlayerId)) == Info.CurrentCount - 1;
    }
}
