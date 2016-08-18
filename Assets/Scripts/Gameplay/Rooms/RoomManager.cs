using UnityEngine;
using System.Collections.Generic;

public class RoomManager : Singleton<RoomManager>
{
	List<RoomInfo> m_lstRooms = new List<RoomInfo>();

	public void RegisterRoom(GridInfo[] acRoomGrid)
	{
		RoomInfo cRoomInfo = new RoomInfo(acRoomGrid);

		m_lstRooms.Add(cRoomInfo);

		Debug.Log("RoomManager: Room Mapped");
	}
}
