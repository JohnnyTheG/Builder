using System.Collections.Generic;

public class RoomInfo
{
	List<GridInfo> m_lstRoomGrid = new List<GridInfo>();
	GridCornerCoordinates m_cGridCornerCoordinates = new GridCornerCoordinates();

	public List<GridInfo> RoomGrid
	{
		get
		{
			return m_lstRoomGrid;
		}
	}

	public bool IsUnmapped
	{
		get
		{
			return m_lstRoomGrid.Count == 0;
		}
	}

	class GridCornerCoordinates
	{
		public int m_nXMin = int.MaxValue;
		public int m_nXMax = int.MinValue;
		public int m_nYMin = int.MaxValue;
		public int m_nYMax = int.MinValue;
	}

	public void RegisterRoomGrid(GridInfo[] acRoomGrid)
	{
		m_lstRoomGrid.AddRange(acRoomGrid);

		for (int nGridInfo = 0; nGridInfo < m_lstRoomGrid.Count; nGridInfo++)
		{
			m_lstRoomGrid[nGridInfo].SetInRoom();
		}

		FindGridCornerCoordinates();
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

	public bool Contains(GridInfo cGridInfo)
	{
		return m_lstRoomGrid.Contains(cGridInfo);
	}

	void FindGridCornerCoordinates()
	{
		for (int nGridInfo = 0; nGridInfo < m_lstRoomGrid.Count; nGridInfo++)
		{
			GridInfo cGridInfo = m_lstRoomGrid[nGridInfo];

			if (cGridInfo.GridX < m_cGridCornerCoordinates.m_nXMin)
			{
				m_cGridCornerCoordinates.m_nXMin = cGridInfo.GridX;
			}

			if (cGridInfo.GridX > m_cGridCornerCoordinates.m_nXMax)
			{
				m_cGridCornerCoordinates.m_nXMax = cGridInfo.GridX;
			}

			if (cGridInfo.GridY < m_cGridCornerCoordinates.m_nYMin)
			{
				m_cGridCornerCoordinates.m_nYMin = cGridInfo.GridY;
			}

			if (cGridInfo.GridY > m_cGridCornerCoordinates.m_nYMax)
			{
				m_cGridCornerCoordinates.m_nYMax = cGridInfo.GridY;
			}
		}
	}

	GridInfo[] FindGridCorners()
	{
		GridInfo[] acGridCorners = new GridInfo[4];

		int nCount = 0;

		for (int nGridInfo = 0; nGridInfo < m_lstRoomGrid.Count; nGridInfo++)
		{
			GridInfo cGridInfo = m_lstRoomGrid[nGridInfo];

			if ((cGridInfo.GridX == m_cGridCornerCoordinates.m_nXMin || cGridInfo.GridX == m_cGridCornerCoordinates.m_nXMax) && (cGridInfo.GridY == m_cGridCornerCoordinates.m_nYMin || cGridInfo.GridY == m_cGridCornerCoordinates.m_nYMax))
			{
				acGridCorners[nCount] = cGridInfo;

				nCount++;
			}
		}

		return acGridCorners;
	}

	GridInfo[] FindGridEdge()
	{
		List<GridInfo> lstGridEdge = new List<GridInfo>();

		for (int nGridInfo = 0; nGridInfo < m_lstRoomGrid.Count; nGridInfo++)
		{
			GridInfo cGridInfo = m_lstRoomGrid[nGridInfo];

			if (cGridInfo.GridX == m_cGridCornerCoordinates.m_nXMin || cGridInfo.GridX == m_cGridCornerCoordinates.m_nXMax || cGridInfo.GridY == m_cGridCornerCoordinates.m_nYMin || cGridInfo.GridY == m_cGridCornerCoordinates.m_nYMax)
			{
				cGridInfo.UnmappingHighlight();

				lstGridEdge.Add(cGridInfo);
			}
		}

		return lstGridEdge.ToArray();
	}

	public bool IsCompleteRoom()
	{
		return false;
	}
}
