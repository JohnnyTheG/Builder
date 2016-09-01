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
	public enum BuildSlot
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
		public Dictionary<BuildSlot, BlockInfo> m_dictOccupiers = new Dictionary<BuildSlot, BlockInfo>()
		{
			{ BuildSlot.North, null },
			{ BuildSlot.East, null },
			{ BuildSlot.South, null },
			{ BuildSlot.West, null },
			{ BuildSlot.Centre, null },
		};
	}

	Dictionary<BuildLayer, BuildLayerInfo> m_dictBuildLayers = new Dictionary<BuildLayer, BuildLayerInfo>()
	{
		{ BuildLayer.Top, new BuildLayerInfo() },
		{ BuildLayer.Bottom, new BuildLayerInfo() },
	};

	Dictionary<BuildSlot, List<BuildSlot>> m_dictBuildCorners = new Dictionary<BuildSlot, List<BuildSlot>>()
	{
		{BuildSlot.North, new List<BuildSlot>() { BuildSlot.East, BuildSlot.West } },
		{BuildSlot.East, new List<BuildSlot>() { BuildSlot.North, BuildSlot.South } },
		{BuildSlot.South, new List<BuildSlot>() { BuildSlot.East, BuildSlot.West } },
		{BuildSlot.West, new List<BuildSlot>() { BuildSlot.North, BuildSlot.South } }
	};

	MeshRenderer m_cMeshRenderer;
	Color m_cOriginalColor;

	void Awake()
	{
		m_cMeshRenderer = GetComponent<MeshRenderer>();
		m_cOriginalColor = m_cMeshRenderer.material.color;
	}

	public void SetOccupied(BuildSlot eBuildSlot, BuildLayer eBuildLayer, BlockInfo cBlockInfo)
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

	public void SetUnoccupied(BuildSlot eBuildSlot, BuildLayer eBuildLayer)
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

	public bool CanBeOccupied(BuildSlot eBuildSlot, BuildLayer eBuildLayer, bool bCheckOpposite)
	{
		bool bCanBeOccupied = CanBeOccupiedInternal(eBuildSlot, eBuildLayer);

		// If the opposite side needs checked, then if the above check was passed.
		if (bCheckOpposite && bCanBeOccupied)
		{
			bCanBeOccupied = CanBeOccupiedInternal(eBuildSlot, GridUtilities.GetOppositeBuildLayer(eBuildLayer));
		}

		return bCanBeOccupied;
	}

	public bool CanBeOccupiedInternal(BuildSlot eBuildSlot, BuildLayer eBuildLayer)
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

	public bool IsOccupied(BuildSlot eBuildSlot, BuildLayer eBuildLayer)
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

	public BlockInfo GetOccupier(BuildSlot eBuildSlot, BuildLayer eBuildLayer)
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

	public List<BuildSlot> GetCornerBuildSlots(BuildSlot eBuildSlot, BuildLayer eBuildLayer)
	{
		List<BuildSlot> lstCornerBuildSlots = new List<BuildSlot>();

		if (m_dictBuildLayers.ContainsKey(eBuildLayer) && m_dictBuildCorners.ContainsKey(eBuildSlot))
		{
			BuildLayerInfo cBuildLayerInfo = m_dictBuildLayers[eBuildLayer];
			List<BuildSlot> lstBuildCorners = m_dictBuildCorners[eBuildSlot];

			for (int nBuildSlot = 0; nBuildSlot < lstBuildCorners.Count; nBuildSlot++)
			{
				if (IsOccupied(lstBuildCorners[nBuildSlot], eBuildLayer))
				{
					lstCornerBuildSlots.Add(lstBuildCorners[nBuildSlot]);
				}
			}
		}

		return lstCornerBuildSlots;
	}
}
