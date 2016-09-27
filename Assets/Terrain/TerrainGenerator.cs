using UnityEngine;
using System.Collections;

public class TerrainGenerator : MonoBehaviour
{
	// From: https://www.youtube.com/playlist?list=PLFt_AvWsXl0eBW2EiBtl_sxmDtSgZBxB3

	public enum DrawModes
	{
		NoiseMap,
		ColorMap,
		Mesh,
	}

	public DrawModes DrawMode;

	[SerializeField]
	int MapXSize;
	[SerializeField]
	int MapYSize;
	[SerializeField]
	float MapScale;

	[SerializeField]
	int Octaves;
	[SerializeField]
	[Range(0.0f, 1.0f)]
	float Persistence;
	[SerializeField]
	float Lacunarity;

	[SerializeField]
	int Seed;
	[SerializeField]
	Vector2 Offset;

	[SerializeField]
	float MeshHeightMultiplier;
	[SerializeField]
	AnimationCurve MeshHeightCurve;

	// This defines the height regions in layers.
	public TerrainType[] Regions;

	[System.Serializable]
	public struct TerrainType
	{
		public string Name;
		public float Height;
		public Color Color;
	}

	public void GenerateTerrain()
	{
		float[,] afNoiseMap = Noise.GenerateNoiseMap(MapXSize, MapYSize, Seed, MapScale, Octaves, Persistence, Lacunarity, Offset);

		Color[] acColorMap = new Color[MapXSize * MapYSize];

		for (int nY = 0; nY < MapYSize; nY++)
		{
			for (int nX = 0; nX < MapXSize; nX++)
			{
				// Get the current height value for the noise map value being examined.
				float fCurrentHeight = afNoiseMap[nX, nY];

				// Iterate the regions defined.
				for (int nRegion = 0; nRegion < Regions.Length; nRegion++)
				{
					// If the 
					if (fCurrentHeight <= Regions[nRegion].Height)
					{
						acColorMap[nY * MapXSize + nX] = Regions[nRegion].Color;

						break;
					}
				}
			}
		}

		TerrainDisplay cTerrainDisplay = FindObjectOfType<TerrainDisplay>();

		switch (DrawMode)
		{
			case DrawModes.NoiseMap:

				cTerrainDisplay.DrawTexture(TextureGenerator.TextureFromHeightMap(afNoiseMap));

				break;

			case DrawModes.ColorMap:

				cTerrainDisplay.DrawTexture(TextureGenerator.TextureFromColorMap(acColorMap, MapXSize, MapYSize));

				break;

			case DrawModes.Mesh:

				cTerrainDisplay.DrawMesh(MeshGenerator.GenerateTerrainMesh(afNoiseMap, MeshHeightMultiplier, MeshHeightCurve), TextureGenerator.TextureFromColorMap(acColorMap, MapXSize, MapYSize));

				break;
		}
	}

	void OnValidate()
	{
		if (MapXSize < 1)
		{
			MapXSize = 1;
		}

		if (MapYSize < 1)
		{
			MapYSize = 1;
		}

		if (Lacunarity < 1)
		{
			Lacunarity = 1;
		}

		if (Octaves < 0)
		{
			Octaves = 0;
		}
	}
}
