using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BuildMode : BaseMode
{
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

	void Start()
	{
		// Enter.
	}

	void Update()
	{
		UpdateMouseHighlight();

		if (InputActions.Instance.Cancel())
		{
			if (GetSelectedBlock() != null)
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
						if (GetSelectedBlock() == null)
						{
							BlockSetEntry cBlockSetEntry = GetCurrentBlockSetEntry();

							GridInfo.BuildSlots eBuildSlot = m_eBuildDirection;

							if (cBlockSetEntry.IsCentreOnly())
							{
								eBuildSlot = GridInfo.BuildSlots.Centre;
							}

							if (cBlockSetEntry.CanBeBuilt(eBuildSlot))
							{
								GridInfo cGridInfo = GridUtilities.GetGridInfoFromCollider(cRaycastHit.collider);
								GridLayer cGridLayer = GridUtilities.GetGridLayerFromCollider(cRaycastHit.collider);

								if (cGridInfo.CanBeOccupied(eBuildSlot))
								{
									CreateBlock(cGridInfo, eBuildSlot, cGridLayer);
								}
							}
						}
						else
						{
							GridInfo cGridInfo = GridUtilities.GetGridInfoFromCollider(cRaycastHit.collider);

							BlockInfo cBlockInfo = GetSelectedBlock();

							GridInfo.BuildSlots eBuildSlot = m_eBuildDirection;

							if (cBlockInfo.IsCentreOnly())
							{
								eBuildSlot = GridInfo.BuildSlots.Centre;
							}

							if (cGridInfo.CanBeOccupied(eBuildSlot))
							{
								// Snap to grid.
								GetSelectedBlock().Move(cGridInfo, eBuildSlot, cBlockInfo.m_eGridLayer);

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
							cBlockInfo.Destroy();
						}
					}
					// This concept doesnt work with multiple build build slots on a single grid, as which one do you delete?
					// This used to delete the single occupier if you right clicked the grid.
					/*else if (cRaycastHit.collider.gameObject.layer == PhysicsLayers.Grid)
					{
						GridInfo cGridInfo = GetGridInfoFromCollider(cRaycastHit.collider);

						if (cGridInfo != null && cGridInfo.IsOccupied(m_eBuildDirection))
						{
							cGridInfo.GetOccupier(m_eBuildDirection).Destroy();
						}
					}*/
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
		}
	}

	void OnDestroy()
	{
		// Exit.
		DestroyBlockBuildHighlight();
	}

	BlockInfo CreateBlock(GridInfo cGridInfo, GridInfo.BuildSlots eBuildSlot, GridLayer cGridLayer, bool bIsGhost = false)
	{
		BlockSetEntry cCurrentBlockSetEntry = GetCurrentBlockSetEntry();

		if (cCurrentBlockSetEntry != null)
		{
			if (!bIsGhost)
			{
				cCurrentBlockSetEntry.Build();
			}

			GameObject cBlock = Instantiate(cCurrentBlockSetEntry.BlockInfo.gameObject);

			BlockInfo cBlockInfo = cBlock.GetComponent<BlockInfo>();

			cBlockInfo.Initialise(bIsGhost);

			if (eBuildSlot != GridInfo.BuildSlots.Centre)
			{
				cBlock.transform.rotation = m_dictBuildDirections[m_eBuildDirection];
			}
			else
			{
				// Just default centre blocks to north for now.
				cBlock.transform.rotation = m_dictBuildDirections[GridInfo.BuildSlots.North];
			}

			if (cBlockInfo != null)
			{
				cBlockInfo.Move(cGridInfo, eBuildSlot, cGridLayer.Layer);
			}

			return cBlockInfo;
		}

		return null;
	}

	public void UpdateMouseHighlight()
	{
		RaycastHit cRaycastHit;

		Ray cRay = Camera.main.ScreenPointToRay(Input.mousePosition);

		if (Physics.Raycast(cRay, out cRaycastHit, Mathf.Infinity, PhysicsLayers.GetPhysicsLayerMask(PhysicsLayers.Grid)))
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
				GridInfo.BuildSlots eBuildSlot = m_eBuildDirection;

				if (cBlockSetEntry.IsCentreOnly())
				{
					eBuildSlot = GridInfo.BuildSlots.Centre;
				}

				m_cBlockInfoBuildHighlight = CreateBlock(cGridInfo, eBuildSlot, cGridLayer, true);
			}

			if (m_cBlockInfoBuildHighlight != null)
			{
				m_cBlockInfoBuildHighlight.Move(cGridInfo, m_eBuildDirection, m_cBlockInfoBuildHighlight.m_eGridLayer);

				m_cBlockInfoBuildHighlight.transform.rotation = m_dictBuildDirections[m_eBuildDirection];

				// Set the colour.
				if (CurrencyManager.Instance.CurrencyAvailable(cBlockSetEntry.BlockCost))
				{
					m_cBlockInfoBuildHighlight.GetComponent<MeshRenderer>().material.color = GameGlobals.Instance.CanBuildColor;
				}
				else
				{
					m_cBlockInfoBuildHighlight.GetComponent<MeshRenderer>().material.color = GameGlobals.Instance.CannotBuildColor;
				}
			}

			GameGlobals.Instance.MouseHighlight.SetActive(true);
			GameGlobals.Instance.MouseHighlight.transform.position = cRaycastHit.collider.transform.position + new Vector3(0.0f, (cGridInfo.Height * 0.5f) + (GameGlobals.Instance.MouseHighlight.transform.localScale.y * 0.5f), 0.0f);
		}
		else
		{
			DestroyBlockBuildHighlight();

			GameGlobals.Instance.MouseHighlight.SetActive(false);
		}
	}

	void DestroyBlockBuildHighlight()
	{
		if (m_cBlockInfoBuildHighlight != null)
		{
			Destroy(m_cBlockInfoBuildHighlight.gameObject);
		}
	}
}
