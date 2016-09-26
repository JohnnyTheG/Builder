using UnityEngine;
using System.Collections;

public class TerrainDisplay : Singleton<TerrainDisplay>
{
	[SerializeField]
	Renderer TextureRenderer;

    public void DrawNoiseMap(float[,] afNoiseMap)
	{
		int nWidth = afNoiseMap.GetLength(0);
		int nHeight = afNoiseMap.GetLength(1);

		Texture2D cTerrainTexture = new Texture2D(nWidth, nHeight);

		Color[] acColorMap = new Color[nWidth * nHeight];

		for (int nX = 0; nX < nWidth; nX++)
		{
			for (int nY = 0; nY < nHeight; nY++)
			{
				acColorMap[nY * nWidth + nX] = Color.Lerp(Color.black, Color.white, afNoiseMap[nX, nY]);
			}
		}

		cTerrainTexture.SetPixels(acColorMap);

		cTerrainTexture.Apply();

		TextureRenderer.sharedMaterial.mainTexture = cTerrainTexture;
		TextureRenderer.transform.localScale = new Vector3(nWidth, 1.0f, nHeight);
	}
}
