using UnityEngine;
using System.Collections;

public class RoomMappingMode : BaseMode
{
	GridInfo m_cGridInfoStart;
	GridInfo m_cGridInfoFinish;

	GridInfo[] m_acCurrentGridSelection;

	public override void Shutdown(OnShutdownCompleteCallback OnShutdownComplete)
	{
		base.Shutdown(OnShutdownComplete);

		// Clear this on exit, to remove highlighting.
		ClearCurrentGridSelection();

		InvokeOnShutdownComplete();
	}

	public void Start()
	{
		// Clear any selected block
		SetSelectedBlock(null);
	}

	public void Update()
	{
		UpdateMouseHighlight();

        if (KeyboardInput.Instance.KeyDown(KeyCode.BackQuote))
		{
			Application.Instance.TrySetMode(Application.Mode.Build);
		}

		if (!InputActions.Instance.RotateCamera())
		{
			RaycastHit cRaycastHit;

			if (m_acCurrentGridSelection == null)
			{
				if (InputActions.Instance.Select())
				{
					// If a new selection starts, clear the old selection.
					ClearCurrentGridSelection();

					if (RaycastForGrid(out cRaycastHit))
					{
						m_cGridInfoStart = cRaycastHit.collider.GetComponent<GridInfo>();
					}
				}

				if (InputActions.Instance.SelectHeld())
				{
					if (RaycastForGrid(out cRaycastHit))
					{
						m_cGridInfoFinish = cRaycastHit.collider.GetComponent<GridInfo>();
					}
					else
					{
						// Hit nothing, so no grid selection.
						m_cGridInfoFinish = null;
					}
				}

				if (InputActions.Instance.SelectReleased())
				{
					if (m_cGridInfoStart != null && m_cGridInfoFinish != null)
					{
						m_acCurrentGridSelection = GridSettings.Instance.GetGridSelection(m_cGridInfoStart, m_cGridInfoFinish);

						Debug.Log("RoomMappingMode: Selection contained " + m_acCurrentGridSelection.Length + " grid squares.");

						for (int nGridInfo = 0; nGridInfo < m_acCurrentGridSelection.Length; nGridInfo++)
						{
							m_acCurrentGridSelection[nGridInfo].Highlight();
						}
					}
				}
			}
			else
			{
				if (InputActions.Instance.ConfirmMapRoom())
				{
					MapCurrentGridSelection();
				}
				else if (InputActions.Instance.SelectReleased() || InputActions.Instance.Cancel())
				{
					ClearCurrentGridSelection();
				}
			}
		}
	}

	bool RaycastForGrid(out RaycastHit cRaycastHit)
	{
		Ray cRay = Camera.main.ScreenPointToRay(Input.mousePosition);

		return Physics.Raycast(cRay, out cRaycastHit, Mathf.Infinity, PhysicsLayers.GetPhysicsLayerMask(PhysicsLayers.Grid));
	}

	void ClearCurrentGridSelection()
	{
		if (m_acCurrentGridSelection != null)
		{
			for (int nGridInfo = 0; nGridInfo < m_acCurrentGridSelection.Length; nGridInfo++)
			{
				m_acCurrentGridSelection[nGridInfo].Dehighlight();
			}

			m_acCurrentGridSelection = null;
		}
	}

	void MapCurrentGridSelection()
	{
		if (m_acCurrentGridSelection != null)
		{
			RoomManager.Instance.RegisterRoom(m_acCurrentGridSelection);
		}
	}

	public void UpdateMouseHighlight()
	{
		RaycastHit cRaycastHit;

		if (RaycastForGrid(out cRaycastHit))
		{
			GridInfo cGridInfo = cRaycastHit.collider.GetComponent<GridInfo>();

			GameGlobals.Instance.MouseHighlight.SetActive(true);
			GameGlobals.Instance.MouseHighlight.transform.position = cRaycastHit.collider.transform.position + new Vector3(0.0f, (cGridInfo.Height * 0.5f) + (GameGlobals.Instance.MouseHighlight.transform.localScale.y * 0.5f), 0.0f);
		}
		else
		{
			GameGlobals.Instance.MouseHighlight.SetActive(false);
		}
	}
}
