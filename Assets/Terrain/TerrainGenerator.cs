using UnityEngine;
using System;
using System.Threading;
using System.Collections.Generic;

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

	Queue<TerrainThreadInfo<TerrainData>> m_qTerrainDataThreadInfo = new Queue<TerrainThreadInfo<TerrainData>>();
	Queue<TerrainThreadInfo<MeshData>> m_qMeshDataThreadInfo = new Queue<TerrainThreadInfo<MeshData>>();

	[System.Serializable]
	public struct TerrainType
	{
		public string Name;
		public float Height;
		public Color Color;
	}

	public struct TerrainData
	{
		public readonly float[,] m_afHeightMap;
		public readonly Color[] m_acColorMap;

		public TerrainData(float[,] afHeightMap, Color[] acColorMap)
		{
			m_afHeightMap = afHeightMap;
			m_acColorMap = acColorMap;
		}
	}

	public struct TerrainThreadInfo<T>
	{
		public readonly Action<T> m_cCallback;
		public readonly T m_cParameter;

		public TerrainThreadInfo(Action<T> cCallback, T cParameter)
		{
			m_cCallback = cCallback;
			m_cParameter = cParameter;
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
		// This generates terrain data.
		TerrainData cTerrainData = GenerateTerrainData();

		// This locks the queue so that it is not edited on multiple threads at the same time.
		// Lock is almost like a coroutine which says to all other threads, while excuting code within
		// the braces, this code cannot run on other threads.
		lock(m_qTerrainDataThreadInfo)
		{
			// Once terrain data has been completed, we need to call the callback but this cannot
			// be called from the thread as Unity does not allow certain things to be modified outside
			// of the main thread. So we enqueue a thread info struct to call the callback from the update loop.
			m_qTerrainDataThreadInfo.Enqueue(new TerrainThreadInfo<TerrainData>(cCallback, cTerrainData));
		}
	}

	public void RequestMeshData(TerrainData cTerrainData, Action<MeshData> cCallback)
	{
		ThreadStart cThreadStart = delegate
		{
			MeshDataThread(cTerrainData, cCallback);
		};

		new Thread(cThreadStart).Start();
	}

	void MeshDataThread(TerrainData cTerrainData, Action<MeshData> cCallback)
	{
		MeshData cMeshData = MeshGenerator.GenerateTerrainMesh(cTerrainData.m_afHeightMap, MeshHeightMultiplier, MeshHeightCurve, LevelOfDetail);

		lock(m_qMeshDataThreadInfo)
		{
			m_qMeshDataThreadInfo.Enqueue(new TerrainThreadInfo<MeshData>(cCallback, cMeshData));
		}
	}

	void Update()
	{
		// If anything is in this queue, it means that the thread has completed its work, so do the callback.
		if (m_qTerrainDataThreadInfo.Count > 0)
		{
			for (int nThreadInfo = 0; nThreadInfo < m_qTerrainDataThreadInfo.Count; nThreadInfo++)
			{
				TerrainThreadInfo<TerrainData> cThreadInfo = m_qTerrainDataThreadInfo.Dequeue();

				// Call the callback for the thread info with the generated data from the thread.
				cThreadInfo.m_cCallback(cThreadInfo.m_cParameter);
			}
		}

		if (m_qMeshDataThreadInfo.Count > 0)
		{
			for (int nThreadInfo = 0; nThreadInfo < m_qMeshDataThreadInfo.Count; nThreadInfo++)
			{
				TerrainThreadInfo<MeshData> cThreadInfo = m_qMeshDataThreadInfo.Dequeue();

				cThreadInfo.m_cCallback(cThreadInfo.m_cParameter);
			}
		}
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
