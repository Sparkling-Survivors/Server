using Google.Protobuf;
using Google.Protobuf.Protocol;
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
		_onRecv.Add((ushort)MsgId.ScRoomList, MakePacket<SC_RoomList>);
		_handler.Add((ushort)MsgId.ScRoomList, PacketHandler.SC_RoomListHandler);		
		_onRecv.Add((ushort)MsgId.ScMakeRoom, MakePacket<SC_MakeRoom>);
		_handler.Add((ushort)MsgId.ScMakeRoom, PacketHandler.SC_MakeRoomHandler);		
		_onRecv.Add((ushort)MsgId.ScAllowEnterRoom, MakePacket<SC_AllowEnterRoom>);
		_handler.Add((ushort)MsgId.ScAllowEnterRoom, PacketHandler.SC_AllowEnterRoomHandler);		
		_onRecv.Add((ushort)MsgId.ScInformNewFaceInRoom, MakePacket<SC_InformNewFaceInRoom>);
		_handler.Add((ushort)MsgId.ScInformNewFaceInRoom, PacketHandler.SC_InformNewFaceInRoomHandler);		
		_onRecv.Add((ushort)MsgId.ScLeaveRoom, MakePacket<SC_LeaveRoom>);
		_handler.Add((ushort)MsgId.ScLeaveRoom, PacketHandler.SC_LeaveRoomHandler);		
		_onRecv.Add((ushort)MsgId.ScReadyRoom, MakePacket<SC_ReadyRoom>);
		_handler.Add((ushort)MsgId.ScReadyRoom, PacketHandler.SC_ReadyRoomHandler);		
		_onRecv.Add((ushort)MsgId.ScPingPong, MakePacket<SC_PingPong>);
		_handler.Add((ushort)MsgId.ScPingPong, PacketHandler.SC_PingPongHandler);		
		_onRecv.Add((ushort)MsgId.DscPingPong, MakePacket<DSC_PingPong>);
		_handler.Add((ushort)MsgId.DscPingPong, PacketHandler.DSC_PingPongHandler);		
		_onRecv.Add((ushort)MsgId.ScConnectDedicatedServer, MakePacket<SC_ConnectDedicatedServer>);
		_handler.Add((ushort)MsgId.ScConnectDedicatedServer, PacketHandler.SC_ConnectDedicatedServerHandler);		
		_onRecv.Add((ushort)MsgId.DscAllowEnterGame, MakePacket<DSC_AllowEnterGame>);
		_handler.Add((ushort)MsgId.DscAllowEnterGame, PacketHandler.DSC_AllowEnterGameHandler);		
		_onRecv.Add((ushort)MsgId.DscInformNewFaceInDedicatedServer, MakePacket<DSC_InformNewFaceInDedicatedServer>);
		_handler.Add((ushort)MsgId.DscInformNewFaceInDedicatedServer, PacketHandler.DSC_InformNewFaceInDedicatedServerHandler);		
		_onRecv.Add((ushort)MsgId.DscInformLeaveDedicatedServer, MakePacket<DSC_InformLeaveDedicatedServer>);
		_handler.Add((ushort)MsgId.DscInformLeaveDedicatedServer, PacketHandler.DSC_InformLeaveDedicatedServerHandler);		
		_onRecv.Add((ushort)MsgId.DscStartGame, MakePacket<DSC_StartGame>);
		_handler.Add((ushort)MsgId.DscStartGame, PacketHandler.DSC_StartGameHandler);		
		_onRecv.Add((ushort)MsgId.DscMove, MakePacket<DSC_Move>);
		_handler.Add((ushort)MsgId.DscMove, PacketHandler.DSC_MoveHandler);		
		_onRecv.Add((ushort)MsgId.DscDayTimerStart, MakePacket<DSC_DayTimerStart>);
		_handler.Add((ushort)MsgId.DscDayTimerStart, PacketHandler.DSC_DayTimerStartHandler);		
		_onRecv.Add((ushort)MsgId.DscDayTimerSync, MakePacket<DSC_DayTimerSync>);
		_handler.Add((ushort)MsgId.DscDayTimerSync, PacketHandler.DSC_DayTimerSyncHandler);		
		_onRecv.Add((ushort)MsgId.DscDayTimerEnd, MakePacket<DSC_DayTimerEnd>);
		_handler.Add((ushort)MsgId.DscDayTimerEnd, PacketHandler.DSC_DayTimerEndHandler);		
		_onRecv.Add((ushort)MsgId.DscNightTimerStart, MakePacket<DSC_NightTimerStart>);
		_handler.Add((ushort)MsgId.DscNightTimerStart, PacketHandler.DSC_NightTimerStartHandler);		
		_onRecv.Add((ushort)MsgId.DscNightTimerSync, MakePacket<DSC_NightTimerSync>);
		_handler.Add((ushort)MsgId.DscNightTimerSync, PacketHandler.DSC_NightTimerSyncHandler);		
		_onRecv.Add((ushort)MsgId.DscNightTimerEnd, MakePacket<DSC_NightTimerEnd>);
		_handler.Add((ushort)MsgId.DscNightTimerEnd, PacketHandler.DSC_NightTimerEndHandler);		
		_onRecv.Add((ushort)MsgId.DscNewChestsInfo, MakePacket<DSC_NewChestsInfo>);
		_handler.Add((ushort)MsgId.DscNewChestsInfo, PacketHandler.DSC_NewChestsInfoHandler);		
		_onRecv.Add((ushort)MsgId.DscChestOpenSuccess, MakePacket<DSC_ChestOpenSuccess>);
		_handler.Add((ushort)MsgId.DscChestOpenSuccess, PacketHandler.DSC_ChestOpenSuccessHandler);		
		_onRecv.Add((ushort)MsgId.DscResponseTimestamp, MakePacket<DSC_ResponseTimestamp>);
		_handler.Add((ushort)MsgId.DscResponseTimestamp, PacketHandler.DSC_ResponseTimestampHandler);		
		_onRecv.Add((ushort)MsgId.DscGaugeSync, MakePacket<DSC_GaugeSync>);
		_handler.Add((ushort)MsgId.DscGaugeSync, PacketHandler.DSC_GaugeSyncHandler);		
		_onRecv.Add((ushort)MsgId.DscNewCleansesInfo, MakePacket<DSC_NewCleansesInfo>);
		_handler.Add((ushort)MsgId.DscNewCleansesInfo, PacketHandler.DSC_NewCleansesInfoHandler);		
		_onRecv.Add((ushort)MsgId.DscGiveCleansePermission, MakePacket<DSC_GiveCleansePermission>);
		_handler.Add((ushort)MsgId.DscGiveCleansePermission, PacketHandler.DSC_GiveCleansePermissionHandler);		
		_onRecv.Add((ushort)MsgId.DscCleanseQuit, MakePacket<DSC_CleanseQuit>);
		_handler.Add((ushort)MsgId.DscCleanseQuit, PacketHandler.DSC_CleanseQuitHandler);		
		_onRecv.Add((ushort)MsgId.DscCleanseSuccess, MakePacket<DSC_CleanseSuccess>);
		_handler.Add((ushort)MsgId.DscCleanseSuccess, PacketHandler.DSC_CleanseSuccessHandler);		
		_onRecv.Add((ushort)MsgId.DscCleanseCooltimeFinish, MakePacket<DSC_CleanseCooltimeFinish>);
		_handler.Add((ushort)MsgId.DscCleanseCooltimeFinish, PacketHandler.DSC_CleanseCooltimeFinishHandler);		
		_onRecv.Add((ushort)MsgId.DscItemBuyResult, MakePacket<DSC_ItemBuyResult>);
		_handler.Add((ushort)MsgId.DscItemBuyResult, PacketHandler.DSC_ItemBuyResultHandler);		
		_onRecv.Add((ushort)MsgId.DscOnHoldItem, MakePacket<DSC_OnHoldItem>);
		_handler.Add((ushort)MsgId.DscOnHoldItem, PacketHandler.DSC_OnHoldItemHandler);		
		_onRecv.Add((ushort)MsgId.DscUseDashItem, MakePacket<DSC_UseDashItem>);
		_handler.Add((ushort)MsgId.DscUseDashItem, PacketHandler.DSC_UseDashItemHandler);		
		_onRecv.Add((ushort)MsgId.DscEndDashItem, MakePacket<DSC_EndDashItem>);
		_handler.Add((ushort)MsgId.DscEndDashItem, PacketHandler.DSC_EndDashItemHandler);		
		_onRecv.Add((ushort)MsgId.DscUseFireworkItem, MakePacket<DSC_UseFireworkItem>);
		_handler.Add((ushort)MsgId.DscUseFireworkItem, PacketHandler.DSC_UseFireworkItemHandler);		
		_onRecv.Add((ushort)MsgId.DscUseInvisibleItem, MakePacket<DSC_UseInvisibleItem>);
		_handler.Add((ushort)MsgId.DscUseInvisibleItem, PacketHandler.DSC_UseInvisibleItemHandler);		
		_onRecv.Add((ushort)MsgId.DscUseFlashlightItem, MakePacket<DSC_UseFlashlightItem>);
		_handler.Add((ushort)MsgId.DscUseFlashlightItem, PacketHandler.DSC_UseFlashlightItemHandler);		
		_onRecv.Add((ushort)MsgId.DscEndFlashlightItem, MakePacket<DSC_EndFlashlightItem>);
		_handler.Add((ushort)MsgId.DscEndFlashlightItem, PacketHandler.DSC_EndFlashlightItemHandler);		
		_onRecv.Add((ushort)MsgId.DscOnHitFlashlightItem, MakePacket<DSC_OnHitFlashlightItem>);
		_handler.Add((ushort)MsgId.DscOnHitFlashlightItem, PacketHandler.DSC_OnHitFlashlightItemHandler);		
		_onRecv.Add((ushort)MsgId.DscUseTrapItem, MakePacket<DSC_UseTrapItem>);
		_handler.Add((ushort)MsgId.DscUseTrapItem, PacketHandler.DSC_UseTrapItemHandler);		
		_onRecv.Add((ushort)MsgId.DscOnHitTrapItem, MakePacket<DSC_OnHitTrapItem>);
		_handler.Add((ushort)MsgId.DscOnHitTrapItem, PacketHandler.DSC_OnHitTrapItemHandler);		
		_onRecv.Add((ushort)MsgId.DscUseHeartlessSkill, MakePacket<DSC_UseHeartlessSkill>);
		_handler.Add((ushort)MsgId.DscUseHeartlessSkill, PacketHandler.DSC_UseHeartlessSkillHandler);		
		_onRecv.Add((ushort)MsgId.DscUseDetectorSkill, MakePacket<DSC_UseDetectorSkill>);
		_handler.Add((ushort)MsgId.DscUseDetectorSkill, PacketHandler.DSC_UseDetectorSkillHandler);		
		_onRecv.Add((ushort)MsgId.DscEndGame, MakePacket<DSC_EndGame>);
		_handler.Add((ushort)MsgId.DscEndGame, PacketHandler.DSC_EndGameHandler);
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