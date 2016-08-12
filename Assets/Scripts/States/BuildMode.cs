using UnityEngine;
using System.Collections;
using System;

public class BuildMode : BaseMode
{
	public override void Shutdown(OnShutdownCompleteCallback OnShutdownComplete)
	{
		base.Shutdown(OnShutdownComplete);

		// Shut this down straight away.
		InvokeOnShutdownComplete();
	}

	Quaternion m_quatBuildDirection = Quaternion.Euler(Vector3.zero);

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

							if (cGridInfo.CanBeOccupied)
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

							if (cGridInfo.CanBeOccupied)
							{
								// Snap to grid.
								GetSelectedBlock().Move(cGridInfo);

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

				if (Physics.Raycast(cRay, out cRaycastHit, Mathf.Infinity, PhysicsLayers.GetPhysicsLayerMask(PhysicsLayers.Block, PhysicsLayers.Grid)))
				{
					if (cRaycastHit.collider.gameObject.layer == PhysicsLayers.Block)
					{
						BlockInfo cBlockInfo = cRaycastHit.collider.gameObject.GetComponent<BlockInfo>();

						if (cBlockInfo != null)
						{
							cBlockInfo.Destroy();
						}
					}
					else if (cRaycastHit.collider.gameObject.layer == PhysicsLayers.Grid)
					{
						GridInfo cGridInfo = cRaycastHit.collider.gameObject.GetComponent<GridInfo>();

						if (cGridInfo != null && cGridInfo.IsOccupied())
						{
							cGridInfo.Occupier.Destroy();
						}
					}
				}
			}

			if (InputActions.Instance.RotateAnticlockwise())
			{
				Vector3 vecEuler = m_quatBuildDirection.eulerAngles;

				vecEuler -= new Vector3(0.0f, 90.0f, 0.0f);

				m_quatBuildDirection = Quaternion.Euler(vecEuler);
			}

			if (InputActions.Instance.RotateClockwise())
			{
				Vector3 vecEuler = m_quatBuildDirection.eulerAngles;

				vecEuler += new Vector3(0.0f, 90.0f, 0.0f);

				m_quatBuildDirection = Quaternion.Euler(vecEuler);
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

	BlockInfo CreateBlock(GridInfo cGridInfo, bool bIsGhost = false)
	{
		BlockSetEntry cCurrentBlockSetEntry = GetCurrentBlockSetEntry();

		if (cCurrentBlockSetEntry != null)
		{
			cCurrentBlockSetEntry.Build();

			GameObject cBlock = Instantiate(cCurrentBlockSetEntry.BlockInfo.gameObject);

			BlockInfo cBlockInfo = cBlock.GetComponent<BlockInfo>();

			cBlockInfo.Initialise(bIsGhost);

			cBlock.transform.rotation = m_quatBuildDirection;

			if (cBlockInfo != null)
			{
				cBlockInfo.Move(cGridInfo);
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
				m_cBlockInfoBuildHighlight.Move(cGridInfo);

				m_cBlockInfoBuildHighlight.transform.rotation = m_quatBuildDirection;

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
