using UnityEngine;
using System;
using System.Collections.Generic;

public class GridInfo : MonoBehaviour
{
	public float Height = 0.3f;

	public int GridX = 0;
	public int GridY = 0;

	public Collider TopCollider;
	public Collider BottomCollider;

	public GameObject TopBuildTarget;
	public GameObject BottomBuildTarget;

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

	public enum BuildLayer
	{
		Top,
		Bottom,
	}

	class BuildLayerInfo
	{
		public Dictionary<BuildSlots, BlockInfo> m_dictOccupiers = new Dictionary<BuildSlots, BlockInfo>()
		{
			{ BuildSlots.North, null },
			{ BuildSlots.East, null },
			{ BuildSlots.South, null },
			{ BuildSlots.West, null },
			{ BuildSlots.Centre, null },
		};
	}

	Dictionary<BuildLayer, BuildLayerInfo> m_dictBuildLayers = new Dictionary<BuildLayer, BuildLayerInfo>()
	{
		{ BuildLayer.Top, new BuildLayerInfo() },
		{ BuildLayer.Bottom, new BuildLayerInfo() },
	};

	MeshRenderer m_cMeshRenderer;
	Color m_cOriginalColor;

	void Awake()
	{
		m_cMeshRenderer = GetComponent<MeshRenderer>();
		m_cOriginalColor = m_cMeshRenderer.material.color;
	}

	public void SetOccupied(BuildSlots eBuildSlot, BuildLayer eBuildLayer, BlockInfo cBlockInfo)
	{
		if (m_dictBuildLayers.ContainsKey(eBuildLayer))
		{
			BuildLayerInfo cBuildLayerInfo = m_dictBuildLayers[eBuildLayer];

			if (cBuildLayerInfo.m_dictOccupiers.ContainsKey(eBuildSlot))
			{
				cBuildLayerInfo.m_dictOccupiers[eBuildSlot] = cBlockInfo;
			}
		}
	}

	public void SetUnoccupied(BuildSlots eBuildSlot, BuildLayer eBuildLayer)
	{
		if (m_dictBuildLayers.ContainsKey(eBuildLayer))
		{
			BuildLayerInfo cBuildLayerInfo = m_dictBuildLayers[eBuildLayer];

			if (cBuildLayerInfo.m_dictOccupiers.ContainsKey(eBuildSlot))
			{
				cBuildLayerInfo.m_dictOccupiers[eBuildSlot] = null;
			}
		}
	}

	public bool CanBeOccupied(BuildSlots eBuildSlot, BuildLayer eBuildLayer, bool bCheckOpposite)
	{
		bool bCanBeOccupied = CanBeOccupiedInternal(eBuildSlot, eBuildLayer);

		// If the opposite side needs checked, then if the above check was passed.
		if (bCheckOpposite && bCanBeOccupied)
		{
			bCanBeOccupied = CanBeOccupiedInternal(eBuildSlot, GridUtilities.GetOppositeBuildLayer(eBuildLayer));
		}

		return bCanBeOccupied;
	}

	public bool CanBeOccupiedInternal(BuildSlots eBuildSlot, BuildLayer eBuildLayer)
	{
		if (m_dictBuildLayers.ContainsKey(eBuildLayer))
		{
			BuildLayerInfo cBuildLayerInfo = m_dictBuildLayers[eBuildLayer];

			if (cBuildLayerInfo.m_dictOccupiers.ContainsKey(eBuildSlot))
			{
				return cBuildLayerInfo.m_dictOccupiers[eBuildSlot] == null && Occupiable;
			}
		}

		return false;
	}

	public bool IsOccupied(BuildSlots eBuildSlot, BuildLayer eBuildLayer)
	{
		if (m_dictBuildLayers.ContainsKey(eBuildLayer))
		{
			BuildLayerInfo cBuildLayerInfo = m_dictBuildLayers[eBuildLayer];

			if (cBuildLayerInfo.m_dictOccupiers.ContainsKey(eBuildSlot))
			{
				return cBuildLayerInfo.m_dictOccupiers[eBuildSlot] != null;
			}
		}

		return true;
	}

	public BlockInfo GetOccupier(BuildSlots eBuildSlot, BuildLayer eBuildLayer)
	{
		if (m_dictBuildLayers.ContainsKey(eBuildLayer))
		{
			BuildLayerInfo cBuildLayerInfo = m_dictBuildLayers[eBuildLayer];

			if (cBuildLayerInfo.m_dictOccupiers.ContainsKey(eBuildSlot))
			{
				return cBuildLayerInfo.m_dictOccupiers[eBuildSlot];
			}
		}

		return null;
	}

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

	public void MappingHighlight()
	{
		m_cMeshRenderer.material.color = Color.green;
	}

	public void UnmappingHighlight()
	{
		m_cMeshRenderer.material.color = Color.red;
	}

	public void Dehighlight()
	{
		m_cMeshRenderer.material.color = m_cOriginalColor;
	}

	public void BlockedHighlight()
	{
		m_cMeshRenderer.material.color = Color.red;
	}

	public void MappedHighlight()
	{
		m_cMeshRenderer.material.color = Color.cyan;
	}
}
