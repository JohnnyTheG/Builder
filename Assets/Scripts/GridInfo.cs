using UnityEngine;
using System;

public class GridInfo : MonoBehaviour
{
	bool Occupiable = true;

	bool Occupied = false;

	public BlockInfo Occupier
	{
		get;

		private set;
	}

	public float Height = 0.3f;

	public bool CanBeOccupied
	{
		get
		{
			return Occupiable && !Occupied;
		}
	}

	public bool IsOccupied()
	{
		return Occupied;
	}

	public void SetOccupied(BlockInfo cOccupyingBlockInfo)
	{
		Occupied = true;
		Occupier = cOccupyingBlockInfo;
	}

	public void SetUnoccupied()
	{
		Occupied = false;
		Occupier = null;
	}
}
