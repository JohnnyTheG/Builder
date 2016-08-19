using System.Collections.Generic;

public class RoomInfo
{
	//GridInfo[] m_acRoomGrid;

	List<GridInfo> m_lstRoomGrid = new List<GridInfo>();

	public void RegisterRoomGrid(GridInfo[] acRoomGrid)
	{
		m_lstRoomGrid.AddRange(acRoomGrid);

		for (int nGridInfo = 0; nGridInfo < m_lstRoomGrid.Count; nGridInfo++)
		{
			m_lstRoomGrid[nGridInfo].SetInRoom();
		}
	}

	public void DeregisterRoomGrid(GridInfo[] acRoomGrid)
	{
		for (int nGridInfo = 0; nGridInfo < acRoomGrid.Length; nGridInfo++)
		{
			if (m_lstRoomGrid.Contains(acRoomGrid[nGridInfo]))
			{
				acRoomGrid[nGridInfo].SetNotInRoom();

				m_lstRoomGrid.Remove(acRoomGrid[nGridInfo]);
			}
		}
    }
}
