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
}
