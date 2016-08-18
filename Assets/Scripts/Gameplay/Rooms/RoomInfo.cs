using System.Collections.Generic;

public class RoomInfo
{
	//GridInfo[] m_acRoomGrid;

	List<GridInfo> m_lstRoomGrid = new List<GridInfo>();

	public void RegisterRoomGrid(GridInfo[] acRoomGrid)
	{
		m_lstRoomGrid.AddRange(acRoomGrid);
	}

	public void DeregisterRoomGrid(GridInfo[] acRoomGrid)
	{
		for (int nGridInfo = 0; nGridInfo < acRoomGrid.Length; nGridInfo++)
		{
			if (m_lstRoomGrid.Contains(acRoomGrid[nGridInfo]))
			{
				m_lstRoomGrid.Remove(acRoomGrid[nGridInfo]);
			}
		}
    }
}
