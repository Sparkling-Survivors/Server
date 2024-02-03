using Google.Protobuf.Protocol;

namespace Server.Game;

public class Player
{ //현재 Info.playerId는 sessionId와 같게 처리하고 있음. (GameRoom의 EnterRoom에서 처리)
    public PlayerInfo Info { get; set; } = new PlayerInfo();
    public GameRoom Room { get; set; }
    public ClientSession Session { get; set; }
}