using Google.Protobuf;
using Google.Protobuf.Protocol;
using ServerCore;
using System;
using System.Collections.Generic;

class PacketManager
{
	#region Singleton
	static PacketManager _instance = new PacketManager();
	public static PacketManager Instance { get { return _instance; } }
	#endregion

	PacketManager()
	{
		Register();
	}

	Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>> _onRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>>();
	Dictionary<ushort, Action<PacketSession, IMessage>> _handler = new Dictionary<ushort, Action<PacketSession, IMessage>>();

	public Action<PacketSession, IMessage, ushort> CustomHandler { get; set; }
		
	public void Register()
	{		
		_onRecv.Add((ushort)MsgId.CsRoomList, MakePacket<CS_RoomList>);
		_handler.Add((ushort)MsgId.CsRoomList, PacketHandler.CS_RoomListHandler);		
		_onRecv.Add((ushort)MsgId.CsMakeRoom, MakePacket<CS_MakeRoom>);
		_handler.Add((ushort)MsgId.CsMakeRoom, PacketHandler.CS_MakeRoomHandler);		
		_onRecv.Add((ushort)MsgId.CsEnterRoom, MakePacket<CS_EnterRoom>);
		_handler.Add((ushort)MsgId.CsEnterRoom, PacketHandler.CS_EnterRoomHandler);		
		_onRecv.Add((ushort)MsgId.CsLeaveRoom, MakePacket<CS_LeaveRoom>);
		_handler.Add((ushort)MsgId.CsLeaveRoom, PacketHandler.CS_LeaveRoomHandler);		
		_onRecv.Add((ushort)MsgId.CsPingPong, MakePacket<CS_PingPong>);
		_handler.Add((ushort)MsgId.CsPingPong, PacketHandler.CS_PingPongHandler);		
		_onRecv.Add((ushort)MsgId.CsQuitUnity, MakePacket<CS_QuitUnity>);
		_handler.Add((ushort)MsgId.CsQuitUnity, PacketHandler.CS_QuitUnityHandler);
	}

	public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
	{
		ushort count = 0;

		ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
		count += 2;
		ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
		count += 2;

		Action<PacketSession, ArraySegment<byte>, ushort> action = null;
		if (_onRecv.TryGetValue(id, out action))
			action.Invoke(session, buffer, id);
	}

	void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer, ushort id) where T : IMessage, new()
	{
		T pkt = new T();
		pkt.MergeFrom(buffer.Array, buffer.Offset + 4, buffer.Count - 4);

		//유니티 메인쓰레드 실행용(OnConnected에서 구현)
		if (CustomHandler != null)
		{
			CustomHandler.Invoke(session, pkt, id);
		}
		else
		{
			Action<PacketSession, IMessage> action = null;
			if (_handler.TryGetValue(id, out action))
				action.Invoke(session, pkt);
		}
	}

	public Action<PacketSession, IMessage> GetPacketHandler(ushort id)
	{
		Action<PacketSession, IMessage> action = null;
		if (_handler.TryGetValue(id, out action))
			return action;
		return null;
	}
}