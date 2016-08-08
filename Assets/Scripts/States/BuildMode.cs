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

	void Start()
	{
		// Enter.
	}

	void Update()
	{
		Application.Instance.UpdateMouseHighlight();

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
								//if(CurrencyManager.Instance.CurrencyAvailable(
								GameObject cBlock = Instantiate(GetBlockBuildType());

								BlockInfo cBlockInfo = cBlock.GetComponent<BlockInfo>();

								cBlock.transform.rotation = m_quatBuildDirection;

								if (cBlockInfo != null)
								{
									cBlockInfo.Move(cGridInfo);
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

			if (GetSelectedBlock() != null)
			{
				if (KeyboardInput.Instance.KeyDown(KeyCode.R))
				{
					GetSelectedBlock().Rotate(new Vector3(0.0f, 90.0f, 0.0f));
					m_quatBuildDirection = GetSelectedBlock().transform.rotation;
				}
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
	}
}
