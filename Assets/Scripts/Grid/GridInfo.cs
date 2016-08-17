using UnityEngine;
using System;
using System.Collections.Generic;

public class GridInfo : MonoBehaviour
{
	public float Height = 0.3f;

	public int GridX = 0;
	public int GridY = 0;

	bool Occupiable = true;

	// Order of this is important.
	public enum BuildSlots
	{
		North,
		East,
		South,
		West,
		Centre,
	}

	Dictionary<BuildSlots, BlockInfo> m_dictOccupiers = new Dictionary<BuildSlots, BlockInfo>()
	{
		{ BuildSlots.North, null },
		{ BuildSlots.East, null },
		{ BuildSlots.South, null },
		{ BuildSlots.West, null },
		{ BuildSlots.Centre, null },
	};

	public void SetOccupied(BuildSlots eBuildSlot, BlockInfo cBlockInfo)
	{
		if (m_dictOccupiers.ContainsKey(eBuildSlot))
		{
			m_dictOccupiers[eBuildSlot] = cBlockInfo;
		}
	}

	public void SetUnoccupied(BuildSlots eBuildSlot)
	{
		if (m_dictOccupiers.ContainsKey(eBuildSlot))
		{
			m_dictOccupiers[eBuildSlot] = null;
		}
	}

	public bool CanBeOccupied(BuildSlots eBuildSlot)
	{
		if (m_dictOccupiers.ContainsKey(eBuildSlot))
		{
			return m_dictOccupiers[eBuildSlot] == null && Occupiable;
		}

		return false;
	}

	public bool IsOccupied(BuildSlots eBuildSlot)
	{
		if (m_dictOccupiers.ContainsKey(eBuildSlot))
		{
			return m_dictOccupiers[eBuildSlot] != null;
		}

		return true;
	}

	public BlockInfo GetOccupier(BuildSlots eBuildSlot)
	{
		if (m_dictOccupiers.ContainsKey(eBuildSlot))
		{
			return m_dictOccupiers[eBuildSlot];
		}

		return null;
	}

	/*

	bool Occupied = false;

	public BlockInfo Occupier
	{
		get;

		private set;
	}

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
	}*/


	public bool InRoom
	{
		get;

		private set;
	}

	public void SetInRoom()
	{
		InRoom = true;
	}

	public void SetNotInRoom()
	{
		InRoom = false;
	}
}
