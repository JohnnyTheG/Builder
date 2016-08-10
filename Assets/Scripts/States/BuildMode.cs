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

		if (KeyboardInput.Instance.KeyDown(KeyCode.Escape))
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
		else if (KeyboardInput.Instance.KeyDown(KeyCode.Tab))
		{
			//Application.Instance.TrySetMode(Application.Mode.BuildEdit);
		}

		if (!KeyboardInput.Instance.KeyHeld(KeyCode.LeftAlt))
		{
			// Update.
			if (MouseInput.Instance.LeftMouseDown)
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

								// Check that currency is available.
								if (CurrencyManager.Instance.CurrencyAvailable(cBlockSetEntry.BlockCost))
								{
									CurrencyManager.Instance.SpendCurrency(cBlockSetEntry.BlockCost);
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

			if (MouseInput.Instance.RightMouseDown)
			{
				RaycastHit cRaycastHit;

				Ray cRay = Camera.main.ScreenPointToRay(Input.mousePosition);

				if (Physics.Raycast(cRay, out cRaycastHit, Mathf.Infinity, PhysicsLayers.GetPhysicsLayerMask(PhysicsLayers.Block)))
				{
					BlockInfo cBlockInfo = cRaycastHit.collider.gameObject.GetComponent<BlockInfo>();

					if (cBlockInfo)
					{
						cBlockInfo.Destroy();
					}
				}
			}

			if (KeyboardInput.Instance.KeyDown(KeyCode.R))
			{
				Vector3 vecEuler = m_quatBuildDirection.eulerAngles;

				vecEuler += new Vector3(0.0f, 90.0f, 0.0f);

				m_quatBuildDirection = Quaternion.Euler(vecEuler);
			}

			if (KeyboardInput.Instance.KeyDown(KeyCode.F))
			{
				if (GetSelectedBlock() != null)
				{
					CameraController.Instance.SetFocus(GetSelectedBlock().transform.position);
				}
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
