using UnityEngine;
using System.Collections.Generic;

public class GridSettings : Singleton<GridSettings>
{
	public int YSize = 20;
	public int XSize = 10;

	public GridInfo[,] Grid;

	new public void Awake()
	{
		base.Awake();

		Grid = new GridInfo[XSize, YSize];

		GridInfo[] acGridInfo = FindObjectsOfType<GridInfo>();

		for (int nGridInfo = 0; nGridInfo < acGridInfo.Length; nGridInfo++)
		{
			GridInfo cGridInfo = acGridInfo[nGridInfo];

			Grid[cGridInfo.GridX, cGridInfo.GridY] = cGridInfo;
		}
	}

	public GridInfo[] GetGridSelection(GridInfo cGridInfoStart, GridInfo cGridInfoFinish)
	{
		List<GridInfo> lstGridSelection = new List<GridInfo>();

		int nXMin = cGridInfoStart.GridX <= cGridInfoFinish.GridX ? cGridInfoStart.GridX : cGridInfoFinish.GridX;
		int nXMax = cGridInfoStart.GridX >= cGridInfoFinish.GridX ? cGridInfoStart.GridX : cGridInfoFinish.GridX;

		int nYMin = cGridInfoStart.GridY <= cGridInfoFinish.GridY ? cGridInfoStart.GridY : cGridInfoFinish.GridY;
		int nYMax = cGridInfoStart.GridY >= cGridInfoFinish.GridY ? cGridInfoStart.GridY : cGridInfoFinish.GridY;

		for (int nX = nXMin; nX <= nXMax; nX++)
		{
			for (int nY = nYMin; nY <= nYMax; nY++)
			{
				lstGridSelection.Add(Grid[nX, nY]);
			}
		}

		return lstGridSelection.ToArray();
	}
} 
