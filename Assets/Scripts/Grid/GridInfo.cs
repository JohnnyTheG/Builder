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

	// The block set entry spawned on this grid slot.
	// Its up to this grid info to select the correct piece from that block set.
	BlockSetEntry m_cBlockSetEntry;

	// Order of this is important.
	public enum BuildSlot
	{
		North,
		East,
		South,
		West,
		Centre,

		Undefined,
	}

	public enum BuildLayer
	{
		Top,
		Bottom,

		Undefined,
	}

	// Information about block build on a certain BuildSlot.
	class GridBuildInfo
	{
		public BlockInfo m_cBlockInfo = null;
		public bool m_bOccupied = false;
	}

	// Information about a BuildLayer and what slots are occupied on that layer.
	class BuildLayerInfo
	{
		public Dictionary<BuildSlot, GridBuildInfo> m_dictOccupiers = new Dictionary<BuildSlot, GridBuildInfo>()
		{
			{ BuildSlot.North, new GridBuildInfo() },
			{ BuildSlot.East, new GridBuildInfo() },
			{ BuildSlot.South, new GridBuildInfo() },
			{ BuildSlot.West, new GridBuildInfo() },
			{ BuildSlot.Centre, new GridBuildInfo() },
		};
	}

	// Information about this GridInfo as a whole, from both layers downwards.
	Dictionary<BuildLayer, BuildLayerInfo> m_dictBuildLayers = new Dictionary<BuildLayer, BuildLayerInfo>()
	{
		{ BuildLayer.Top, new BuildLayerInfo() },
		{ BuildLayer.Bottom, new BuildLayerInfo() },
	};

	// Dictionary of slots which make up corners.
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

	public void SetOccupied(BuildSlot eBuildSlot, BuildLayer eBuildLayer, BlockSetEntry cBlockSetEntry)
	{
		m_cBlockSetEntry = cBlockSetEntry;

        if (m_dictBuildLayers.ContainsKey(eBuildLayer))
		{
			BuildLayerInfo cBuildLayerInfo = m_dictBuildLayers[eBuildLayer];

			if (cBuildLayerInfo.m_dictOccupiers.ContainsKey(eBuildSlot))
			{
				// Set the slot to occupied, the block will generate itself later.
				cBuildLayerInfo.m_dictOccupiers[eBuildSlot].m_bOccupied = true;
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
				cBuildLayerInfo.m_dictOccupiers[eBuildSlot].m_bOccupied = false;
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
				return cBuildLayerInfo.m_dictOccupiers[eBuildSlot].m_bOccupied == false && Occupiable;
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
				return cBuildLayerInfo.m_dictOccupiers[eBuildSlot].m_bOccupied;
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
				return cBuildLayerInfo.m_dictOccupiers[eBuildSlot].m_cBlockInfo;
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

			// For each slot that can make a corner, if its occupied, then add it to the list to be turned into a corner.
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


	// NICKED FROM BUILDMODE.CS START

	BlockInfo CreateBlockGameObject(GameObject cBlockToCreate, GridInfo cGridInfo, GridInfo.BuildSlot eBuildSlot, GridInfo.BuildLayer eGridLayer, bool bIsGhost)
	{
		GameObject cBlock = Instantiate(cBlockToCreate);

		BlockInfo cBlockInfo = cBlock.GetComponent<BlockInfo>();

		cBlockInfo.Initialise(bIsGhost);

		cBlock.transform.rotation = GetBlockRotation(eBuildSlot, eGridLayer);

		if (cBlockInfo != null)
		{
			cBlockInfo.Move(cGridInfo, eBuildSlot, eGridLayer, false);
		}

		return cBlockInfo;
	}

	Dictionary<GridInfo.BuildSlot, Quaternion> m_dictBuildDirections = new Dictionary<GridInfo.BuildSlot, Quaternion>()
	{
		{GridInfo.BuildSlot.North,     Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f))},
		{GridInfo.BuildSlot.East,      Quaternion.Euler(new Vector3(0.0f, 90.0f, 0.0f))},
		{GridInfo.BuildSlot.South,     Quaternion.Euler(new Vector3(0.0f, 180.0f, 0.0f))},
		{GridInfo.BuildSlot.West,      Quaternion.Euler(new Vector3(0.0f, 270.0f, 0.0f))},
	};

	Quaternion GetBlockRotation(GridInfo.BuildSlot eBuildSlot, GridInfo.BuildLayer eBuildLayer)
	{
		Vector3 vecRotation = Vector3.zero;

		switch (eBuildSlot)
		{
			case GridInfo.BuildSlot.Centre:

				// Just default centre blocks to north for now.
				vecRotation = m_dictBuildDirections[GridInfo.BuildSlot.North].eulerAngles;

				break;

			default:

				vecRotation = m_dictBuildDirections[eBuildSlot].eulerAngles;

				break;
		}

		if (GridSettings.Instance.UpBuildLayer != eBuildLayer)
		{
			vecRotation += new Vector3(0.0f, 180.0f, 180.0f);
		}

		return Quaternion.Euler(vecRotation);
	}

	// NICKED FROM BUILDMODE.CS END


	public void Refresh()
	{
		// Get number of connections on own grid slot.

		BuildLayer eBuildLayer = BuildLayer.Top;

		// This is the dictionary which is populated with actual corners which 100% exist.
		Dictionary<BuildSlot, List<BuildSlot>> dictActualCorners = new Dictionary<BuildSlot, List<BuildSlot>>()
		{
			{BuildSlot.North, new List<BuildSlot>() },
			{BuildSlot.East, new List<BuildSlot>() },
			{BuildSlot.South, new List<BuildSlot>() },
			{BuildSlot.West, new List<BuildSlot>() },
		};

		// For each build slot that a corner is possible.
		foreach (KeyValuePair<BuildSlot, List<BuildSlot>> cPair in m_dictBuildCorners)
		{
			BuildSlot eBuildSlot = cPair.Key;

			// If that slot is occupied.
			if (IsOccupied(eBuildSlot, eBuildLayer))
			{
				// Then for each other slot which can form a corner with the current slot.
				for (int nPossibleCorner = 0; nPossibleCorner < cPair.Value.Count; nPossibleCorner++)
				{
					BuildSlot ePossibleCornerBuildSlot = cPair.Value[nPossibleCorner];

					// If any of the other slots are filled, we know there is a corner.
					if (IsOccupied(ePossibleCornerBuildSlot, eBuildLayer))
					{
						// Make sure its in the dictionary.
						if (dictActualCorners.ContainsKey(eBuildSlot))
						{
							// Add the possible corner to the slot being examined as we know there is a corner.
							dictActualCorners[eBuildSlot].Add(ePossibleCornerBuildSlot);
						}
					}
				}

				// BlockSetEntry BlockInfo to be created for occupation.
				GameObject cBlockToCreate = null;

				// If this block generates automatic corners.
				if (m_cBlockSetEntry.AutomaticCorners)
				{
					int nConnectionCount = dictActualCorners[eBuildSlot].Count;

					if (nConnectionCount == 0)
					{
						// Flat wall.

						cBlockToCreate = m_cBlockSetEntry.BlockInfo.gameObject;
                    }
					else if (nConnectionCount == 1)
					{
						// L shaped corner.

						// Get the slots of the left and right corner segments.
						GridUtilities.CornerInfo cCornerInfo = GridUtilities.GetCornerInfo(eBuildSlot, dictActualCorners[eBuildSlot][0]);

						// Create the corner piece for the slot that the user has actually selected.
						if (cCornerInfo.m_eLeftCornerBuildSlot == eBuildSlot)
						{
							cBlockToCreate = m_cBlockSetEntry.LeftCorner.BlockInfo.gameObject;
						}
						else if (cCornerInfo.m_eRightCornerBuildSlot == eBuildSlot)
						{
							cBlockToCreate = m_cBlockSetEntry.RightCorner.BlockInfo.gameObject;
						}
						else
						{
							Debug.Log("BuildMode: Automatic Corner Building Error");
						}
					}
					else if (nConnectionCount == 2)
					{
						// U Shaped corner.
					}
					else
					{
						// Something else.
					}
				}
				else
				{
					cBlockToCreate = m_cBlockSetEntry.BlockInfo.gameObject;
				}

				CreateBlockGameObject(cBlockToCreate, this, eBuildSlot, eBuildLayer, false);
			}
		}




		// Get number of connections made with other grid slots.

		/*GridInfo cNorthGridInfo = GridSettings.Instance.GetTouchingGridInfo(this, BuildSlot.North, BuildLayer.Top);

		// Get the slots which can form a corner with north.
		List<BuildSlot> lstPossibleCorners = m_dictBuildCorners[BuildSlot.North];

		GridInfo cTouchingGridInfo = GridSettings.Instance.GetTouchingGridInfo(this, BuildSlot.North, BuildLayer.Top);

		int nNorthConnection = 0;

		for (int nPossibleCorner = 0; nPossibleCorner < lstPossibleCorners.Count; nPossibleCorner++)
		{
			if (cTouchingGridInfo.IsOccupied(lstPossibleCorners[nPossibleCorner], BuildLayer.Top))
			{
				nNorthConnection++;
			}
		}*/
	}
}
