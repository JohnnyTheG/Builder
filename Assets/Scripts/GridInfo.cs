using UnityEngine;
using System;

public class GridInfo : MonoBehaviour
{
	public bool Occupiable = true;

	[NonSerialized]
	[HideInInspector]
	public bool Occupied = false;

	public float Height = 0.3f;

	public bool CanBeOccupied
	{
		get
		{
			return Occupiable && !Occupied;
		}
	}
}
