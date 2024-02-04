using Google.Protobuf.Collections;
using Google.Protobuf.Protocol;

namespace Server.Game;

public class GameRoom
{
    object _lock = new object();
    public RoomInfo Info { get; set; } = new RoomInfo();
    public string Password { get; set; }

    List<Player> _players = new List<Player>(); //방에 있는 플레이어 리스트

    public void EnterRoom(ClientSession session, string name)
    {
        if (session == null)
            return;

        S_AllowEnterRoom allowEnterPacket = new S_AllowEnterRoom();
        S_InformNewFaceInRoom informNewFaceInRoomPacket = new S_InformNewFaceInRoom();

        lock (_lock)    
        {
            Player newPlayer = new Player();
            newPlayer.Info.PlayerId = session.SessionId;
            newPlayer.Info.Name = name;
            newPlayer.Info.Transform = null;
            newPlayer.Room = this;
            newPlayer.Session = session;
            
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

            S_LeaveRoom leavePacket = new S_LeaveRoom();
            leavePacket.PlayerId = player.Info.PlayerId;

            _players.Remove(player);
            player.Room = null;
            
            //현재 방의 인원수 업데이트
            Info.CurrentCount = _players.Count;

            //본인 포함 방 인원 모두한테 나갔다는 정보 전송
            foreach (Player p in _players)
                p.Session.Send(leavePacket);
            
            if( _players.Count <= 0)
                RoomManager.Instance.Remove(Info.RoomId);
        }
    }

    /*public void EnterGame(Player newPlayer)
    {
        if (newPlayer == null)
            return;

        lock (_lock)
        {
            _players.Add(newPlayer);
            newPlayer.Room = this;

            //본인한테 정보 전송
            S_EnterGame enterPacket = new S_EnterGame();
            enterPacket.Player = newPlayer.Info;
            newPlayer.Session.Send(enterPacket);

            S_Spawn spawnPacket = new S_Spawn();
            foreach (Player p in _players)
            {
                if (p != newPlayer)
                    spawnPacket.Players.Add(p.Info);
            }
            newPlayer.Session.Send(spawnPacket);


            //타인한테 정보 전송
            S_Spawn spawnPacket2 = new S_Spawn();
            spawnPacket2.Players.Add(newPlayer.Info);
            foreach (Player p in _players)
            {
                if (p != newPlayer)
                    p.Session.Send(spawnPacket2);
            }
        }
    }

    public void LeaveGame(int playerId)
    {
        lock (_lock)
        {
            Player player = _players.Find(x => x.Info.PlayerId == playerId);
            if(player== null)
                return;

            _players.Remove(player);
            player.Room = null;

            //본인한테 정보 전송
            S_LeaveGame leavePacket = new S_LeaveGame();
            player.Session.Send(leavePacket);

            //타인한테 정보 전송
            S_Despawn despawnPacket = new S_Despawn();
            despawnPacket.PlayerIds.Add(player.Info.PlayerId);
            foreach (Player p in _players)
            {
                if(p!=player)
                    p.Session.Send(despawnPacket);
            }
        }
    }*/
}