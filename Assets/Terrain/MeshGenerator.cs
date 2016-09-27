﻿using UnityEngine;
using System.Collections;

public static class MeshGenerator
{
	public static MeshData GenerateTerrainMesh(float[,] afHeightMap)
	{
		int nWidth = afHeightMap.GetLength(0);
		int nHeight = afHeightMap.GetLength(1);

		// These are used to centre the map around the origin.
		float fTopLeftX = (nWidth - 1) / -2.0f;
		float fTopLeftY = (nHeight - 1) / 2.0f;

		MeshData cMeshData = new MeshData(nWidth, nHeight);
		int nVertexIndex = 0;

		for (int nY = 0; nY < nHeight; nY++)
		{
			for (int nX = 0; nX < nWidth; nX++)
			{
				// Position the vertice in the world at nX, nY with height based on the height map at that point.
				cMeshData.Vertices[nVertexIndex] = new Vector3(fTopLeftX + nX, afHeightMap[nX, nY], fTopLeftY - nY);
				cMeshData.UVs[nVertexIndex] = new Vector2(nX / (float)nWidth, nY / (float)nHeight);

				// Ignore the right and bottom vertices of the map as these have no triangles to map.
				if (nX < nWidth - 1 && nY < nHeight - 1)
				{
					cMeshData.AddTriangle(nVertexIndex, nVertexIndex + nWidth + 1, nVertexIndex + nWidth);
					cMeshData.AddTriangle(nVertexIndex + nWidth + 1, nVertexIndex, nVertexIndex + 1);
				}

				nVertexIndex++;
			}
		}

		return cMeshData;
	}
}

public class MeshData
{
	// Vertices of the mesh.
	public Vector3[] Vertices;
	// Indices for generating the triangles of the mesh.
	public int[] Indices;

	public Vector2[] UVs;

	// Used to iterate the indices array and add triangles.
	int m_nTriangleIndex;

	public MeshData(int nWidth, int nHeight)
	{
		Vertices = new Vector3[nWidth * nHeight];
		Indices = new int[(nWidth - 1) * (nHeight - 1) * 6];
		UVs = new Vector2[nWidth * nHeight];
	}

	public void AddTriangle(int nA, int nB, int nC)
	{
		Indices[m_nTriangleIndex] = nA;
		Indices[m_nTriangleIndex + 1] = nB;
		Indices[m_nTriangleIndex + 2] = nC;

		m_nTriangleIndex += 3;
	}

	public Mesh CreateMesh()
	{
		Mesh cMesh = new Mesh();

		cMesh.vertices = Vertices;
		cMesh.triangles = Indices;
		cMesh.uv = UVs;

		cMesh.RecalculateNormals();

		return cMesh;
	}
}
