﻿using UnityEngine;
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
		public bool m_bIsOpposite = false;
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

	// This is used during Refresh call to hold information about actual corners which exist.
	struct CornerInfo
	{
		public GridInfo m_cGridInfo;
		public BuildSlot m_eBuildSlot;
		public BuildLayer m_eBuildLayer;
	}

	MeshRenderer m_cMeshRenderer;
	Color m_cOriginalColor;

	void Awake()
	{
		m_cMeshRenderer = GetComponent<MeshRenderer>();
		m_cOriginalColor = m_cMeshRenderer.material.color;
	}

	public BuildSlotInfo GetBuildSlotInfo(BuildSlot eBuildSlot, BuildLayer eBuildLayer)
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
		SetOccupiedInternal(eBuildSlot, eBuildLayer, cBlockSetEntry, false);

		if (cBlockSetEntry.HasOppositeBlock())
		{
			SetOccupiedInternal(eBuildSlot, GridUtilities.GetOppositeBuildLayer(eBuildLayer), cBlockSetEntry, true);
		}

		// Fill in the corresponding blocks on touching grid squares but only if not centre (as centre isnt shared).
		if (eBuildSlot != GridInfo.BuildSlot.Centre)
		{
			// Fill the opposite grid info as both are "occupied".
			GridInfo cTouchingGridInfo = GridSettings.Instance.GetTouchingGridInfo(this, eBuildSlot, eBuildLayer);

			GridInfo.BuildSlot eOppositeBuildSlot = GridUtilities.GetOppositeBuildSlot(eBuildSlot);

			if (cTouchingGridInfo != null)
			{
				cTouchingGridInfo.SetOccupiedInternal(eOppositeBuildSlot, eBuildLayer, cBlockSetEntry, false);
			}

			if (cBlockSetEntry.HasOppositeBlock())
			{
				cTouchingGridInfo.SetOccupiedInternal(eOppositeBuildSlot, GridUtilities.GetOppositeBuildLayer(eBuildLayer), cBlockSetEntry, true);
			}
		}
	}

	void SetOccupiedInternal(BuildSlot eBuildSlot, BuildLayer eBuildLayer, BlockSetEntry cBlockSetEntry, bool bIsOpposite)
	{
		GridSettings.Instance.RefreshGrid();

		m_cBlockSetEntry = cBlockSetEntry;

		BuildSlotInfo cBuildSlotInfo = GetBuildSlotInfo(eBuildSlot, eBuildLayer);

		if (cBuildSlotInfo != null)
		{
			// Set the slot to occupied, the block will generate itself later.
			cBuildSlotInfo.m_bOccupied = true;
			cBuildSlotInfo.m_cBlockSetEntry = cBlockSetEntry;
			// Is this an opposite.
			cBuildSlotInfo.m_bIsOpposite = bIsOpposite;
		}
	}

	public void SetUnoccupied(BuildSlot eBuildSlot, BuildLayer eBuildLayer)
	{
		GridSettings.Instance.RefreshGrid();

		BuildSlotInfo cBuildSlotInfo = GetBuildSlotInfo(eBuildSlot, eBuildLayer);

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

	public bool HasOpposite(BuildSlot eBuildSlot, BuildLayer eBuildLayer)
	{
		BuildSlotInfo cBuildSlotInfo = GetBuildSlotInfo(eBuildSlot, GridUtilities.GetOppositeBuildLayer(eBuildLayer));

		if (cBuildSlotInfo != null)
		{
			return cBuildSlotInfo.m_bIsOpposite;
		}

		return false;
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

	BlockInfo CreateBlockInfo(GameObject cBlockToCreate, BuildSlot eBuildSlot, BuildLayer eBuildLayer, bool bIsGhost)
	{
		GameObject cBlock = Instantiate(cBlockToCreate);

		BlockInfo cBlockInfo = cBlock.GetComponent<BlockInfo>();

		cBlockInfo.Initialise(this, eBuildSlot, eBuildLayer, bIsGhost);

		cBlock.transform.rotation = GetBlockRotation(eBuildSlot, eBuildLayer);

		Vector3 vecPosition = Vector3.zero;

		switch (eBuildLayer)
		{
			case GridInfo.BuildLayer.Top:

				vecPosition = TopBuildTarget.transform.position;

				break;

			case GridInfo.BuildLayer.Bottom:

				vecPosition = BottomBuildTarget.transform.position;

				break;
		}

		cBlock.transform.position = vecPosition;

		// Attach to the grid info holding it.
		cBlock.transform.parent = transform;

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
			Dictionary<BuildSlot, List<CornerInfo>> dictActualCorners = new Dictionary<BuildSlot, List<CornerInfo>>()
			{
				{BuildSlot.North, new List<CornerInfo>() },
				{BuildSlot.East, new List<CornerInfo>() },
				{BuildSlot.South, new List<CornerInfo>() },
				{BuildSlot.West, new List<CornerInfo>() },
				{BuildSlot.Centre, new List<CornerInfo>() },
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
			Dictionary<BuildSlot, List<CornerInfo>> dictActualCorners = new Dictionary<BuildSlot, List<CornerInfo>>()
			{
				{BuildSlot.North, new List<CornerInfo>() },
				{BuildSlot.East, new List<CornerInfo>() },
				{BuildSlot.South, new List<CornerInfo>() },
				{BuildSlot.West, new List<CornerInfo>() },
				{BuildSlot.Centre, new List<CornerInfo>() },
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

	void RefreshBlockInfo(BuildSlot eBuildSlot, BuildLayer eBuildLayer, BlockSetEntry cBlockSetEntry, List<BuildSlot> lstCornerPairs, ref Dictionary<BuildSlot, List<CornerInfo>> dictActualCorners, bool bIsGhost)
	{
		// Good for debugging when looking for a block which should exist.
		bool bOccupied = IsOccupied(eBuildSlot, eBuildLayer);

		// If that slot is occupied or this is a ghost highlight.
		if (bOccupied || bIsGhost)
		{
			// BlockSetEntry BlockInfo to be created for occupation.
			GameObject cBlockToCreate = null;

			BuildSlotInfo cBuildSlotInfo = GetBuildSlotInfo(eBuildSlot, eBuildLayer);

			// If the block is in the centre slot, its easy as there are no corners or anything.
			if (eBuildSlot == BuildSlot.Centre)
			{
				if (cBuildSlotInfo.m_bIsOpposite)
				{
					cBlockToCreate = cBlockSetEntry.OppositeBlockInfo.gameObject;
				}
				else
				{
					cBlockToCreate = cBlockSetEntry.BlockInfo.gameObject;
				}
			}
			else
			{
				GridInfo cTouching = GridSettings.Instance.GetTouchingGridInfo(this, eBuildSlot, eBuildLayer);

				// Then for each other slot which can form a corner with the current slot.
				for (int nPossibleCorner = 0; nPossibleCorner < lstCornerPairs.Count; nPossibleCorner++)
				{
					BuildSlot ePossibleCornerBuildSlot = lstCornerPairs[nPossibleCorner];

					CheckForCorner(this, eBuildSlot, eBuildLayer, ePossibleCornerBuildSlot, ref dictActualCorners);

					if (cTouching != null)
					{
						CheckForCorner(cTouching, eBuildSlot, eBuildLayer, ePossibleCornerBuildSlot, ref dictActualCorners);
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
						GridUtilities.CornerInfo cCornerInfo = GridUtilities.GetCornerInfo(eBuildSlot, dictActualCorners[eBuildSlot][0].m_eBuildSlot);

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
						// U or T corner.

						Debug.Log("GridInfo: Skipping build of U or T Corner.");

						return;
					}
					else if (nConnectionCount == 3)
					{
						Debug.Log("GridInfo: Skipping build of J corner.");

						return;
                    }
					else
					{
						// Something else.

						Debug.Log("GridInfo: Skipping build of I corner.");

						return;
					}
				}
				else
				{
					cBlockToCreate = cBlockSetEntry.BlockInfo.gameObject;
				}
			}

			if (cBuildSlotInfo != null)
			{
				// If there is already a block info spawned on this grid info.
				if (cBuildSlotInfo.m_cBlockInfo != null)
				{
					// Destroy it.
					Destroy(cBuildSlotInfo.m_cBlockInfo.gameObject);
				}

				// Create new block info of the correct type.
				cBuildSlotInfo.m_cBlockInfo = CreateBlockInfo(cBlockToCreate, eBuildSlot, eBuildLayer, bIsGhost);

				// Set the block set on the block itself. So that when it moves, it knows what to assign to the new GridInfo!
				cBuildSlotInfo.m_cBlockInfo.BlockSetEntryCreatedFrom = cBlockSetEntry;
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

	void CheckForCorner(GridInfo cGridInfo, BuildSlot eBuildSlot, BuildLayer eBuildLayer, BuildSlot ePossibleCornerBuildSlot, ref Dictionary<BuildSlot, List<CornerInfo>> dictActualCorners)
	{
		// If any of the other corner pair slots are filled, we know there is a corner.
		if (cGridInfo.IsOccupied(ePossibleCornerBuildSlot, eBuildLayer))
		{
			// Make sure its in the dictionary.
			if (dictActualCorners.ContainsKey(eBuildSlot))
			{
				CornerInfo cCornerInfo = new CornerInfo();

				cCornerInfo.m_cGridInfo = cGridInfo;
				cCornerInfo.m_eBuildSlot = ePossibleCornerBuildSlot;
				cCornerInfo.m_eBuildLayer = eBuildLayer;

				// Add the possible corner to the slot being examined as we know there is a corner.
				dictActualCorners[eBuildSlot].Add(cCornerInfo);
			}
		}
	}

	public void Move(GridInfo cDestinationGridInfo, BuildSlot eOriginBuildSlot, BuildLayer eOriginBuildLayer, BuildSlot eDestinationBuildSlot, BuildLayer eDestinationBuildLayer)
	{
		// Get the details for the slot being moved.
		BuildSlotInfo cBuildSlotInfo = GetBuildSlotInfo(eOriginBuildSlot, eOriginBuildLayer);

		if (cBuildSlotInfo != null)
		{
			if (cDestinationGridInfo != null)
			{
				// Set the destination to be occupied with the details from the slot being moved.
				cDestinationGridInfo.SetOccupiedInternal(eDestinationBuildSlot, eDestinationBuildLayer, cBuildSlotInfo.m_cBlockSetEntry, cBuildSlotInfo.m_bIsOpposite);

				if (HasOpposite(eOriginBuildSlot, eOriginBuildLayer))
				{
					BuildSlotInfo cOppositeBuildSlotInfo = GetBuildSlotInfo(eOriginBuildSlot, GridUtilities.GetOppositeBuildLayer(eOriginBuildLayer));

					cDestinationGridInfo.SetOccupiedInternal(eDestinationBuildSlot, GridUtilities.GetOppositeBuildLayer(eDestinationBuildLayer), cOppositeBuildSlotInfo.m_cBlockSetEntry, cOppositeBuildSlotInfo.m_bIsOpposite);

					SetUnoccupied(eOriginBuildSlot, GridUtilities.GetOppositeBuildLayer(eOriginBuildLayer));
                }
	        }
		}

		// Set this gridinfo to be unoccupied in the original slot as it has been moved.
		SetUnoccupied(eOriginBuildSlot, eOriginBuildLayer);
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
