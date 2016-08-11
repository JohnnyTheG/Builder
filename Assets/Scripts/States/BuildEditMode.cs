/*using UnityEngine;
using System.Collections;

public class BuildEditMode : BaseMode
{
	public override void Shutdown(OnShutdownCompleteCallback OnShutdownComplete)
	{
		base.Shutdown(OnShutdownComplete);

		InvokeOnShutdownComplete();
	}

	public void Update()
	{
		if (KeyboardInput.Instance.KeyDown(KeyCode.Tab))
		{
			Application.Instance.TrySetMode(Application.Mode.Build);
		}

		if (!InputActions.Instance.RotateCamera())
		{
			// Update.
			if (InputActions.Instance.Select())
			{
				RaycastHit cRaycastHit;

				Ray cRay = Camera.main.ScreenPointToRay(Input.mousePosition);

				if (Physics.Raycast(cRay, out cRaycastHit, Mathf.Infinity))
				{
					if (cRaycastHit.collider.gameObject.layer == PhysicsLayers.Grid)
					{
						if (GetSelectedBlock() != null)
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
		}

		if (GetSelectedBlock() != null)
		{
			if (InputActions.Instance.Rotate())
			{
				GetSelectedBlock().Rotate(new Vector3(0.0f, 90.0f, 0.0f));
			}
		}

		if (InputActions.Instance.Delete())
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

		if (KeyboardInput.Instance.KeyDown(KeyCode.F))
		{
			if (GetSelectedBlock() != null)
			{
				CameraController.Instance.SetFocus(GetSelectedBlock().transform.position);
			}
		}
	}
}*/
