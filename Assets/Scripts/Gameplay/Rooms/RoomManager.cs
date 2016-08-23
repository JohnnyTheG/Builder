using UnityEngine;
using System.Collections.Generic;

public class RoomManager : Singleton<RoomManager>
{
	List<RoomInfo> m_lstRooms = new List<RoomInfo>();

	public void RegisterRoom(GridInfo[] acRoomGrid)
	{
		List<GridInfo> lstRoomGrid = new List<GridInfo>(acRoomGrid);

		List<RoomInfo> lstRoomsContaining = new List<RoomInfo>();

		for (int nGridInfo = 0; nGridInfo < acRoomGrid.Length; nGridInfo++)
		{
			GridInfo cGridInfo = acRoomGrid[nGridInfo];

			RoomInfo[] acRoomsContaining = null;

			// Get all rooms containing that grid square.
			GetRoomsContaining(cGridInfo, out acRoomsContaining);

			for (int nRoom = 0; nRoom < acRoomsContaining.Length; nRoom++)
			{
				RoomInfo cContainingRoom = acRoomsContaining[nRoom];

				if (!lstRoomsContaining.Contains(cContainingRoom))
				{
					lstRoomsContaining.Add(cContainingRoom);
				}
			}
		}

		if (lstRoomsContaining.Count > 0)
		{
			for (int nRoom = 0; nRoom < lstRoomsContaining.Count; nRoom++)
			{
				RoomInfo cContainingRoomInfo = lstRoomsContaining[nRoom];

				lstRoomGrid.AddRange(cContainingRoomInfo.RoomGrid);

				m_lstRooms.Remove(cContainingRoomInfo);
			}

			Debug.Log("RoomManager: Combining Rooms While Mapping");
		}

		// No rooms contain the grid square.
		// This is a new room.
		RoomInfo cRoomInfo = new RoomInfo();

		cRoomInfo.RegisterRoomGrid(lstRoomGrid.ToArray());

		m_lstRooms.Add(cRoomInfo);

		Debug.Log("RoomManager: Room Mapped");
	}

	public void DeregisterRoom(GridInfo[] acRoomGrid)
	{
		for (int nRoom = m_lstRooms.Count - 1; nRoom >= 0; nRoom--)
		{
			RoomInfo cRoomInfo = m_lstRooms[nRoom];

			cRoomInfo.DeregisterRoomGrid(acRoomGrid);

			if (cRoomInfo.IsUnmapped)
			{
				m_lstRooms.Remove(cRoomInfo);
			}
		}
	}

	public bool GetRoomsContaining(GridInfo cGridInfo, out RoomInfo[] acRoomsContaining)
	{
		List<RoomInfo> lstRoomsContaining = new List<RoomInfo>();

		// Find all the rooms containing the grid square.
		for (int nRoom = 0; nRoom < m_lstRooms.Count; nRoom++)
		{
			RoomInfo cRoomInfo = m_lstRooms[nRoom];

			if (cRoomInfo.Contains(cGridInfo))
			{
				lstRoomsContaining.Add(cRoomInfo);
			}
		}

		acRoomsContaining = lstRoomsContaining.ToArray();

		return lstRoomsContaining.Count > 0;
	}
}
