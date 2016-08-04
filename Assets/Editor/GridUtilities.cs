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

		cGridSettings.Grid = new GameObject[cGridSettings.Length, cGridSettings.Width];

		for (int nWidth = 0; nWidth < cGridSettings.Width; nWidth++)
		{
			for (int nLength = 0; nLength < cGridSettings.Length; nLength++)
			{
				GameObject cGridSquare = (GameObject)Instantiate(m_cGridPrefab, new Vector3(nLength, 0.0f, nWidth), Quaternion.identity);

				cGridSquare.transform.parent = cGrid.transform;

				cGridSettings.Grid[nLength, nWidth] = cGridSquare;
			}
		}
	}
}
