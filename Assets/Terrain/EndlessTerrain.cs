using UnityEngine;
using System.Collections.Generic;

public class EndlessTerrain : MonoBehaviour
{
	// https://www.youtube.com/watch?v=xlSkYjiE-Ck&index=7&list=PLFt_AvWsXl0eBW2EiBtl_sxmDtSgZBxB3&spfreload=1
	// 7:18

	public const float MaxViewDist = 450;

	[SerializeField]
	Transform Viewer;

	public static Vector2 ViewerPosition;

	// The width and height of a chunk.
	// In this case, this is the value of the generated terrain minus 1.
	int ChunkSize;
	// How many chunks should be visible within the view distance.
	// Reducing this improves performance.
	int ChunksVisibleInViewDist;

	// Dictionary of terrain chunks against coordinates.
	// Prevents duplication of chunks.
	Dictionary<Vector2, TerrainChunk> TerrainChunks = new Dictionary<Vector2, TerrainChunk>();

	List<TerrainChunk> m_lstTerrainChunksVisibleLastUpdate = new List<TerrainChunk>();

	void Start()
	{
		// Set the chunk size.
		ChunkSize = TerrainGenerator.MapChunkSize - 1;
		// We know we can see MaxViewDist units in the world, so divide that by the size of a chunk to tell you how many chunks are needed!
		ChunksVisibleInViewDist = Mathf.RoundToInt(MaxViewDist / ChunkSize);
	}

	void Update()
	{
		ViewerPosition = new Vector2(Viewer.position.x, Viewer.position.z);

		UpdateVisibleChunks();
	}

	void UpdateVisibleChunks()
	{
		for (int nChunk = 0; nChunk < m_lstTerrainChunksVisibleLastUpdate.Count; nChunk++)
		{
			m_lstTerrainChunksVisibleLastUpdate[nChunk].SetVisible(false);
		}

		m_lstTerrainChunksVisibleLastUpdate.Clear();

		int nCurrentChunkX = Mathf.RoundToInt(ViewerPosition.x / ChunkSize);
		int nCurrentChunkY = Mathf.RoundToInt(ViewerPosition.y / ChunkSize);

		for (int nYOffset = -ChunksVisibleInViewDist; nYOffset <= ChunksVisibleInViewDist; nYOffset++)
		{
			for (int nXOffset = -ChunksVisibleInViewDist; nXOffset <= ChunksVisibleInViewDist; nXOffset++)
			{
				Vector2 ViewedChunkCoordinates = new Vector2(nCurrentChunkX + nXOffset, nCurrentChunkY + nYOffset);

				if (TerrainChunks.ContainsKey(ViewedChunkCoordinates))
				{
					// Chunk exists so update it.
					TerrainChunks[ViewedChunkCoordinates].UpdateTerrainChunk();

					if (TerrainChunks[ViewedChunkCoordinates].IsVisible())
					{
						m_lstTerrainChunksVisibleLastUpdate.Add(TerrainChunks[ViewedChunkCoordinates]);
					}
				}
				else
				{
					// Chunk doesnt exist so add a new chunk to the dictionary for this coordinate.
					TerrainChunks.Add(ViewedChunkCoordinates, new TerrainChunk(ViewedChunkCoordinates, ChunkSize, transform));
				}
			}
		}
	}

	public class TerrainChunk
	{
		Vector2 m_vec2Position;

		GameObject m_cMeshObject;

		Bounds m_cBounds;

		public TerrainChunk(Vector2 vec2Coord, int nSize, Transform cParent)
		{
			m_vec2Position = vec2Coord * nSize;

			m_cBounds = new Bounds(m_vec2Position, Vector2.one * nSize);

			Vector3 vecPosition = new Vector3(m_vec2Position.x, 0.0f, m_vec2Position.y);

			m_cMeshObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
			m_cMeshObject.transform.position = vecPosition;
			m_cMeshObject.transform.localScale = Vector3.one * nSize / 10.0f;
			m_cMeshObject.transform.parent = cParent;

			// Hide by default.
			SetVisible(false);
		}

		public void UpdateTerrainChunk()
		{
			float fViewerDistanceFromNearestEdge = Mathf.Sqrt(m_cBounds.SqrDistance(ViewerPosition));

			bool bVisible = fViewerDistanceFromNearestEdge <= MaxViewDist;

			SetVisible(bVisible);
		}

		public void SetVisible(bool bVisible)
		{
			m_cMeshObject.SetActive(bVisible);
		}

		public bool IsVisible()
		{
			return m_cMeshObject.activeSelf;
		}
	}
}
