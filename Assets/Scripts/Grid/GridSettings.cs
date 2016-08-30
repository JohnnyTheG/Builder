using UnityEngine;
using System.Collections.Generic;

public class GridSettings : Singleton<GridSettings>
{
	public int YSize = 20;
	public int XSize = 10;

	public GridInfo[,] Grid;

	bool m_bGridFlipInProgress = false;

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

	public void Flip()
	{
		//transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0.0f, 0.0f, 180.0f));

		if (!m_bGridFlipInProgress)
		{
			m_bGridFlipInProgress = true;

			m_fFlipTime = 0.0f;

			/*m_quatStart = transform.rotation;

			m_quatFinish = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0.0f, 0.0f, 180.0f));*/

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
	}
}
