using UnityEngine;
using System.Collections.Generic;

public static class Utils
{
	public static T GetArrayEntry<T>(T[] atArray, ref int nIndex, int nIncrement)
	{
		nIndex = (nIndex + nIncrement) % atArray.Length;

		if (nIndex < 0)
		{
			nIndex = atArray.Length + nIndex;
		}

		return atArray[nIndex];
	}

	public static T GetArrayEntry<T>(T[] atArray, int nIndex, int nIncrement)
	{
		nIndex = (nIndex + nIncrement) % atArray.Length;

		if (nIndex < 0)
		{
			nIndex = atArray.Length + nIndex;
		}

		return atArray[nIndex];
	}

	public static T[] GetSurrounding2DArrayEntries<T>(T[,] atArray, int nOriginX, int nOriginY, int nIncrementX, int nIncrementY)
	{
		List<T> lstEntries = new List<T>();

		int nXMin = Mathf.Clamp(nOriginX - nIncrementX, 0, atArray.GetLength(0) - 1);
		int nXMax = Mathf.Clamp(nOriginX + nIncrementX, 0, atArray.GetLength(0) - 1);

		int nYMin = Mathf.Clamp(nOriginY - nIncrementY, 0, atArray.GetLength(1) - 1);
		int nYMax = Mathf.Clamp(nOriginY + nIncrementY, 0, atArray.GetLength(1) - 1);

		for (int nX = nXMin; nX <= nXMax; nX++)
		{
			for (int nY = nYMin; nY <= nYMax; nY++)
			{
				lstEntries.Add(atArray[nX, nY]);
			}
		}

		return lstEntries.ToArray();
	}
}
