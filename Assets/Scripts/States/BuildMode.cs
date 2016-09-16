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

	Dictionary<GridInfo.BuildSlot, Quaternion> m_dictBuildDirections = new Dictionary<GridInfo.BuildSlot, Quaternion>()
	{
		{GridInfo.BuildSlot.North,     Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f))},
		{GridInfo.BuildSlot.East,      Quaternion.Euler(new Vector3(0.0f, 90.0f, 0.0f))},
		{GridInfo.BuildSlot.South,     Quaternion.Euler(new Vector3(0.0f, 180.0f, 0.0f))},
		{GridInfo.BuildSlot.West,      Quaternion.Euler(new Vector3(0.0f, 270.0f, 0.0f))},
	};

	GridInfo.BuildSlot m_eBuildSlot = GridInfo.BuildSlot.North;

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
				//Application.Instance.TrySetMode(Application.Mode.BuildMenu);
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

							GridInfo.BuildSlot eBuildSlot = m_eBuildSlot;

							if (cBlockInfo.IsCentreOnly())
							{
								eBuildSlot = GridInfo.BuildSlot.Centre;
							}

							if (cGridInfo.CanBeOccupied(eBuildSlot, cGridLayer.Layer, cGridInfo.HasOpposite(eBuildSlot, cGridLayer.Layer)))
							{
								// Snap to grid.
								BlockInfo cMovingBlockInfo = GetSelectedBlock();

								cMovingBlockInfo.GridInfo.Move(cGridInfo, cMovingBlockInfo.BuildSlot, cMovingBlockInfo.BuildLayer, eBuildSlot, cGridLayer.Layer);
							}

							SetSelectedBlock(null);
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
				m_eBuildSlot = Utils.GetArrayEntry<GridInfo.BuildSlot>(m_dictBuildDirections.Keys.ToArray(), (int)m_eBuildSlot, -1);
			}

			if (InputActions.Instance.RotateClockwise())
			{
				m_eBuildSlot = Utils.GetArrayEntry<GridInfo.BuildSlot>(m_dictBuildDirections.Keys.ToArray(), (int)m_eBuildSlot, 1);
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
									cBlockInfo.GridInfo.SetUnoccupied(cBlockInfo.BuildSlot, cBlockInfo.BuildLayer);

									// If this block has an opposite, then clear its opposite too.
									if(cBlockInfo.GridInfo.HasOpposite(cBlockInfo.BuildSlot, cBlockInfo.BuildLayer))
									{
										cBlockInfo.GridInfo.SetUnoccupied(cBlockInfo.BuildSlot, GridUtilities.GetOppositeBuildLayer(cBlockInfo.BuildLayer));
									}
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
		DestroyBuildHighlights();
	}

	void ValidateAndCreateBlock(BlockSetEntry cBlockSetEntry, GridInfo cGridInfo, GridInfo.BuildSlot eBuildSlot, GridInfo.BuildLayer eBuildLayer)
	{
		if (cBlockSetEntry != null)
		{
			if (cBlockSetEntry.IsCentreOnly())
			{
				eBuildSlot = GridInfo.BuildSlot.Centre;
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

	BlockInfo CreateBlock(GridInfo cGridInfo, GridInfo.BuildSlot eBuildSlot, GridInfo.BuildLayer eBuildLayer, bool bIsGhost = false, BlockSetEntry cBlockSetEntry = null)
	{
		BlockSetEntry cCurrentBlockSetEntry;

		// If there is a force block set passed as a parameter, this must be the BlockSetEntry we create from.
		if (cBlockSetEntry != null)
		{
			cCurrentBlockSetEntry = cBlockSetEntry;
		}
		else
		{
			// No passed block set, use the menu based one.
			cCurrentBlockSetEntry = GetCurrentBlockSetEntry();
		}

		// If there is a block set entry to be built.
		if (cCurrentBlockSetEntry != null)
		{
			cGridInfo.SetOccupied(eBuildSlot, eBuildLayer, cCurrentBlockSetEntry);

			//cGridInfo.SetOccupied(eBuildSlot, eBuildLayer, cCurrentBlockSetEntry, false);

			//if (cCurrentBlockSetEntry.HasOppositeBlock())
			//{
			//	cGridInfo.SetOccupied(eBuildSlot, GridUtilities.GetOppositeBuildLayer(eBuildLayer), cCurrentBlockSetEntry, true);
			//}

			//// Fill in the corresponding blocks on touching grid squares but only if not centre (as centre isnt shared).
			//if (eBuildSlot != GridInfo.BuildSlot.Centre)
			//{
			//	// Fill the opposite grid info as both are "occupied".
			//	GridInfo cTouchingGridInfo = GridSettings.Instance.GetTouchingGridInfo(cGridInfo, eBuildSlot, eBuildLayer);

			//	GridInfo.BuildSlot eOppositeBuildSlot = GridUtilities.GetOppositeBuildSlot(eBuildSlot);

			//	if (cTouchingGridInfo != null)
			//	{
			//		cTouchingGridInfo.SetOccupied(eOppositeBuildSlot, eBuildLayer, cCurrentBlockSetEntry, false);
			//	}

			//	if (cCurrentBlockSetEntry.HasOppositeBlock())
			//	{
			//		cTouchingGridInfo.SetOccupied(eOppositeBuildSlot, GridUtilities.GetOppositeBuildLayer(eBuildLayer), cCurrentBlockSetEntry, true);
   //             }
			//}
		}

		return null;
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
		// Get rid of existing highlights.
		DestroyBuildHighlights();

		// Hide the ground square icon.
		GameGlobals.Instance.MouseHighlight.SetActive(false);

		BlockSetEntry cBlockSetEntry = GetCurrentBlockSetEntry();

		if (cBlockSetEntry != null)
		{
			GridInfo[] acGridLine = GridSettings.Instance.GetGridLine(m_cDragBuildStartGridInfo, m_cDragBuildFinishGridInfo);

			// Block cost times number of blocks in line.
			bool bCanAffordBuild = CanAffordBlocks(cBlockSetEntry.BlockCost, acGridLine.Length);

			GridInfo.BuildSlot eBuildSlot = GetBuildDirection(cBlockSetEntry);

			for (int nGridInfo = 0; nGridInfo < acGridLine.Length; nGridInfo++)
			{
				acGridLine[nGridInfo].SetHighlighted(eBuildSlot, m_eDragBuildLayer, cBlockSetEntry);

				//BlockInfo cBlockInfo = CreateBlock(acGridLine[nGridInfo], eBuildSlot, m_eDragBuildLayer, true);

				//SetHighlightColour(cBlockInfo, bCanAffordBuild);

				//m_lstDragBuildHighlights.Add(cBlockInfo);
			}
		}
	}

	void UpdateMouseHighlightBuild()
	{
		// Ensure there are no highlights from a drag build.
		DestroyBuildHighlights();

		RaycastHit cRaycastHit;

		if (GridUtilities.RaycastForGridFromMouse(out cRaycastHit))
		{
			GridInfo cGridInfo = GridUtilities.GetGridInfoFromCollider(cRaycastHit.collider);
			GridLayer cGridLayer = GridUtilities.GetGridLayerFromCollider(cRaycastHit.collider);

			BlockSetEntry cBlockSetEntry = GetCurrentBlockSetEntry();

			if (cBlockSetEntry != null)
			{
				GridInfo.BuildSlot eBuildSlot = GetBuildDirection(cBlockSetEntry);
				GridInfo.BuildLayer eBuildLayer = cGridLayer.Layer;

				cGridInfo.SetHighlighted(eBuildSlot, eBuildLayer, cBlockSetEntry);
			}

			// Always get rid of the block.
			/*DestroyBlockBuildHighlight();
			m_cBlockInfoBuildHighlight = null;

			if (cBlockSetEntry != null && m_cBlockInfoBuildHighlight == null)
			{
				//GridInfo.BuildSlot eBuildSlot = GetBuildDirection(cBlockSetEntry);

				//m_cBlockInfoBuildHighlight = CreateBlock(cGridInfo, eBuildSlot, cGridLayer.Layer, true);
			}

			if (m_cBlockInfoBuildHighlight != null)
			{
				m_cBlockInfoBuildHighlight.Move(cGridInfo, m_eBuildSlot, cGridLayer.Layer, true);

				m_cBlockInfoBuildHighlight.transform.rotation = GetBlockRotation(m_eBuildSlot, cGridLayer.Layer);

				if (m_cBlockInfoBuildHighlight.HasOppositeBlock())
				{
					m_cBlockInfoBuildHighlight.m_cOppositeBlockInfo.transform.rotation = GetBlockRotation(m_eBuildSlot, GridUtilities.GetOppositeBuildLayer(cGridLayer.Layer));
				}

				// Only creating a single block, hence the 1.
				SetHighlightColour(m_cBlockInfoBuildHighlight, CanAffordBlocks(cBlockSetEntry.BlockCost, 1));
			}*/

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

	GridInfo.BuildSlot GetBuildDirection(BlockSetEntry cBlockSetEntry)
	{
		GridInfo.BuildSlot eBuildSlot = m_eBuildSlot;

		if (cBlockSetEntry.IsCentreOnly())
		{
			eBuildSlot = GridInfo.BuildSlot.Centre;
		}

		return eBuildSlot;
	}

	void DisableHighlights()
	{
		DestroyBuildHighlights();

		GameGlobals.Instance.MouseHighlight.SetActive(false);
	}

	void DestroyBuildHighlights()
	{
		GridInfo[,] acGrid = GridSettings.Instance.Grid;

		for (int nGridX = 0; nGridX < acGrid.GetLength(0); nGridX++)
		{
			for (int nGridY = 0; nGridY < acGrid.GetLength(1); nGridY++)
			{
				acGrid[nGridX, nGridY].SetUnhighlighted();
			}
		}
	}

	GridInfo m_cDragBuildStartGridInfo = null;
	GridInfo m_cDragBuildFinishGridInfo = null;

	GridInfo.BuildLayer m_eDragBuildLayer = GridInfo.BuildLayer.Top;

	void StartDragBuild(GridInfo cGridInfo, GridInfo.BuildLayer eBuildLayer)
	{
		m_eState = State.DragBuild;

		// Start position of the drag build.
		m_cDragBuildStartGridInfo = cGridInfo;

		m_eDragBuildLayer = eBuildLayer;
	}

	void UpdateDragBuild()
	{
		BlockSetEntry cBlockSetEntry = GetCurrentBlockSetEntry();

		if (cBlockSetEntry != null)
		{
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
	}

	void FinishDragBuild()
	{
		m_eState = State.Build;

		GridInfo[] acGridLine = GridSettings.Instance.GetGridLine(m_cDragBuildStartGridInfo, m_cDragBuildFinishGridInfo);

		BlockSetEntry cBlockSetEntry = GetCurrentBlockSetEntry();

		if (CanAffordBlocks(cBlockSetEntry.BlockCost, acGridLine.Length))
		{
			for (int nGridInfo = 0; nGridInfo < acGridLine.Length; nGridInfo++)
			{
				ValidateAndCreateBlock(GetCurrentBlockSetEntry(), acGridLine[nGridInfo], m_eBuildSlot, m_eDragBuildLayer);
			}
		}
	}

	void CancelDragBuild()
	{
		m_eState = State.Build;
	}

	void PairCorner(BlockInfo cBlockA, BlockInfo cBlockB)
	{
		cBlockA.PairedCorner = cBlockB;
		cBlockB.PairedCorner = cBlockA;
	}
}
