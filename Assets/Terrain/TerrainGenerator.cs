using UnityEngine;
using System.Collections;

public class TerrainGenerator : MonoBehaviour
{
	// From: https://www.youtube.com/playlist?list=PLFt_AvWsXl0eBW2EiBtl_sxmDtSgZBxB3

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

	public void GenerateTerrain()
	{
		float[,] afNoiseMap = Noise.GenerateNoiseMap(MapXSize, MapYSize, Seed, MapScale, Octaves, Persistence, Lacunarity, Offset);

		TerrainDisplay cTerrainDisplay = FindObjectOfType<TerrainDisplay>();

		cTerrainDisplay.DrawNoiseMap(afNoiseMap);
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
