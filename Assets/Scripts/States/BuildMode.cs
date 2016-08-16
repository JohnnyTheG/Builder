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

	//Quaternion m_quatBuildDirection = Quaternion.Euler(Vector3.zero);

	Dictionary<GridInfo.BuildSlots, Quaternion> m_dictBuildDirections = new Dictionary<GridInfo.BuildSlots, Quaternion>()
	{
		{GridInfo.BuildSlots.North,     Quaternion.Euler(new Vector3(0.0f, 0.0f, 0.0f))},
		{GridInfo.BuildSlots.East,		Quaternion.Euler(new Vector3(0.0f, 90.0f, 0.0f))},
		{GridInfo.BuildSlots.South,		Quaternion.Euler(new Vector3(0.0f, 180.0f, 0.0f))},
		{GridInfo.BuildSlots.West,		Quaternion.Euler(new Vector3(0.0f, 270.0f, 0.0f))},
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
		//else if (KeyboardInput.Instance.KeyDown(KeyCode.Tab))
		//{
		//	//Application.Instance.TrySetMode(Application.Mode.BuildEdit);
		//}

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
							GridInfo cGridInfo = cRaycastHit.collider.GetComponent<GridInfo>();

							if (cGridInfo.CanBeOccupied(m_eBuildDirection))
							{
								BlockSetEntry cBlockSetEntry = GetCurrentBlockSetEntry();

								if (cBlockSetEntry.CanBeBuilt())
								{
									CreateBlock(cGridInfo);
								}
							}
						}
						else
						{
							GridInfo cGridInfo = cRaycastHit.collider.GetComponent<GridInfo>();

							if (cGridInfo.CanBeOccupied(m_eBuildDirection))
							{
								// Snap to grid.
								GetSelectedBlock().Move(cGridInfo, m_eBuildDirection);

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
						GridInfo cGridInfo = cRaycastHit.collider.gameObject.GetComponent<GridInfo>();

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

			Debug.Log("Build Direction: " + m_eBuildDirection.ToString());

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

	BlockInfo CreateBlock(GridInfo cGridInfo, bool bIsGhost = false)
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

			cBlock.transform.rotation = m_dictBuildDirections[m_eBuildDirection];

			if (cBlockInfo != null)
			{
				cBlockInfo.Move(cGridInfo, m_eBuildDirection);
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
			GridInfo cGridInfo = cRaycastHit.collider.GetComponent<GridInfo>();

			// If the build type has changed, then get rid of the current highlight. Then further down new one is spawned.
			if ((GetCurrentBlockSetEntry() == null) || (m_cBlockInfoBuildHighlight != null && (m_cBlockInfoBuildHighlight.Name != GetCurrentBlockSetEntry().BlockInfo.Name)))
			{
				DestroyBlockBuildHighlight();
				m_cBlockInfoBuildHighlight = null;
			}

			if (m_cBlockInfoBuildHighlight == null)
			{
				m_cBlockInfoBuildHighlight = CreateBlock(cGridInfo, true);
			}

			if (m_cBlockInfoBuildHighlight != null)
			{
				m_cBlockInfoBuildHighlight.Move(cGridInfo, m_eBuildDirection);

				m_cBlockInfoBuildHighlight.transform.rotation = m_dictBuildDirections[m_eBuildDirection];

				BlockSetEntry cCurrentBlockSetEntry = GetCurrentBlockSetEntry();

				// Set the colour.
				if (CurrencyManager.Instance.CurrencyAvailable(cCurrentBlockSetEntry.BlockCost))
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
