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

	// Information about a build slot.
	public class BuildSlotInfo
	{
		public BlockInfo m_cBlockInfo = null;
		public bool m_bOccupied = false;
		public BlockSetEntry m_cBlockSetEntry = null;
	}

	// Information about a BuildLayer and what slots are occupied on that layer.
	class BuildLayerInfo
	{
		public Dictionary<BuildSlot, BuildSlotInfo> m_dictBuildSlotOccupiers = new Dictionary<BuildSlot, BuildSlotInfo>()
		{
			{ BuildSlot.North, new BuildSlotInfo() },
			{ BuildSlot.East, new BuildSlotInfo() },
			{ BuildSlot.South, new BuildSlotInfo() },
			{ BuildSlot.West, new BuildSlotInfo() },
			{ BuildSlot.Centre, new BuildSlotInfo() },
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
		{BuildSlot.West, new List<BuildSlot>() { BuildSlot.North, BuildSlot.South } },
		{BuildSlot.Centre, new List<BuildSlot>() { } },
	};

	MeshRenderer m_cMeshRenderer;
	Color m_cOriginalColor;

	void Awake()
	{
		m_cMeshRenderer = GetComponent<MeshRenderer>();
		m_cOriginalColor = m_cMeshRenderer.material.color;
	}

	BuildSlotInfo GetBuildSlotInfo(BuildSlot eBuildSlot, BuildLayer eBuildLayer)
	{
		if (m_dictBuildLayers.ContainsKey(eBuildLayer))
		{
			BuildLayerInfo cBuildLayerInfo = m_dictBuildLayers[eBuildLayer];

			if (cBuildLayerInfo.m_dictBuildSlotOccupiers.ContainsKey(eBuildSlot))
			{
				BuildSlotInfo cBuildSlotInfo = cBuildLayerInfo.m_dictBuildSlotOccupiers[eBuildSlot];

				return cBuildSlotInfo;
			}
		}

		return null;
	}

	public void SetOccupied(BuildSlot eBuildSlot, BuildLayer eBuildLayer, BlockSetEntry cBlockSetEntry)
	{
		GridSettings.Instance.RefreshGrid();

		m_cBlockSetEntry = cBlockSetEntry;

		BuildSlotInfo cBuildSlotInfo = GetBuildSlotInfo(eBuildSlot, eBuildLayer);

		if (cBuildSlotInfo != null)
		{
			// Set the slot to occupied, the block will generate itself later.
			cBuildSlotInfo.m_bOccupied = true;
			cBuildSlotInfo.m_cBlockSetEntry = cBlockSetEntry;
		}

		if (cBlockSetEntry.HasOppositeBlock())
		{
			BuildSlotInfo cOppositeBuildSlotInfo = GetBuildSlotInfo(eBuildSlot, GridUtilities.GetOppositeBuildLayer(eBuildLayer));

			if (cOppositeBuildSlotInfo != null)
			{
				cBuildSlotInfo.m_bOccupied = true;
				cBuildSlotInfo.m_cBlockSetEntry = cBlockSetEntry;
			}
		}
	}

	public void SetUnoccupied(BuildSlot eBuildSlot, BuildLayer eBuildLayer)
	{
		GridSettings.Instance.RefreshGrid();

		BuildSlotInfo cBuildSlotInfo = GetBuildSlotInfo(eBuildSlot, eBuildLayer);

		if (cBuildSlotInfo.m_cBlockSetEntry.HasOppositeBlock())
		{
			BuildSlotInfo cOppositeBuildSlotInfo = GetBuildSlotInfo(eBuildSlot, GridUtilities.GetOppositeBuildLayer(eBuildLayer));

			if (cOppositeBuildSlotInfo != null)
			{
				cOppositeBuildSlotInfo.m_bOccupied = false;
				cOppositeBuildSlotInfo.m_cBlockSetEntry = null;
			}
		}

		if (cBuildSlotInfo != null)
		{
			cBuildSlotInfo.m_bOccupied = false;
			cBuildSlotInfo.m_cBlockSetEntry = null;
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
		BuildSlotInfo cBuildSlotInfo = GetBuildSlotInfo(eBuildSlot, eBuildLayer);

		if (cBuildSlotInfo != null)
		{
			return cBuildSlotInfo.m_bOccupied == false && Occupiable;
		}

		return false;
	}

	public bool IsOccupied(BuildSlot eBuildSlot, BuildLayer eBuildLayer)
	{
		BuildSlotInfo cBuildSlotInfo = GetBuildSlotInfo(eBuildSlot, eBuildLayer);

		if (cBuildSlotInfo != null)
		{
			return cBuildSlotInfo.m_bOccupied;
		}

		return true;
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

	BlockInfo CreateBlockInfo(GameObject cBlockToCreate, GridInfo cGridInfo, GridInfo.BuildSlot eBuildSlot, GridInfo.BuildLayer eGridLayer, bool bIsGhost)
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

	Dictionary<GridInfo.BuildSlot, Quaternion> m_dictBuildRotations = new Dictionary<GridInfo.BuildSlot, Quaternion>()
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
				vecRotation = m_dictBuildRotations[GridInfo.BuildSlot.North].eulerAngles;

				break;

			default:

				vecRotation = m_dictBuildRotations[eBuildSlot].eulerAngles;

				break;
		}

		if (GridSettings.Instance.UpBuildLayer != eBuildLayer)
		{
			vecRotation += new Vector3(0.0f, 180.0f, 180.0f);
		}

		return Quaternion.Euler(vecRotation);
	}

	// NICKED FROM BUILDMODE.CS END

	public void RefreshHighlight()
	{
		if (IsOccupied(m_eHighlightBuildSlot, m_eHighlightBuildLayer))
		{
			// Cant highlight existing blocks!

			return;
		}

		// Ensure the old slot isnt occupied and check if it the highlight direction has changed.
		if (!IsOccupied(m_eHighlightPreviousBuildSlot, m_eHighlightPreviousBuildLayer) && (m_eHighlightPreviousBuildLayer != m_eHighlightBuildLayer || m_eHighlightPreviousBuildSlot != m_eHighlightBuildSlot))
		{
			// Destroy any old highlight.
			BuildSlotInfo cBuildSlotInfo = GetBuildSlotInfo(m_eHighlightPreviousBuildSlot, m_eHighlightPreviousBuildLayer);

			if (cBuildSlotInfo != null)
			{
				if (cBuildSlotInfo.m_cBlockInfo != null)
				{
					// Get rid of highlight.
					Destroy(cBuildSlotInfo.m_cBlockInfo.gameObject);
				}

				// Get rid of the reference to the ghost currently in the slot.
				cBuildSlotInfo.m_cBlockInfo = null;
			}
		}

		// Create highlight.
		if (m_bHighlighted)
		{
			Dictionary<BuildSlot, List<BuildSlot>> dictActualCorners = new Dictionary<BuildSlot, List<BuildSlot>>()
			{
				{BuildSlot.North, new List<BuildSlot>() },
				{BuildSlot.East, new List<BuildSlot>() },
				{BuildSlot.South, new List<BuildSlot>() },
				{BuildSlot.West, new List<BuildSlot>() },
			};

			List<BuildSlot> lstCornerPairs = null;

			if (m_dictBuildCorners.ContainsKey(m_eHighlightBuildSlot))
			{
				lstCornerPairs = m_dictBuildCorners[m_eHighlightBuildSlot];
			}

			if (lstCornerPairs != null)
			{
				RefreshBlockInfo(m_eHighlightBuildSlot, m_eHighlightBuildLayer, m_cHighlightBlockSetEntry, lstCornerPairs, ref dictActualCorners, true);
			}
		}
		else
		{
			BuildSlotInfo cBuildSlotInfo = GetBuildSlotInfo(m_eHighlightBuildSlot, m_eHighlightBuildLayer);

			if (cBuildSlotInfo != null)
			{
				if (cBuildSlotInfo.m_cBlockInfo != null)
				{
					// Get rid of highlight.
					Destroy(cBuildSlotInfo.m_cBlockInfo.gameObject);
				}

				// Get rid of the reference to the ghost currently in the slot.
				cBuildSlotInfo.m_cBlockInfo = null;
			}

			m_eHighlightBuildSlot = BuildSlot.Undefined;
			m_eHighlightBuildLayer = BuildLayer.Undefined;
		}
	}

	public void Refresh()
	{
		// Get number of connections on own grid slot.

		foreach (KeyValuePair<BuildLayer, BuildLayerInfo> cPairLayer in m_dictBuildLayers)
		{
			BuildLayer eBuildLayer = cPairLayer.Key;

			// This is the dictionary which is populated with actual corners which 100% exist.
			Dictionary<BuildSlot, List<BuildSlot>> dictActualCorners = new Dictionary<BuildSlot, List<BuildSlot>>()
			{
				{BuildSlot.North, new List<BuildSlot>() },
				{BuildSlot.East, new List<BuildSlot>() },
				{BuildSlot.South, new List<BuildSlot>() },
				{BuildSlot.West, new List<BuildSlot>() },
				{BuildSlot.Centre, new List<BuildSlot>() },
			};

			// For each build slot that a corner is possible.
			foreach (KeyValuePair<BuildSlot, List<BuildSlot>> cPair in m_dictBuildCorners)
			{
				BuildSlot eBuildSlot = cPair.Key;
				List<BuildSlot> lstCornerPairs = cPair.Value;

				RefreshBlockInfo(eBuildSlot, eBuildLayer, m_cBlockSetEntry, lstCornerPairs, ref dictActualCorners, false);
			}
		}
	}

	void RefreshBlockInfo(BuildSlot eBuildSlot, BuildLayer eBuildLayer, BlockSetEntry cBlockSetEntry, List<BuildSlot> lstCornerPairs, ref Dictionary<BuildSlot, List<BuildSlot>> dictActualCorners, bool bIsGhost)
	{
		// Good for debugging when looking for a block which should exist.
		bool bOccupied = IsOccupied(eBuildSlot, eBuildLayer);

		// If that slot is occupied or this is a ghost highlight.
		if (bOccupied || bIsGhost)
		{
			// BlockSetEntry BlockInfo to be created for occupation.
			GameObject cBlockToCreate = null;

			if (eBuildSlot == BuildSlot.Centre)
			{
				cBlockToCreate = cBlockSetEntry.BlockInfo.gameObject;
			}
			else
			{
				// Then for each other slot which can form a corner with the current slot.
				for (int nPossibleCorner = 0; nPossibleCorner < lstCornerPairs.Count; nPossibleCorner++)
				{
					BuildSlot ePossibleCornerBuildSlot = lstCornerPairs[nPossibleCorner];

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

				// If this block generates automatic corners.
				if (cBlockSetEntry.AutomaticCorners)
				{
					int nConnectionCount = dictActualCorners[eBuildSlot].Count;

					if (nConnectionCount == 0)
					{
						// Flat wall.

						cBlockToCreate = cBlockSetEntry.BlockInfo.gameObject;
					}
					else if (nConnectionCount == 1)
					{
						// L shaped corner.

						// Get the slots of the left and right corner segments.
						GridUtilities.CornerInfo cCornerInfo = GridUtilities.GetCornerInfo(eBuildSlot, dictActualCorners[eBuildSlot][0]);

						// Create the corner piece for the slot that the user has actually selected.
						if (cCornerInfo.m_eLeftCornerBuildSlot == eBuildSlot)
						{
							cBlockToCreate = cBlockSetEntry.LeftCorner.BlockInfo.gameObject;
						}
						else if (cCornerInfo.m_eRightCornerBuildSlot == eBuildSlot)
						{
							cBlockToCreate = cBlockSetEntry.RightCorner.BlockInfo.gameObject;
						}
						else
						{
							Debug.Log("BuildMode: Automatic Corner Building Error");
						}
					}
					else if (nConnectionCount == 2)
					{
						// U Shaped corner.

						Debug.Log("GridInfo: Skipping build of U Corner.");

						return;
					}
					else
					{
						// Something else.

						return;
					}
				}
				else
				{
					cBlockToCreate = cBlockSetEntry.BlockInfo.gameObject;
				}
			}

			BuildSlotInfo cBuildSlotInfo = GetBuildSlotInfo(eBuildSlot, eBuildLayer);

			if (cBuildSlotInfo != null)
			{
				// If there is already a block info spawned on this grid info.
				if (cBuildSlotInfo.m_cBlockInfo != null)
				{
					// Destroy it.
					Destroy(cBuildSlotInfo.m_cBlockInfo.gameObject);
				}

				// Create new block info of the correct type.
				cBuildSlotInfo.m_cBlockInfo = CreateBlockInfo(cBlockToCreate, this, eBuildSlot, eBuildLayer, bIsGhost);
			}
		}
		else
		{
			// If this section is hit, the slot is unoccupied, so should have no block on it.
			BuildSlotInfo cBuildSlotInfo = GetBuildSlotInfo(eBuildSlot, eBuildLayer);

			if (cBuildSlotInfo != null)
			{
				if (cBuildSlotInfo.m_cBlockInfo != null)
				{
					// Destroy any existing object on unoccupied slot.
					Destroy(cBuildSlotInfo.m_cBlockInfo.gameObject);
				}

				cBuildSlotInfo.m_cBlockSetEntry = null;
			}
		}
	}

	bool m_bHighlighted = false;
	BuildSlot m_eHighlightBuildSlot = BuildSlot.Undefined;
	BuildLayer m_eHighlightBuildLayer = BuildLayer.Undefined;

	BuildSlot m_eHighlightPreviousBuildSlot = BuildSlot.Undefined;
	BuildLayer m_eHighlightPreviousBuildLayer = BuildLayer.Undefined;

	BlockSetEntry m_cHighlightBlockSetEntry = null;
	BlockInfo m_cHighlightBlockInfo = null;

	public void SetHighlighted(BuildSlot eBuildSlot, BuildLayer eBuildLayer, BlockSetEntry cHighlightBlockSetEntry)
	{
		m_bHighlighted = true;

		// Store these so when we know it changes.
		m_eHighlightPreviousBuildSlot = m_eHighlightBuildSlot;
		m_eHighlightPreviousBuildLayer = m_eHighlightBuildLayer;

		m_eHighlightBuildSlot = eBuildSlot;
		m_eHighlightBuildLayer = eBuildLayer;
		m_cHighlightBlockSetEntry = cHighlightBlockSetEntry;
	}

	public void SetUnhighlighted()
	{
		m_bHighlighted = false;
	}
}
