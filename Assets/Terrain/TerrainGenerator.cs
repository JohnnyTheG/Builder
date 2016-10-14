using UnityEngine;
using System;
using System.Threading;

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

	public const int MapChunkSize = 241;

	[SerializeField]
	// Range 0 to 6 gives 0, 1, 2, 4, 8, 10, 12 when doubled.
	[Range(0, 6)]
	int LevelOfDetail = 1;

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

	public struct TerrainData
	{
		public float[,] m_afHeightMap;
		public Color[] m_acColorMap;

		public TerrainData(float[,] afHeightMap, Color[] acColorMap)
		{
			m_afHeightMap = afHeightMap;
			m_acColorMap = acColorMap;
		}
	}

	public void RequestTerrainData(Action<TerrainData> cCallback)
	{
		ThreadStart cThreadStart = delegate
		{
			TerrainDataThread(cCallback);
		};

		new Thread(cThreadStart).Start();
	}

	void TerrainDataThread(Action<TerrainData> cCallback)
	{
		TerrainData cTerrainData = GenerateTerrainData();


	}

	TerrainData GenerateTerrainData()
	{
		float[,] afNoiseMap = Noise.GenerateNoiseMap(MapChunkSize, MapChunkSize, Seed, MapScale, Octaves, Persistence, Lacunarity, Offset);

		Color[] acColorMap = new Color[MapChunkSize * MapChunkSize];

		for (int nY = 0; nY < MapChunkSize; nY++)
		{
			for (int nX = 0; nX < MapChunkSize; nX++)
			{
				// Get the current height value for the noise map value being examined.
				float fCurrentHeight = afNoiseMap[nX, nY];

				// Iterate the regions defined.
				for (int nRegion = 0; nRegion < Regions.Length; nRegion++)
				{
					// If the 
					if (fCurrentHeight <= Regions[nRegion].Height)
					{
						acColorMap[nY * MapChunkSize + nX] = Regions[nRegion].Color;

						break;
					}
				}
			}
		}

		return new TerrainData(afNoiseMap, acColorMap);
	}

	public void DrawTerrainInEditor()
	{
		TerrainData cTerrainData = GenerateTerrainData();

		TerrainDisplay cTerrainDisplay = FindObjectOfType<TerrainDisplay>();

		switch (DrawMode)
		{
			case DrawModes.NoiseMap:

				cTerrainDisplay.DrawTexture(TextureGenerator.TextureFromHeightMap(cTerrainData.m_afHeightMap));

				break;

			case DrawModes.ColorMap:

				cTerrainDisplay.DrawTexture(TextureGenerator.TextureFromColorMap(cTerrainData.m_acColorMap, MapChunkSize, MapChunkSize));

				break;

			case DrawModes.Mesh:

				cTerrainDisplay.DrawMesh(MeshGenerator.GenerateTerrainMesh(cTerrainData.m_afHeightMap, MeshHeightMultiplier, MeshHeightCurve, LevelOfDetail), TextureGenerator.TextureFromColorMap(cTerrainData.m_acColorMap, MapChunkSize, MapChunkSize));

				break;
		}
	}

	void OnValidate()
	{
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
