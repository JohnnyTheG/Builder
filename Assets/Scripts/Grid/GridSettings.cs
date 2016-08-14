using UnityEngine;
using System.Collections;

public class GridSettings : Singleton<GridSettings>
{
	public int Length = 20;
	public int Width = 10;

	public GameObject[,] Grid;
} 
