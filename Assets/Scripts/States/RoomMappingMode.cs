using UnityEngine;
using System.Collections;

public class RoomMappingMode : BaseMode
{
	GridInfo m_cGridInfoStart;
	GridInfo m_cGridInfoFinish;

	GridInfo[] m_acCurrentGridSelection;
	GridInfo[] m_acSelectHeldCurrentGridSelection;

	enum Mode
	{
		Mapping,
		Unmapping,
	}

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
					StartGridSelection(out cRaycastHit);
				}

				if (InputActions.Instance.SelectHeld())
				{
					TickGridSelection(out cRaycastHit, Mode.Mapping);
				}

				if (InputActions.Instance.SelectReleased())
				{
					if (FinishGridSelection(Mode.Mapping))
					{
						MapCurrentGridSelection();
					}
				}

				if (InputActions.Instance.Delete())
				{
					StartGridSelection(out cRaycastHit);
				}

				if (InputActions.Instance.DeleteHeld())
				{
					TickGridSelection(out cRaycastHit, Mode.Unmapping);
				}

				if (InputActions.Instance.DeleteReleased())
				{
					if (FinishGridSelection(Mode.Unmapping))
					{
						UnmapCurrentGridSelection();
					}
				}
			}
			else
			{
				if (InputActions.Instance.SelectReleased() || InputActions.Instance.Cancel())
				{
					ClearCurrentGridSelection();
				}
			}
		}
	}	

	void StartGridSelection(out RaycastHit cRaycastHit)
	{
		// If a new selection starts, clear the old selection.
		ClearCurrentGridSelection();

		if (RaycastForGrid(out cRaycastHit))
		{
			m_cGridInfoStart = cRaycastHit.collider.GetComponent<GridInfo>();
		}
	}

	void TickGridSelection(out RaycastHit cRaycastHit, Mode eMode)
	{
		if (RaycastForGrid(out cRaycastHit))
		{
			m_cGridInfoFinish = cRaycastHit.collider.GetComponent<GridInfo>();

			GetCurrentGridSelection(eMode);
		}
		else
		{
			// Hit nothing, so no grid selection.
			m_cGridInfoFinish = null;

			ClearCurrentGridSelection();
		}
	}

	bool FinishGridSelection(Mode eMode)
	{
		if (m_cGridInfoStart != null && m_cGridInfoFinish != null)
		{
			GetCurrentGridSelection(eMode);

			// The operation is over now. So set the actual selection.
			m_acCurrentGridSelection = m_acSelectHeldCurrentGridSelection;

			m_acSelectHeldCurrentGridSelection = null;

			return true;
		}

		return false;
	}

	void GetCurrentGridSelection(Mode eMode)
	{
		// Unhighlight any currently selected block.
		if (m_acSelectHeldCurrentGridSelection != null)
		{
			for (int nGridInfo = 0; nGridInfo < m_acSelectHeldCurrentGridSelection.Length; nGridInfo++)
			{
				DehighlightGridInfo(m_acSelectHeldCurrentGridSelection[nGridInfo]);
            }
		}

		m_acSelectHeldCurrentGridSelection = GridSettings.Instance.GetGridSelection(m_cGridInfoStart, m_cGridInfoFinish);

		switch (eMode)
		{
			case Mode.Mapping:

				GetCurrentMappingGridSelection();

				break;

			case Mode.Unmapping:

				GetCurrentUnmappingGridSelection();

				break;
		}
	}

	void DehighlightGridInfo(GridInfo cGridInfo)
	{
		if (!cGridInfo.InRoom)
		{
			cGridInfo.Dehighlight();
		}
		else
		{
			cGridInfo.MappedHighlight();
		}
	}

	void GetCurrentMappingGridSelection()
	{
		for (int nGridInfo = 0; nGridInfo < m_acSelectHeldCurrentGridSelection.Length; nGridInfo++)
		{
			m_acSelectHeldCurrentGridSelection[nGridInfo].MappingHighlight();
		}
	}

	void GetCurrentUnmappingGridSelection()
	{
		for (int nGridInfo = 0; nGridInfo < m_acSelectHeldCurrentGridSelection.Length; nGridInfo++)
		{
			m_acSelectHeldCurrentGridSelection[nGridInfo].UnmappingHighlight();
		}
	}

	bool RaycastForGrid(out RaycastHit cRaycastHit)
	{
		Ray cRay = Camera.main.ScreenPointToRay(Input.mousePosition);

		return Physics.Raycast(cRay, out cRaycastHit, Mathf.Infinity, PhysicsLayers.GetPhysicsLayerMask(PhysicsLayers.Grid));
	}

	void ClearCurrentGridSelection(bool bDehighlight = true)
	{
		if (m_acCurrentGridSelection != null)
		{
			if (bDehighlight)
			{
				for (int nGridInfo = 0; nGridInfo < m_acCurrentGridSelection.Length; nGridInfo++)
				{
					DehighlightGridInfo(m_acCurrentGridSelection[nGridInfo]);
				}
			}

			m_acCurrentGridSelection = null;
		}

		if (m_acSelectHeldCurrentGridSelection != null)
		{
			if (bDehighlight)
			{
				for (int nGridInfo = 0; nGridInfo < m_acSelectHeldCurrentGridSelection.Length; nGridInfo++)
				{
					GridInfo cGridInfo = m_acSelectHeldCurrentGridSelection[nGridInfo];

					DehighlightGridInfo(cGridInfo);
				}
			}

			m_acSelectHeldCurrentGridSelection = null;
		}
	}

	void MapCurrentGridSelection()
	{
		if (m_acCurrentGridSelection != null)
		{
			for (int nGridInfo = 0; nGridInfo < m_acCurrentGridSelection.Length; nGridInfo++)
			{
				m_acCurrentGridSelection[nGridInfo].MappedHighlight();
			}

			RoomManager.Instance.RegisterRoom(m_acCurrentGridSelection);

			ClearCurrentGridSelection(false);
		}
	}

	void UnmapCurrentGridSelection()
	{
		if (m_acCurrentGridSelection != null)
		{
			RoomManager.Instance.DeregisterRoom(m_acCurrentGridSelection);

			for (int nGridInfo = 0; nGridInfo < m_acCurrentGridSelection.Length; nGridInfo++)
			{
				DehighlightGridInfo(m_acCurrentGridSelection[nGridInfo]);
			}

			ClearCurrentGridSelection(false);
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
