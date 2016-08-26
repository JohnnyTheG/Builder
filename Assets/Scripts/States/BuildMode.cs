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

	bool m_bGridFlipping = false;

	void OnGridFlipStart()
	{
		m_bGridFlipping = true;
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
						// If there isnt a selected block.
						if (GetSelectedBlock() == null)
						{
							BlockSetEntry cBlockSetEntry = GetCurrentBlockSetEntry();

							GridInfo.BuildSlots eBuildSlot = m_eBuildDirection;
							GridLayer cGridLayer = GridUtilities.GetGridLayerFromCollider(cRaycastHit.collider);

							if (cBlockSetEntry.IsCentreOnly())
							{
								eBuildSlot = GridInfo.BuildSlots.Centre;
							}

							if (cBlockSetEntry.CanBeBuilt(eBuildSlot, cGridLayer.Layer))
							{
								GridInfo cGridInfo = GridUtilities.GetGridInfoFromCollider(cRaycastHit.collider);

								if (cGridInfo.CanBeOccupied(eBuildSlot, cGridLayer.Layer))
								{
									CreateBlock(cGridInfo, eBuildSlot, cGridLayer);
								}
							}
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

							if (cGridInfo.CanBeOccupied(eBuildSlot, cGridLayer.Layer))
							{
								// Snap to grid.
								GetSelectedBlock().Move(cGridInfo, eBuildSlot, cGridLayer.Layer);

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
		GridSettings.Instance.OnGridFlipStart -= OnGridFlipStart;
		GridSettings.Instance.OnGridFlipComplete -= OnGridFlipComplete;

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

			cBlock.transform.rotation = GetBlockRotation(eBuildSlot, cGridLayer.Layer);

			if (cBlockInfo != null)
			{
				cBlockInfo.Move(cGridInfo, eBuildSlot, cGridLayer.Layer);
			}

			return cBlockInfo;
		}

		return null;
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

		// Flip the block if its on the bottom.
		/*switch (eBuildLayer)
		{
			case GridInfo.BuildLayer.Top:

				vecRotation.z = 0.0f;

				break;

			case GridInfo.BuildLayer.Bottom:

				vecRotation.y += 180.0f;
				vecRotation.z += 180.0f;

				break;
		}*/

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
					m_cBlockInfoBuildHighlight.Move(cGridInfo, m_eBuildDirection, cGridLayer.Layer);

					m_cBlockInfoBuildHighlight.transform.rotation = GetBlockRotation(m_eBuildDirection, cGridLayer.Layer);

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
				DisableHighlights();
			}
		}
	}

	void DisableHighlights()
	{
		DestroyBlockBuildHighlight();

		GameGlobals.Instance.MouseHighlight.SetActive(false);
	}

	void DestroyBlockBuildHighlight()
	{
		if (m_cBlockInfoBuildHighlight != null)
		{
			Destroy(m_cBlockInfoBuildHighlight.gameObject);
		}
	}
}
