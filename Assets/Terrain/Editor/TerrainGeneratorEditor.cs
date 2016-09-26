using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(TerrainGenerator))]
public class TerrainGeneratorEditor : Editor
{
	public override void OnInspectorGUI()
	{
		TerrainGenerator cTerrainGenerator = (TerrainGenerator)target;

		if (DrawDefaultInspector())
		{
			cTerrainGenerator.GenerateTerrain();
		}

		if (GUILayout.Button("Generate"))
		{
			cTerrainGenerator.GenerateTerrain();
		}
	}
}
