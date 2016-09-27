using UnityEngine;
using System.Collections;

public static class TextureGenerator
{
	public static Texture2D TextureFromColorMap(Color[] acColorMap, int nWidth, int nHeight)
	{
		Texture2D cTexture = new Texture2D(nWidth, nHeight);

		cTexture.filterMode = FilterMode.Point;

		cTexture.wrapMode = TextureWrapMode.Clamp;

		cTexture.SetPixels(acColorMap);

		cTexture.Apply();

		return cTexture;
	}

	public static Texture2D TextureFromHeightMap(float[,] afHeightMap)
	{
		int nWidth = afHeightMap.GetLength(0);
		int nHeight = afHeightMap.GetLength(1);

		Color[] acColorMap = new Color[nWidth * nHeight];

		for (int nY = 0; nY < nHeight; nY++)
		{
			for (int nX = 0; nX < nWidth; nX++)
			{
				acColorMap[nY * nWidth + nX] = Color.Lerp(Color.black, Color.white, afHeightMap[nX, nY]);
			}
		}

		return TextureFromColorMap(acColorMap, nWidth, nHeight);
	}
}
