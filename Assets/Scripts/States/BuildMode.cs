using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BuildMode : BaseMode
{
	enum State
	{
		Build,
		DragBuild,
	}

	State m_eState = State.Build;

	public override void Shutdown(OnShutdownCompleteCallback OnShutdownComplete)
	{
		base.Shutdown(OnShutdownComplete);

		// Shut this down straight away.
		InvokeOnShutdownComplete();
	}

	Dictionary<GridInfo.BuildSlots, Quaternion> m_dictBuildDirections = new Dictionary<GridInfo.BuildSlots, Quaternion>()
	{
		{GridInfo.BuildSlots.North,     Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f))},
		{GridInfo.BuildSlots.East,      Quaternion.Euler(new Vector3(0.0f, 90.0f, 0.0f))},
		{GridInfo.BuildSlots.South,     Quaternion.Euler(new Vector3(0.0f, 180.0f, 0.0f))},
		{GridInfo.BuildSlots.West,      Quaternion.Euler(new Vector3(0.0f, 270.0f, 0.0f))},
	};

	GridInfo.BuildSlots m_eBuildDirection = GridInfo.BuildSlots.North;

	BlockInfo m_cBlockInfoBuildHighlight;
	List<BlockInfo> m_lstDragBuildHighlights = new List<BlockInfo>();

	bool m_bGridFlipping = false;

	void OnGridFlipStart()
	{
		m_bGridFlipping = true;

		switch (m_eState)
		{
			case State.DragBuild:

				// When grid starts flipping, cancel any drag build in progress.
				CancelDragBuild();

				break;
		}
	}

	void OnGridFlipComplete()
	{
		m_bGridFlipping = false;
	}

	void Start()
	{
		// Enter.
		GridSettings.Instance.OnGridFlipStart += OnGridFlipStart;
		GridSettings.Instance.OnGridFlipComplete += OnGridFlipComplete;
	}

	void Update()
	{
		UpdateMouseHighlight();

		if (m_bGridFlipping)
		{
			return;
		}

		if (InputActions.Instance.FlipGrid())
		{
			GridSettings.Instance.Flip();

			SetSelectedBlock(null);

			return;
		}

		if (InputActions.Instance.Cancel())
		{
			if (m_eState == State.DragBuild && m_lstDragBuildHighlights.Count > 0)
			{
				CancelDragBuild();
			}
			else if (GetSelectedBlock() != null)
			{
				SetSelectedBlock(null);
			}
			else
			{
				Application.Instance.TrySetMode(Application.Mode.BuildMenu);
			}
		}
		else if (KeyboardInput.Instance.KeyDown(KeyCode.BackQuote))
		{
			Application.Instance.TrySetMode(Application.Mode.RoomMapping);
		}

		if (!InputActions.Instance.RotateCamera())
		{
			if (InputActions.Instance.Select())
			{
				RaycastHit cRaycastHit;

				Ray cRay = Camera.main.ScreenPointToRay(Input.mousePosition);

				if (Physics.Raycast(cRay, out cRaycastHit, Mathf.Infinity))
				{
					if (cRaycastHit.collider.gameObject.layer == PhysicsLayers.Grid)
					{
						// If there isnt a selected block.
						if (GetSelectedBlock() == null)
						{
							GridInfo cGridInfo = GridUtilities.GetGridInfoFromCollider(cRaycastHit.collider);
							GridLayer cGridLayer = GridUtilities.GetGridLayerFromCollider(cRaycastHit.collider);

							StartDragBuild(cGridInfo, cGridLayer.Layer);
						}
						// There is a selected block.
						else
						{
							GridInfo cGridInfo = GridUtilities.GetGridInfoFromCollider(cRaycastHit.collider);
							GridLayer cGridLayer = GridUtilities.GetGridLayerFromCollider(cRaycastHit.collider);

							BlockInfo cBlockInfo = GetSelectedBlock();

							GridInfo.BuildSlots eBuildSlot = m_eBuildDirection;

							if (cBlockInfo.IsCentreOnly())
							{
								eBuildSlot = GridInfo.BuildSlots.Centre;
							}

							if (cGridInfo.CanBeOccupied(eBuildSlot, cGridLayer.Layer, cBlockInfo.HasOppositeBlock()))
							{
								// Snap to grid.
								GetSelectedBlock().Move(cGridInfo, eBuildSlot, cGridLayer.Layer, true);

								SetSelectedBlock(null);
							}
						}
					}
					else if (cRaycastHit.collider.gameObject.layer == PhysicsLayers.Block)
					{
						SetSelectedBlock(cRaycastHit.collider.gameObject.GetComponent<BlockInfo>());
					}
					else
					{
						SetSelectedBlock(null);
					}
				}
				else
				{
					SetSelectedBlock(null);
				}
			}

			if (InputActions.Instance.SelectHeld())
			{
				switch (m_eState)
				{
					case State.DragBuild:

						UpdateDragBuild();

						break;
				}
			}

			if (InputActions.Instance.SelectReleased())
			{
				switch (m_eState)
				{
					case State.DragBuild:

						FinishDragBuild();

						break;
				}
			}

			if (InputActions.Instance.RotateAnticlockwise())
			{
				m_eBuildDirection = Utils.GetArrayEntry<GridInfo.BuildSlots>(m_dictBuildDirections.Keys.ToArray(), (int)m_eBuildDirection, -1);
			}

			if (InputActions.Instance.RotateClockwise())
			{
				m_eBuildDirection = Utils.GetArrayEntry<GridInfo.BuildSlots>(m_dictBuildDirections.Keys.ToArray(), (int)m_eBuildDirection, 1);
			}

			switch (m_eState)
			{
				case State.Build:

					if (InputActions.Instance.Delete())
					{
						RaycastHit cRaycastHit;

						Ray cRay = Camera.main.ScreenPointToRay(Input.mousePosition);

						if (Physics.Raycast(cRay, out cRaycastHit, Mathf.Infinity, PhysicsLayers.GetPhysicsLayerMask(PhysicsLayers.Block)))
						{
							if (cRaycastHit.collider.gameObject.layer == PhysicsLayers.Block)
							{
								BlockInfo cBlockInfo = cRaycastHit.collider.gameObject.GetComponent<BlockInfo>();

								if (cBlockInfo != null)
								{
									cBlockInfo.DestroyBlockInfo(true);
								}
							}
						}
					}

					if (InputActions.Instance.Focus())
					{
						if (GetSelectedBlock() != null)
						{
							CameraController.Instance.SetFocus(GetSelectedBlock().transform.position);
						}
					}

					if (InputActions.Instance.NextBlockSetEntryCategory())
					{
						BlockManager.Instance.GetNextBlockSetEntryCategory(true);
					}

					if (InputActions.Instance.PreviousBlockSetEntryCategory())
					{
						BlockManager.Instance.GetPreviousBlockSetEntryCategory(true);
					}

					if (InputActions.Instance.NextBlockSetEntry())
					{
						BlockManager.Instance.GetNextBlock(true);
					}

					if (InputActions.Instance.PreviousBlockSetEntry())
					{
						BlockManager.Instance.GetPreviousBlock(true);
					}

					break;
			}
		}
	}

	void OnDestroy()
	{
		GridSettings.Instance.OnGridFlipStart -= OnGridFlipStart;
		GridSettings.Instance.OnGridFlipComplete -= OnGridFlipComplete;

		// Exit.
		DestroyBlockBuildHighlight();
	}

	void CreateBlockOnGrid(BlockSetEntry cBlockSetEntry, GridInfo cGridInfo, GridInfo.BuildSlots eBuildSlot, GridInfo.BuildLayer eBuildLayer)
	{
		if (cBlockSetEntry != null)
		{
			if (cBlockSetEntry.IsCentreOnly())
			{
				eBuildSlot = GridInfo.BuildSlots.Centre;
			}

			if (cBlockSetEntry.CanBeBuilt(eBuildSlot, eBuildLayer))
			{
				if (cGridInfo.CanBeOccupied(eBuildSlot, eBuildLayer, cBlockSetEntry.HasOppositeBlock()))
				{
					CreateBlock(cGridInfo, eBuildSlot, eBuildLayer);
				}
			}
		}
	}

	BlockInfo CreateBlock(GridInfo cGridInfo, GridInfo.BuildSlots eBuildSlot, GridInfo.BuildLayer eBuildLayer, bool bIsGhost = false)
	{
		BlockSetEntry cCurrentBlockSetEntry = GetCurrentBlockSetEntry();

		if (cCurrentBlockSetEntry != null)
		{
			if (!bIsGhost)
			{
				cCurrentBlockSetEntry.Build();
			}

			BlockInfo cBlock = CreateBlockGameObject(cCurrentBlockSetEntry.BlockInfo.gameObject, cGridInfo, eBuildSlot, eBuildLayer, bIsGhost);

			BlockInfo cOpposite = null;

			if (cCurrentBlockSetEntry.HasOppositeBlock())
			{
				cOpposite = CreateBlockGameObject(cCurrentBlockSetEntry.OppositeBlockInfo.gameObject, cGridInfo, eBuildSlot, GridUtilities.GetOppositeBuildLayer(eBuildLayer), bIsGhost);
				cBlock.m_cOppositeBlockInfo = cOpposite;
			}

			cBlock.m_cOppositeBlockInfo = cOpposite;

			return cBlock;
		}

		return null;
	}

	BlockInfo CreateBlockGameObject(GameObject cBlockToCreate, GridInfo cGridInfo, GridInfo.BuildSlots eBuildSlot, GridInfo.BuildLayer eGridLayer, bool bIsGhost)
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

	Quaternion GetBlockRotation(GridInfo.BuildSlots eBuildSlot, GridInfo.BuildLayer eBuildLayer)
	{
		Vector3 vecRotation = Vector3.zero;

		switch (eBuildSlot)
		{
			case GridInfo.BuildSlots.Centre:

				// Just default centre blocks to north for now.
				vecRotation = m_dictBuildDirections[GridInfo.BuildSlots.North].eulerAngles;

				break;

			default:

				vecRotation = m_dictBuildDirections[m_eBuildDirection].eulerAngles;

				break;
		}

		if (GridSettings.Instance.UpBuildLayer != eBuildLayer)
		{
			vecRotation += new Vector3(0.0f, 180.0f, 180.0f);
		}

		return Quaternion.Euler(vecRotation);
	}

	public void UpdateMouseHighlight()
	{
		// If the grid is flipping, then get rid of highlighting.
		if (m_bGridFlipping)
		{
			DisableHighlights();
		}
		else
		{
			switch (m_eState)
			{
				case State.Build:

					UpdateMouseHighlightBuild();

					break;

				case State.DragBuild:

					UpdateMouseHighlightDragBuild();

					break;
			}
		}
	}

	void UpdateMouseHighlightDragBuild()
	{
		// Ensure there are no highlights from a normal build.
		DestroyBlockBuildHighlight();

		// Get rid of existing highlights.
		DestroyDragBuildHighlights();

		// Hide the ground square icon.
		GameGlobals.Instance.MouseHighlight.SetActive(false);

		BlockSetEntry cBlockSetEntry = GetCurrentBlockSetEntry();

		GridInfo[] acGridLine = GridSettings.Instance.GetGridLine(m_cDragBuildStartGridInfo, m_cDragBuildFinishGridInfo);

		// Block cost times number of blocks in line.
		bool bCanAffordBuild = CanAffordBlocks(GetCurrentBlockSetEntry().BlockCost, acGridLine.Length);

		GridInfo.BuildSlots eBuildSlot = GetBuildDirection(cBlockSetEntry);

		for (int nGridInfo = 0; nGridInfo < acGridLine.Length; nGridInfo++)
		{
			BlockInfo cBlockInfo = CreateBlock(acGridLine[nGridInfo], eBuildSlot, m_eDragBuildLayer, true);

			SetHighlightColour(cBlockInfo, bCanAffordBuild);

			m_lstDragBuildHighlights.Add(cBlockInfo);
		}
	}

	void UpdateMouseHighlightBuild()
	{
		// Ensure there are no highlights from a drag build.
		DestroyDragBuildHighlights();

		RaycastHit cRaycastHit;

		if (GridUtilities.RaycastForGridFromMouse(out cRaycastHit))
		{
			GridInfo cGridInfo = GridUtilities.GetGridInfoFromCollider(cRaycastHit.collider);
			GridLayer cGridLayer = GridUtilities.GetGridLayerFromCollider(cRaycastHit.collider);

			BlockSetEntry cBlockSetEntry = GetCurrentBlockSetEntry();

			// If the build type has changed, then get rid of the current highlight. Then further down new one is spawned.
			if ((cBlockSetEntry == null) || (m_cBlockInfoBuildHighlight != null && (m_cBlockInfoBuildHighlight.Name != cBlockSetEntry.BlockInfo.Name)))
			{
				DestroyBlockBuildHighlight();
				m_cBlockInfoBuildHighlight = null;
			}

			if (cBlockSetEntry != null && m_cBlockInfoBuildHighlight == null)
			{
				GridInfo.BuildSlots eBuildSlot = GetBuildDirection(cBlockSetEntry);

				m_cBlockInfoBuildHighlight = CreateBlock(cGridInfo, eBuildSlot, cGridLayer.Layer, true);
			}

			if (m_cBlockInfoBuildHighlight != null)
			{
				m_cBlockInfoBuildHighlight.Move(cGridInfo, m_eBuildDirection, cGridLayer.Layer, true);

				m_cBlockInfoBuildHighlight.transform.rotation = GetBlockRotation(m_eBuildDirection, cGridLayer.Layer);

				if (m_cBlockInfoBuildHighlight.HasOppositeBlock())
				{
					m_cBlockInfoBuildHighlight.m_cOppositeBlockInfo.transform.rotation = GetBlockRotation(m_eBuildDirection, GridUtilities.GetOppositeBuildLayer(cGridLayer.Layer));
				}

				// Only creating a single block, hence the 1.
				SetHighlightColour(m_cBlockInfoBuildHighlight, CanAffordBlocks(cBlockSetEntry.BlockCost, 1));
			}

			GameGlobals.Instance.MouseHighlight.SetActive(true);
			GameGlobals.Instance.MouseHighlight.transform.position = cRaycastHit.collider.transform.position + new Vector3(0.0f, (cGridInfo.Height * 0.5f) + (GameGlobals.Instance.MouseHighlight.transform.localScale.y * 0.5f), 0.0f);
		}
		else
		{
			DisableHighlights();
		}
	}

	bool CanAffordBlocks(int nBlockCost, int nBlockCount)
	{
		return CurrencyManager.Instance.CurrencyAvailable(nBlockCost * nBlockCount);
	}

	void SetHighlightColour(BlockInfo cBlockInfo, bool bCanAffordBuild)
	{
		if (bCanAffordBuild)
		{
			cBlockInfo.GetComponent<MeshRenderer>().material.color = GameGlobals.Instance.CanBuildColor;
		}
		else
		{
			cBlockInfo.GetComponent<MeshRenderer>().material.color = GameGlobals.Instance.CannotBuildColor;
		}
	}

	GridInfo.BuildSlots GetBuildDirection(BlockSetEntry cBlockSetEntry)
	{
		GridInfo.BuildSlots eBuildSlot = m_eBuildDirection;

		if (cBlockSetEntry.IsCentreOnly())
		{
			eBuildSlot = GridInfo.BuildSlots.Centre;
		}

		return eBuildSlot;
	}

	void DisableHighlights()
	{
		DestroyBlockBuildHighlight();

		DestroyDragBuildHighlights();

		GameGlobals.Instance.MouseHighlight.SetActive(false);
	}

	void DestroyBlockBuildHighlight()
	{
		if (m_cBlockInfoBuildHighlight != null)
		{
			m_cBlockInfoBuildHighlight.DestroyBlockInfo(true);
		}
	}

	void DestroyDragBuildHighlights()
	{
		for (int nHighlight = m_lstDragBuildHighlights.Count - 1; nHighlight >= 0; nHighlight--)
		{
			m_lstDragBuildHighlights[nHighlight].DestroyBlockInfo(true);

			m_lstDragBuildHighlights.RemoveAt(nHighlight);
		}
	}

	GridInfo m_cDragBuildStartGridInfo = null;
	GridInfo m_cDragBuildFinishGridInfo = null;

	GridInfo.BuildLayer m_eDragBuildLayer = GridInfo.BuildLayer.Top;

	void StartDragBuild(GridInfo cGridInfo, GridInfo.BuildLayer eBuildLayer)
	{
		Debug.Log("Starting Drag Build");

		m_eState = State.DragBuild;

		// Start position of the drag build.
		m_cDragBuildStartGridInfo = cGridInfo;

		m_eDragBuildLayer = eBuildLayer;
	}

	void UpdateDragBuild()
	{
		Debug.Log("Updating Drag Build");

		BlockSetEntry cBlockSetEntry = GetCurrentBlockSetEntry();

		// If the block can be drag built.
		if (cBlockSetEntry.CanDragBuild)
		{
			RaycastHit cRaycastHit;

			// Find the grid and then set that as the finish position.
			if (GridUtilities.RaycastForGridFromMouse(out cRaycastHit))
			{
				GridInfo cGridInfo = GridUtilities.GetGridInfoFromCollider(cRaycastHit.collider);

				m_cDragBuildFinishGridInfo = cGridInfo;
			}
		}
		else
		{
			// Cant drag build this block. It should be spawned on the start position only when key is released.
			m_cDragBuildFinishGridInfo = m_cDragBuildStartGridInfo;
		}
	}

	void FinishDragBuild()
	{
		Debug.Log("Finishing Drag Build");

		m_eState = State.Build;

		GridInfo[] acGridLine = GridSettings.Instance.GetGridLine(m_cDragBuildStartGridInfo, m_cDragBuildFinishGridInfo);

		BlockSetEntry cBlockSetEntry = GetCurrentBlockSetEntry();

		if (CanAffordBlocks(cBlockSetEntry.BlockCost, acGridLine.Length))
		{
			for (int nGridInfo = 0; nGridInfo < acGridLine.Length; nGridInfo++)
			{
				CreateBlockOnGrid(GetCurrentBlockSetEntry(), acGridLine[nGridInfo], m_eBuildDirection, m_eDragBuildLayer);
			}
		}
	}

	void CancelDragBuild()
	{
		Debug.Log("Cancelling Drag Build");

		m_eState = State.Build;
	}
}
