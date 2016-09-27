using UnityEngine;

public static class Noise
{
	public static float[,] GenerateNoiseMap(int nMapXSize, int nMapYSize, int nSeed, float fScale, int nOctaves, float fPersistence, float fLacunarity, Vector2 vec2Offset)
	{
		float[,] afNoiseMap = new float[nMapXSize, nMapYSize];

		// Random number generator.
		System.Random cRNG = new System.Random(nSeed);

		// Create some random offsets for getting perlin noise so that the map differs based on the seed.
		Vector2[] avec2OctaveOffsets = new Vector2[nOctaves];

		for (int nOctave = 0; nOctave < nOctaves; nOctave++)
		{
			float fOffsetX = cRNG.Next(-100000, 100000) + vec2Offset.x;
			float fOffsetY = cRNG.Next(-100000, 100000) + vec2Offset.y;

			avec2OctaveOffsets[nOctave] = new Vector2(fOffsetX, fOffsetY);
		}

		if (fScale <= 0.0f)
		{
			fScale = 0.0001f;
		}

		// Min and max noise height values found.
		float fMinNoiseHeight = float.MaxValue;
		float fMaxNoiseHeight = float.MinValue;

		float fHalfWidth = nMapXSize / 2.0f;
		float fHalfHeight = nMapYSize / 2.0f;

		for (int nY = 0; nY < afNoiseMap.GetLength(1); nY++)
		{
			for (int nX = 0; nX < afNoiseMap.GetLength(0); nX++)
			{
				float fAmplitude = 1;
				float fFrequency = 1;

				float fNoiseHeight = 0;

				for (int nOctave = 0; nOctave < nOctaves; nOctave++)
				{
					float fSampleX = (nX - fHalfWidth) / fScale * fFrequency + avec2OctaveOffsets[nOctave].x;
					float fSampleY = (nY - fHalfHeight) / fScale * fFrequency + avec2OctaveOffsets[nOctave].y;

					float fPerlin = Mathf.PerlinNoise(fSampleX, fSampleY) * 2 - 1;

					fNoiseHeight += fPerlin * fAmplitude;

					fAmplitude *= fPersistence;
					fFrequency *= fLacunarity;
				}

				if (fNoiseHeight > fMaxNoiseHeight)
				{
					fMaxNoiseHeight = fNoiseHeight;
				}
				else if (fNoiseHeight < fMinNoiseHeight)
				{
					fMinNoiseHeight = fNoiseHeight;
				}

				afNoiseMap[nX, nY] = fNoiseHeight;
			}
		}

		for (int nY = 0; nY < afNoiseMap.GetLength(1); nY++)
		{
			for (int nX = 0; nX < afNoiseMap.GetLength(0); nX++)
			{
				// Normalise the noise map to get it into the range of 0 to 1.
				afNoiseMap[nX, nY] = Mathf.InverseLerp(fMinNoiseHeight, fMaxNoiseHeight, afNoiseMap[nX, nY]);
			}
		}

		return afNoiseMap;
	}
}
