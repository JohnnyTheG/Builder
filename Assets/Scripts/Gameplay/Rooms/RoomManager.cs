using UnityEngine;
using System.Collections.Generic;

public class RoomManager : Singleton<RoomManager>
{
	List<RoomInfo> m_lstRooms = new List<RoomInfo>();

	public void RegisterRoom(GridInfo[] acRoomGrid)
	{
		RoomInfo cRoomInfo = new RoomInfo();

		cRoomInfo.RegisterRoomGrid(acRoomGrid);

		m_lstRooms.Add(cRoomInfo);

		Debug.Log("RoomManager: Room Mapped");
	}

	public void DeregisterRoom(GridInfo[] acRoomGrid)
	{
		for (int nRoom = 0; nRoom < m_lstRooms.Count; nRoom++)
		{
			RoomInfo cRoomInfo = m_lstRooms[nRoom];

			cRoomInfo.DeregisterRoomGrid(acRoomGrid);
		}
	}
}
