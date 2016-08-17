using UnityEngine;
using UnityEditor;
using System.Collections;

public class GridUtilities : EditorWindow
{
	static string m_strGridParentName = "Grid";

	static GameObject m_cGridPrefab;

	[MenuItem("GridUtilities/Create Grid")]
	static void CreateGrid()
	{
		if (m_cGridPrefab == null)
		{
			m_cGridPrefab = (GameObject)Resources.Load("GridPrefab");
		}

		EditorCoroutine.Start(CreateGridCoroutine());
	}

	static IEnumerator CreateGridCoroutine()
	{
		GameObject cGrid = GameObject.Find(m_strGridParentName);

		if (cGrid == null)
		{
			cGrid = new GameObject();

			cGrid.name = m_strGridParentName;
		}

		int nChildren = cGrid.transform.childCount;
		for (int nChild = nChildren - 1; nChild >= 0; nChild--)
		{
			DestroyImmediate(cGrid.transform.GetChild(nChild).gameObject);
		}

		GridSettings cGridSettings = cGrid.GetComponent<GridSettings>();

		if (cGridSettings == null)
		{
			cGridSettings = cGrid.AddComponent<GridSettings>();
		}

		while (cGridSettings == null)
		{
			yield return null;

			cGridSettings = cGrid.GetComponent<GridSettings>();
		}

		cGridSettings.Grid = new GridInfo[cGridSettings.Length, cGridSettings.Width];

		for (int nX = 0; nX < cGridSettings.Width; nX++)
		{
			for (int nY = 0; nY < cGridSettings.Length; nY++)
			{
				GameObject cGridSquare = (GameObject)Instantiate(m_cGridPrefab, new Vector3(nY, 0.0f, nX), Quaternion.identity);

				cGridSquare.transform.parent = cGrid.transform;

				GridInfo cGridInfo = cGridSquare.GetComponent<GridInfo>();

				if (cGridInfo != null)
				{
					cGridInfo.GridX = nX;
					cGridInfo.GridY = nY;

					cGridSettings.Grid[nY, nX] = cGridInfo;
				}
				else
				{
					Debug.LogError("GridUtilities: Spawned Grid prefab has no GridInfo component.");
				}
			}
		}
	}
}
