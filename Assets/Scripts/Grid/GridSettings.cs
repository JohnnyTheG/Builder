using UnityEngine;
using System.Collections.Generic;

public class GridSettings : Singleton<GridSettings>
{
	public int Length = 20;
	public int Width = 10;

	public GridInfo[,] Grid;

	public GridInfo[] GetGridSelection(GridInfo cGridInfoStart, GridInfo cGridInfoFinish)
	{
		List<GridInfo> lstGridSelection = new List<GridInfo>();

		int nXStart = cGridInfoStart.GridX;
		int nXFinish = cGridInfoFinish.GridX;

		int nYStart = cGridInfoStart.GridY;
		int nYFinish = cGridInfoFinish.GridY;

		for (int nX = nXStart; nX <= nXFinish; nX++)
		{
			for (int nY = nYStart; nY <= nYFinish; nY++)
			{
				lstGridSelection.Add(Grid[nX, nY]);
			}
		}

		return lstGridSelection.ToArray();
	}
} 
