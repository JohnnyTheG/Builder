using UnityEngine;
using System.Collections.Generic;

public class EndlessTerrain : MonoBehaviour
{
	// https://www.youtube.com/watch?v=xlSkYjiE-Ck&index=7&list=PLFt_AvWsXl0eBW2EiBtl_sxmDtSgZBxB3&spfreload=1
	// 7:18

	public const float MaxViewDist = 300;

	[SerializeField]
	Transform Viewer;

	// The width and height of a chunk.
	// In this case, this is the value of the generated terrain minus 1.
	int ChunkSize;
	// How many chunks should be visible within the view distance.
	// Reducing this improves performance.
	int ChunksVisibleInViewDist;

	// Dictionary of terrain chunks against coordinates.
	// Prevents duplication of chunks.
	Dictionary<Vector2, TerrainChunk> TerrainChunks = new Dictionary<Vector2, TerrainChunk>();

	void Start()
	{
		// Set the chunk size.
		ChunkSize = TerrainGenerator.MapChunkSize - 1;
		// We know we can see MaxViewDist units in the world, so divide that by the size of a chunk to tell you how many chunks are needed!
		ChunksVisibleInViewDist = Mathf.RoundToInt(MaxViewDist / ChunkSize);
	}

	void UpdateVisibleChunks()
	{
		int nCurrentChunkX = Mathf.RoundToInt(Viewer.position.x / ChunkSize);
		int nCurrentChunkY = Mathf.RoundToInt(Viewer.position.y / ChunkSize);

		for (int nYOffset = -ChunksVisibleInViewDist; nYOffset <= ChunksVisibleInViewDist; nYOffset++)
		{
			for (int nXOffset = -ChunksVisibleInViewDist; nXOffset <= ChunksVisibleInViewDist; nXOffset++)
			{
				Vector2 ViewedChunkCoordinates = new Vector2(nCurrentChunkX + nXOffset, nCurrentChunkY + nYOffset);

				if (TerrainChunks.ContainsKey(ViewedChunkCoordinates))
				{

				}
				else
				{
					// Chunk doesnt exist so add a new chunk to the dictionary for this coordinate.
					TerrainChunks.Add(ViewedChunkCoordinates, new TerrainChunk());
				}
			}
		}
	}

	public class TerrainChunk
	{
	}
}
