using UnityEngine;
using System.Collections.Generic;

public class GridSettings : Singleton<GridSettings>
{
	public int YSize = 20;
	public int XSize = 10;

	public GridInfo[,] Grid;

	bool m_bGridFlipInProgress = false;

	//bool m_bGridRefresh = false;

	[SerializeField]
	float GridFlipDuration = 1.0f;

	float m_fFlipTime = 0.0f;

	Quaternion m_quatStart;
	Quaternion m_quatFinish;

	Quaternion m_quatTopUp = Quaternion.Euler(Vector3.zero);
	Quaternion m_quatBottomUp = Quaternion.Euler(new Vector3(0.0f, 0.0f, 180.0f));

	// Which build layer is "up" (i.e. facing upwards towards Vector3.up).
	GridInfo.BuildLayer m_eUpBuildLayer = GridInfo.BuildLayer.Top;
	public GridInfo.BuildLayer UpBuildLayer
	{
		get
		{
			return m_eUpBuildLayer;
		}
	}

	public delegate void OnGridFlipStartCallback();
	public delegate void OnGridFlipCompleteCallback();

	public event OnGridFlipStartCallback OnGridFlipStart;
	public event OnGridFlipCompleteCallback OnGridFlipComplete;

	new public void Awake()
	{
		base.Awake();

		Grid = new GridInfo[XSize, YSize];

		// Get all the grid infos in the scene.
		GridInfo[] acGridInfo = FindObjectsOfType<GridInfo>();

		// All grid info in the scene have their coordinates pre-set.
		// Fill the grid array with the correct block for the correct grid entry.
		for (int nGridInfo = 0; nGridInfo < acGridInfo.Length; nGridInfo++)
		{
			GridInfo cGridInfo = acGridInfo[nGridInfo];

			Grid[cGridInfo.GridX, cGridInfo.GridY] = cGridInfo;
		}

		// We now have a populated grid.
		// Iterate the grid and set the build slot infos in touching pairs.

		for (int nGridX = 0; nGridX < Grid.GetLength(0); nGridX++)
		{
			for (int nGridY = 0; nGridY < Grid.GetLength(1); nGridY++)
			{
				//GridInfo.BuildSlotInfo cBuildSlotInfo = new GridInfo.BuildSlotInfo();

				GridInfo cGridInfo = Grid[nGridX, nGridY];

				if (cGridInfo != null)
				{
					foreach (KeyValuePair<GridInfo.BuildLayer, GridInfo.BuildLayerInfo> cLayerPair in cGridInfo.m_dictBuildLayers)
					{
						GridInfo.BuildLayer eBuildLayer = cLayerPair.Key;

						List<GridInfo.BuildSlot> lstBuildSlots = new List<GridInfo.BuildSlot>(cLayerPair.Value.m_dictBuildSlotOccupiers.Keys);

						for(int nBuildSlot = 0; nBuildSlot < lstBuildSlots.Count; nBuildSlot++)
						{
							GridInfo.BuildSlot eBuildSlot = lstBuildSlots[nBuildSlot];

							GridInfo.BuildSlotInfo cBuildSlotInfo = new GridInfo.BuildSlotInfo();

							// Set the slot info for the current grid info.
							cGridInfo.SetBuildSlotInfo(cBuildSlotInfo, eBuildSlot, eBuildLayer);

							// See if there is a touching grid info on the side being set.
                            GridInfo cTouchingGridInfo = GetTouchingGridInfo(cGridInfo, eBuildSlot, eBuildLayer);

							if (cTouchingGridInfo != null)
							{
								// If there is a touching info, then set the opposite slot on that one so they share the same reference.
								cTouchingGridInfo.SetBuildSlotInfo(cBuildSlotInfo, GridUtilities.GetOppositeBuildSlot(eBuildSlot), eBuildLayer);
                            }
						}
					}
				}
			}
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

	public GridInfo[] GetGridLine(GridInfo cGridInfoStart, GridInfo cGridInfoFinish)
	{
		List<GridInfo> lstGridSelection = new List<GridInfo>();

		int nXMin = cGridInfoStart.GridX <= cGridInfoFinish.GridX ? cGridInfoStart.GridX : cGridInfoFinish.GridX;
		int nXMax = cGridInfoStart.GridX >= cGridInfoFinish.GridX ? cGridInfoStart.GridX : cGridInfoFinish.GridX;

		int nYMin = cGridInfoStart.GridY <= cGridInfoFinish.GridY ? cGridInfoStart.GridY : cGridInfoFinish.GridY;
		int nYMax = cGridInfoStart.GridY >= cGridInfoFinish.GridY ? cGridInfoStart.GridY : cGridInfoFinish.GridY;

		int nXDelta = nXMax - nXMin;
		int nYDelta = nYMax - nYMin;

		if (nXDelta >= nYDelta)
		{
			for (int nX = nXMin; nX <= nXMax; nX++)
			{
				lstGridSelection.Add(Grid[nX, cGridInfoStart.GridY]);
			}
		}
		else
		{
			for (int nY = nYMin; nY <= nYMax; nY++)
			{
				lstGridSelection.Add(Grid[cGridInfoStart.GridX, nY]);
			}
		}

		return lstGridSelection.ToArray();
	}

	public GridInfo GetTouchingGridInfo(GridInfo cGridInfo, GridInfo.BuildSlot eBuildSlot, GridInfo.BuildLayer eBuildLayer)
	{
		int nX = cGridInfo.GridX;
		int nY = cGridInfo.GridY;

		GridInfo cTouchingGridInfo = null;

		// Ensure the grid info exists itself.
		if (Grid[nX, nY] != null)
		{
			int nXIncrement = 0;
			int nYIncrement = 0;

			switch (eBuildSlot)
			{
				case GridInfo.BuildSlot.North:

					nYIncrement -= 1;

					break;

				case GridInfo.BuildSlot.East:

					nXIncrement += 1;

					break;

				case GridInfo.BuildSlot.South:

					nYIncrement += 1;

					break;

				case GridInfo.BuildSlot.West:

					nXIncrement -= 1;

					break;
			}

			try
			{
				cTouchingGridInfo = Grid[nX + nXIncrement, nY + nYIncrement];
			}
			catch
			{
				cTouchingGridInfo = null;
			}
		}

		return cTouchingGridInfo;
	}

	public GridInfo[] GetAllTouchingGridInfo(GridInfo cGridInfo, GridInfo.BuildLayer eBuildLayer)
	{
		List<GridInfo> lstTouchingGridInfo = new List<GridInfo>();

		lstTouchingGridInfo.Add(GetTouchingGridInfo(cGridInfo, GridInfo.BuildSlot.North, eBuildLayer));
		lstTouchingGridInfo.Add(GetTouchingGridInfo(cGridInfo, GridInfo.BuildSlot.East, eBuildLayer));
		lstTouchingGridInfo.Add(GetTouchingGridInfo(cGridInfo, GridInfo.BuildSlot.South, eBuildLayer));
		lstTouchingGridInfo.Add(GetTouchingGridInfo(cGridInfo, GridInfo.BuildSlot.West, eBuildLayer));

		return lstTouchingGridInfo.ToArray();
	}

	public void Flip()
	{
		if (!m_bGridFlipInProgress)
		{
			m_bGridFlipInProgress = true;

			m_fFlipTime = 0.0f;

			switch (m_eUpBuildLayer)
			{
				case GridInfo.BuildLayer.Top:

					m_quatStart = m_quatTopUp;
					m_quatFinish = m_quatBottomUp;

					m_eUpBuildLayer = GridInfo.BuildLayer.Bottom;

					break;

				case GridInfo.BuildLayer.Bottom:

					m_quatStart = m_quatBottomUp;
					m_quatFinish = m_quatTopUp;

					m_eUpBuildLayer = GridInfo.BuildLayer.Top;

					break;
			}

			if (OnGridFlipStart != null)
			{
				OnGridFlipStart();
			}
		}
	}

	public void Update()
	{
		if (m_bGridFlipInProgress)
		{
			m_fFlipTime += Time.deltaTime;

			if (m_fFlipTime >= GridFlipDuration)
			{
				transform.rotation = m_quatFinish;

				m_bGridFlipInProgress = false;

				if (OnGridFlipComplete != null)
				{
					OnGridFlipComplete();
				}
			}
			else
			{
				transform.rotation = Quaternion.Lerp(m_quatStart, m_quatFinish, Easing.EaseInOut(m_fFlipTime / GridFlipDuration, EasingType.Cubic, EasingType.Cubic));
			}
		}

		GridInfo cGridInfo = null;

		for (int nGridX = 0; nGridX < Grid.GetLength(0); nGridX++)
		{
			for (int nGridY = 0; nGridY < Grid.GetLength(1); nGridY++)
			{
				cGridInfo = Grid[nGridX, nGridY];

				if (cGridInfo.Refresh)
				{
					Grid[nGridX, nGridY].RefreshStep1();
				}
			}
		}

		for (int nGridX = 0; nGridX < Grid.GetLength(0); nGridX++)
		{
			for (int nGridY = 0; nGridY < Grid.GetLength(1); nGridY++)
			{
				cGridInfo = Grid[nGridX, nGridY];

				if (cGridInfo.Refresh)
				{
					cGridInfo.RefreshStep2();
				}
			}
		}

		for (int nGridX = 0; nGridX < Grid.GetLength(0); nGridX++)
		{
			for (int nGridY = 0; nGridY < Grid.GetLength(1); nGridY++)
			{
				cGridInfo = Grid[nGridX, nGridY];

				if (cGridInfo.Refresh)
				{
					Grid[nGridX, nGridY].RefreshStep3();
				}
			}
		}

		//m_bGridRefresh = false;
	}

	/*public void RefreshGrid()
	{
		m_bGridRefresh = true;
	}*/

	public GridInfo[] GetSurroundingGridInfo(GridInfo cOriginGridInfo, int nIncrementX, int nIncrementY)
	{
		return Utils.GetSurrounding2DArrayEntries<GridInfo>(Grid, cOriginGridInfo.GridX, cOriginGridInfo.GridY, nIncrementX, nIncrementY);
	}
}
