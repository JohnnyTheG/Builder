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
		public bool m_bIsGhost = false;
	}

	// Information about a BuildLayer and what slots are occupied on that layer.
	public class BuildLayerInfo
	{
		public Dictionary<BuildSlot, BuildSlotInfo> m_dictBuildSlotOccupiers = new Dictionary<BuildSlot, BuildSlotInfo>()
		{
			{ BuildSlot.North, null },
			{ BuildSlot.East, null },
			{ BuildSlot.South, null },
			{ BuildSlot.West, null },
			{ BuildSlot.Centre, null },
		};
	}

	// Information about this GridInfo as a whole, from both layers downwards.
	public Dictionary<BuildLayer, BuildLayerInfo> m_dictBuildLayers = new Dictionary<BuildLayer, BuildLayerInfo>()
	{
		{ BuildLayer.Top, new BuildLayerInfo() },
		{ BuildLayer.Bottom, new BuildLayerInfo() },
	};

	// Dictionary of slots which make up corners.
	static readonly Dictionary<BuildSlot, List<BuildSlot>> m_dictCornerPairs = new Dictionary<BuildSlot, List<BuildSlot>>()
	{
		{BuildSlot.North, new List<BuildSlot>() { BuildSlot.East, BuildSlot.West } },
		{BuildSlot.East, new List<BuildSlot>() { BuildSlot.North, BuildSlot.South } },
		{BuildSlot.South, new List<BuildSlot>() { BuildSlot.East, BuildSlot.West } },
		{BuildSlot.West, new List<BuildSlot>() { BuildSlot.North, BuildSlot.South } },
		{BuildSlot.Centre, new List<BuildSlot>() { } },
	};

	// Dictionary containing information about corners for each build layer.
	Dictionary<BuildLayer, BuildLayerBuiltCornerInfo> m_dictBuildLayerBuiltCorners = new Dictionary<BuildLayer, BuildLayerBuiltCornerInfo>()
	{
		{BuildLayer.Top, new BuildLayerBuiltCornerInfo() },
		{BuildLayer.Bottom, new BuildLayerBuiltCornerInfo() }
	};

	// This is used during Refresh call to hold information about actual corners which exist.
	struct CornerInfo
	{
		public GridInfo m_cGridInfo;
		public BuildSlot m_eBuildSlot;
		public BuildLayer m_eBuildLayer;
	}

	class BuildLayerBuiltCornerInfo
	{
		// This is the dictionary which is populated with actual corners which 100% exist.
		public Dictionary<BuildSlot, List<CornerInfo>> m_dictBuiltCorners = new Dictionary<BuildSlot, List<CornerInfo>>()
		{
			{BuildSlot.North, new List<CornerInfo>() },
			{BuildSlot.East, new List<CornerInfo>() },
			{BuildSlot.South, new List<CornerInfo>() },
			{BuildSlot.West, new List<CornerInfo>() },
			{BuildSlot.Centre, new List<CornerInfo>() },
		};
	}

	void InitialiseCornerDictionary(ref Dictionary<BuildLayer, BuildLayerBuiltCornerInfo> dictCorners)
	{
		dictCorners = new Dictionary<BuildLayer, BuildLayerBuiltCornerInfo>()
		{
			{BuildLayer.Top, new BuildLayerBuiltCornerInfo() },
			{BuildLayer.Bottom, new BuildLayerBuiltCornerInfo() }
		};
	}

	class CornerConnectionInfo
	{
		public bool OneAExists = false;
		public bool OneBExists = false;
		public bool TwoAExists = false;
		public bool TwoBExists = false;

		// This should only contain 2 entries.
		// The first is A, the second is B.
		// e.g. If this connection info is stored against North key, then this will contain East and West.
		public List<BuildSlot> m_lstAB;

		public int GetConnectionCount()
		{
			int nConnection = 0;
			if (OneAExists)
			{
				nConnection++;
			}

			if (OneBExists)
			{
				nConnection++;
			}

			if (TwoAExists)
			{
				nConnection++;
			}

			if (TwoBExists)
			{
				nConnection++;
			}

			return nConnection;
		}

		public bool IsTCorner()
		{
			return (OneAExists && TwoAExists) || (OneBExists && TwoBExists);		   
		}

		public bool ShouldFlipTCorner()
		{
			return (OneAExists && TwoAExists);
		}

		public bool IsZCorner()
		{
			return (OneAExists && TwoBExists) || (OneBExists && TwoAExists);
		}
	}

	class Corners
	{
		public Dictionary<BuildSlot, CornerConnectionInfo> dictCorners = new Dictionary<BuildSlot, CornerConnectionInfo>()
		{
			{BuildSlot.North, new CornerConnectionInfo() },
			{BuildSlot.East, new CornerConnectionInfo() },
			{BuildSlot.South, new CornerConnectionInfo() },
			{BuildSlot.West, new CornerConnectionInfo() },
			{BuildSlot.Centre, new CornerConnectionInfo() },
		};
	}

	Dictionary<BuildLayer, Corners> m_dictCorners = new Dictionary<BuildLayer, Corners>()
	{
		{BuildLayer.Top, new Corners() },
		{BuildLayer.Bottom, new Corners() },
	};

	MeshRenderer m_cMeshRenderer;
	Color m_cOriginalColor;

	void Awake()
	{
		m_cMeshRenderer = GetComponent<MeshRenderer>();
		m_cOriginalColor = m_cMeshRenderer.material.color;
	}

	public void SetBuildSlotInfo(BuildSlotInfo cBuildSlotInfo, BuildSlot eBuildSlot, BuildLayer eBuildLayer)
	{
		if (m_dictBuildLayers.ContainsKey(eBuildLayer))
		{
			BuildLayerInfo cBuildLayerInfo = m_dictBuildLayers[eBuildLayer];

			if (cBuildLayerInfo.m_dictBuildSlotOccupiers.ContainsKey(eBuildSlot))
			{
				cBuildLayerInfo.m_dictBuildSlotOccupiers[eBuildSlot] = cBuildSlotInfo;
			}
		}
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

	public void SetOccupied(BuildSlot eBuildSlot, BuildLayer eBuildLayer, BlockSetEntry cBlockSetEntry, bool bIsGhost)
	{
		SetOccupiedInternal(eBuildSlot, eBuildLayer, cBlockSetEntry, false, bIsGhost);

		if (cBlockSetEntry.HasOppositeBlock())
		{
			SetOccupiedInternal(eBuildSlot, GridUtilities.GetOppositeBuildLayer(eBuildLayer), cBlockSetEntry, true, bIsGhost);
		}
	}

	void SetOccupiedInternal(BuildSlot eBuildSlot, BuildLayer eBuildLayer, BlockSetEntry cBlockSetEntry, bool bIsOpposite, bool bIsGhost)
	{
		GridSettings.Instance.RefreshGrid();

		BuildSlotInfo cBuildSlotInfo = GetBuildSlotInfo(eBuildSlot, eBuildLayer);

		if (cBuildSlotInfo != null)
		{
			// Set the slot to occupied, the block will generate itself later.
			cBuildSlotInfo.m_bOccupied = true;
			cBuildSlotInfo.m_cBlockSetEntry = cBlockSetEntry;
			// Is this an opposite.
			cBuildSlotInfo.m_bIsOpposite = bIsOpposite;
			cBuildSlotInfo.m_bIsGhost = bIsGhost;
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
			cBuildSlotInfo.m_bIsGhost = false;
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
			return (!cBuildSlotInfo.m_bOccupied || cBuildSlotInfo.m_bIsGhost) && Occupiable;
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

		if (m_dictBuildLayers.ContainsKey(eBuildLayer) && m_dictCornerPairs.ContainsKey(eBuildSlot))
		{
			BuildLayerInfo cBuildLayerInfo = m_dictBuildLayers[eBuildLayer];
			List<BuildSlot> lstBuildCorners = m_dictCornerPairs[eBuildSlot];

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

	static readonly Dictionary<BuildSlot, Quaternion> m_dictBuildRotations = new Dictionary<GridInfo.BuildSlot, Quaternion>()
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

	public void RefreshStep1()
	{
		InitialiseCornerDictionary(ref m_dictBuildLayerBuiltCorners);

		// For each build layer.
		foreach (KeyValuePair<BuildLayer, BuildLayerInfo> cPairLayer in m_dictBuildLayers)
		{
			BuildLayer eBuildLayer = cPairLayer.Key;

			BuildLayerInfo cBuildLayerInfo = cPairLayer.Value;

			// For each slot in the dictionary of occupiers on the layer.
			foreach (KeyValuePair<BuildSlot, BuildSlotInfo> cSlotPair in cBuildLayerInfo.m_dictBuildSlotOccupiers)
			{
				BuildSlot eBuildSlot = cSlotPair.Key;

				// If the build slot is occupied.
				if (IsOccupied(eBuildSlot, eBuildLayer))
				{
					// Get the other slots which make a corner with this build slot.
					if (m_dictCornerPairs.ContainsKey(eBuildSlot))
					{
						List<BuildSlot> lstCornerPairs = m_dictCornerPairs[eBuildSlot];

						// For each corner pair, if the corresponding corner is also occupied, there is a corner.
						for (int nCornerPair = 0; nCornerPair < lstCornerPairs.Count; nCornerPair++)
						{
							if (IsOccupied(lstCornerPairs[nCornerPair], eBuildLayer))
							{
								// We have a corner.

								if (m_dictBuildLayerBuiltCorners.ContainsKey(eBuildLayer))
								{
									BuildLayerBuiltCornerInfo cBuildLayerBuiltCornerInfo = m_dictBuildLayerBuiltCorners[eBuildLayer];

									// Add corner info to the original build slots entry, so we know its a pair.
									if (cBuildLayerBuiltCornerInfo.m_dictBuiltCorners.ContainsKey(eBuildSlot))
									{
										CornerInfo cCornerInfo = new CornerInfo();

										cCornerInfo.m_cGridInfo = this;
										cCornerInfo.m_eBuildLayer = eBuildLayer;
										cCornerInfo.m_eBuildSlot = lstCornerPairs[nCornerPair];

										cBuildLayerBuiltCornerInfo.m_dictBuiltCorners[eBuildSlot].Add(cCornerInfo);
									}
								}
							}
						}
					}
				}
			}
		}
	}

	public void RefreshStep2()
	{
		// For each layer.
		foreach (KeyValuePair<BuildLayer, BuildLayerBuiltCornerInfo> cLayerPair in m_dictBuildLayerBuiltCorners)
		{
			BuildLayer eBuildLayer = cLayerPair.Key;

			BuildLayerBuiltCornerInfo cBuildLayerBuiltCornerInfo = m_dictBuildLayerBuiltCorners[eBuildLayer];

			foreach (KeyValuePair<BuildSlot, List<CornerInfo>> cSlotPair in cBuildLayerBuiltCornerInfo.m_dictBuiltCorners)
			{
				CornerConnectionInfo cCornerConnectionInfo = new CornerConnectionInfo();

				BuildSlot eBuildSlot = cSlotPair.Key;

				List<CornerInfo> lstCornerInfo = cSlotPair.Value;

				List<BuildSlot> lstCornerPair = new List<BuildSlot>();

				if (m_dictCornerPairs.ContainsKey(eBuildSlot))
				{
					lstCornerPair.AddRange(m_dictCornerPairs[eBuildSlot]);
				}

				cCornerConnectionInfo.m_lstAB = lstCornerPair;

				for (int nCornerPair = 0; nCornerPair < lstCornerPair.Count; nCornerPair++)
				{
					// The matching corner we are trying to confirm exists.
					BuildSlot eCornerBuildSlot = lstCornerPair[nCornerPair];

					// Iterate the existing corners.
					for (int nExistingCornerInfo = 0; nExistingCornerInfo < lstCornerInfo.Count; nExistingCornerInfo++)
					{
						// If there is one which matches layer and corner, then con
						if (lstCornerInfo[nExistingCornerInfo].m_eBuildLayer == eBuildLayer && lstCornerInfo[nExistingCornerInfo].m_eBuildSlot == eCornerBuildSlot)
						{
							if (nCornerPair == 0)
							{
								cCornerConnectionInfo.OneAExists = true;
							}
							else
							{
								cCornerConnectionInfo.OneBExists = true;
							}

							break;
						}
					}
				}

				GridInfo cTouching = GridSettings.Instance.GetTouchingGridInfo(this, eBuildSlot, eBuildLayer);

				if (cTouching)
				{
					if (cTouching.m_dictBuildLayerBuiltCorners.ContainsKey(eBuildLayer))
					{
						BuildLayerBuiltCornerInfo cBuildLayerBuiltCornerInfoTouching = cTouching.m_dictBuildLayerBuiltCorners[eBuildLayer];

						if (cBuildLayerBuiltCornerInfoTouching.m_dictBuiltCorners.ContainsKey(GridUtilities.GetOppositeBuildSlot(eBuildSlot)))
						{
							List<CornerInfo> lstCornerInfoTouching = cBuildLayerBuiltCornerInfoTouching.m_dictBuiltCorners[GridUtilities.GetOppositeBuildSlot(eBuildSlot)];

							for (int nCornerPair = 0; nCornerPair < lstCornerPair.Count; nCornerPair++)
							{
								BuildSlot eCornerBuildSlot = lstCornerPair[nCornerPair];

								// Iterate the existing corners.
								for (int nExistingCornerInfo = 0; nExistingCornerInfo < lstCornerInfoTouching.Count; nExistingCornerInfo++)
								{
									if (lstCornerInfoTouching[nExistingCornerInfo].m_eBuildLayer == eBuildLayer && lstCornerInfoTouching[nExistingCornerInfo].m_eBuildSlot == eCornerBuildSlot)
									{
										if (nCornerPair == 0)
										{
											cCornerConnectionInfo.TwoAExists = true;
										}
										else
										{
											cCornerConnectionInfo.TwoBExists = true;
										}
									}
								}
							}
						}
					}
				}

				m_dictCorners[eBuildLayer].dictCorners[eBuildSlot] = cCornerConnectionInfo;
			}
		}
	}

	public void RefreshStep3()
	{
		foreach (KeyValuePair<BuildLayer, BuildLayerInfo> cPairLayer in m_dictBuildLayers)
		{
			BuildLayer eBuildLayer = cPairLayer.Key;

			// For each build slot that a corner is possible.
			foreach (KeyValuePair<BuildSlot, List<BuildSlot>> cPair in m_dictCornerPairs)
			{
				BuildSlot eBuildSlot = cPair.Key;

				// Block set entry is null as it should use whatever is stored in the BuildSlotInfo.
				RefreshBlockInfo(eBuildSlot, eBuildLayer, null);
			}
		}
	}

	void RefreshBlockInfo(BuildSlot eBuildSlot, BuildLayer eBuildLayer, BlockSetEntry cBlockSetEntry)
	{
		// Good for debugging when looking for a block which should exist.
		bool bOccupied = IsOccupied(eBuildSlot, eBuildLayer);

		// If that slot is occupied or this is a ghost highlight.
		if (bOccupied)
		{
			// BlockSetEntry BlockInfo to be created for occupation.
			GameObject cBlockToCreate = null;

			BuildSlotInfo cBuildSlotInfo = GetBuildSlotInfo(eBuildSlot, eBuildLayer);

			if (cBlockSetEntry == null)
			{
				cBlockSetEntry = cBuildSlotInfo.m_cBlockSetEntry;
			}

			BuildSlot eRotationBuildSlot = eBuildSlot;

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
				// If this block generates automatic corners.
				if (cBlockSetEntry.AutomaticCorners)
				{
					if (bOccupied)
					{
						//Debug.Log("");
					}

					CornerConnectionInfo cCornerConnectionInfo = m_dictCorners[eBuildLayer].dictCorners[eBuildSlot];

					int nConnectionCount = cCornerConnectionInfo.GetConnectionCount();

					if (nConnectionCount == 0)
					{
						// Flat wall.

						cBlockToCreate = cBlockSetEntry.BlockInfo.gameObject;
					}
					else if (nConnectionCount == 1)
					{
						// L shaped corner.

						if (bOccupied)
						{
							//Debug.Log("Getting corner for " + eBuildSlot + " and " + dictCorners[eBuildSlot][0].m_eBuildSlot);
						}

						BuildSlot eCornerPairBuildSlot = BuildSlot.Undefined;

						if (cCornerConnectionInfo.OneAExists)
						{
							eCornerPairBuildSlot = cCornerConnectionInfo.m_lstAB[0];
						}
						else if (cCornerConnectionInfo.OneBExists)
						{
							eCornerPairBuildSlot = cCornerConnectionInfo.m_lstAB[1];
						}

						if (eCornerPairBuildSlot != BuildSlot.Undefined)
						{
							// Get the slots of the left and right corner segments.
							GridUtilities.CornerInfo cCornerInfo = GridUtilities.GetCornerInfo(eBuildSlot, eCornerPairBuildSlot);

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
						else
						{
							// This will be executed when a block exists with a B connection.
							// B connection means its on a different grid info.
							// This B connection is picked up when that grid info is processed
							// (at which point it is an A connection as it belongs to that grid info).
							return;
						}
					}
					else if (nConnectionCount == 2)
					{
						// U or T corner.

						if (cCornerConnectionInfo.IsTCorner())
						{
							cBlockToCreate = cBlockSetEntry.TCorner.BlockInfo.gameObject;

							if (cCornerConnectionInfo.ShouldFlipTCorner())
							{
							}
						}
						else
						{
							Debug.Log("GridInfo: Skipping build of U or Z Corner.");

							return;
						}
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
				cBuildSlotInfo.m_cBlockInfo = CreateBlockInfo(cBlockToCreate, eBuildSlot, eBuildLayer, cBuildSlotInfo.m_bIsGhost);
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

	public void Move(GridInfo cDestinationGridInfo, BuildSlot eOriginBuildSlot, BuildLayer eOriginBuildLayer, BuildSlot eDestinationBuildSlot, BuildLayer eDestinationBuildLayer)
	{
		// Get the details for the slot being moved.
		BuildSlotInfo cBuildSlotInfo = GetBuildSlotInfo(eOriginBuildSlot, eOriginBuildLayer);

		if (cBuildSlotInfo != null)
		{
			if (cDestinationGridInfo != null)
			{
				// Set the destination to be occupied with the details from the slot being moved.
				cDestinationGridInfo.SetOccupiedInternal(eDestinationBuildSlot, eDestinationBuildLayer, cBuildSlotInfo.m_cBlockSetEntry, cBuildSlotInfo.m_bIsOpposite, false);

				if (HasOpposite(eOriginBuildSlot, eOriginBuildLayer))
				{
					BuildSlotInfo cOppositeBuildSlotInfo = GetBuildSlotInfo(eOriginBuildSlot, GridUtilities.GetOppositeBuildLayer(eOriginBuildLayer));

					cDestinationGridInfo.SetOccupiedInternal(eDestinationBuildSlot, GridUtilities.GetOppositeBuildLayer(eDestinationBuildLayer), cOppositeBuildSlotInfo.m_cBlockSetEntry, cOppositeBuildSlotInfo.m_bIsOpposite, false);

					SetUnoccupied(eOriginBuildSlot, GridUtilities.GetOppositeBuildLayer(eOriginBuildLayer));
				}
			}
		}

		// Set this gridinfo to be unoccupied in the original slot as it has been moved.
		SetUnoccupied(eOriginBuildSlot, eOriginBuildLayer);
	}

	public void ClearGhosts()
	{
		foreach (KeyValuePair<BuildLayer, BuildLayerInfo> cLayerPair in m_dictBuildLayers)
		{
			BuildLayer eBuildLayer = cLayerPair.Key;

			BuildLayerInfo cBuildLayerInfo = cLayerPair.Value;

			foreach (KeyValuePair<BuildSlot, BuildSlotInfo> cSlotPair in cBuildLayerInfo.m_dictBuildSlotOccupiers)
			{
				BuildSlot eBuildSlot = cSlotPair.Key;

				BuildSlotInfo cBuildSlotInfo = cSlotPair.Value;

				if (cBuildSlotInfo.m_bOccupied && cBuildSlotInfo.m_bIsGhost)
				{
					cBuildSlotInfo.m_bOccupied = false;
					cBuildSlotInfo.m_bIsGhost = false;
				}
			}
		}
	}
}
