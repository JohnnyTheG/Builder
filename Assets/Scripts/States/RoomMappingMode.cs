using UnityEngine;
using System.Collections;

public class RoomMappingMode : BaseMode
{
	GridInfo m_cGridInfoStart;
	GridInfo m_cGridInfoFinish;

	public override void Shutdown(OnShutdownCompleteCallback OnShutdownComplete)
	{
		base.Shutdown(OnShutdownComplete);

		InvokeOnShutdownComplete();
	}

	public void Start()
	{
		// Clear any selected block
		SetSelectedBlock(null);
	}

	public void Update()
	{
		if (KeyboardInput.Instance.KeyDown(KeyCode.Tab))
		{
			Application.Instance.TrySetMode(Application.Mode.Build);
		}

		if (!InputActions.Instance.RotateCamera())
		{
			RaycastHit cRaycastHit;

			if (InputActions.Instance.Select())
			{
				Debug.Log("RoomMappingMode: Select Pressed");

				if (RaycastForGrid(out cRaycastHit))
				{
					m_cGridInfoStart = cRaycastHit.collider.GetComponent<GridInfo>();
				}
			}

			if (InputActions.Instance.SelectHeld())
			{
				Debug.Log("RoomMappingMode: Select Held");

				if (RaycastForGrid(out cRaycastHit))
				{
					m_cGridInfoFinish = cRaycastHit.collider.GetComponent<GridInfo>();
				}
			}

			if (InputActions.Instance.SelectReleased())
			{
				Debug.Log("RoomMappingMode: Select Released");

				if (m_cGridInfoStart != null && m_cGridInfoFinish != null)
				{
					GridInfo[] acGridSelection = GridSettings.Instance.GetGridSelection(m_cGridInfoStart, m_cGridInfoFinish);

					Debug.Log("RoomMappingMode: Selection contained " + acGridSelection.Length + " grid squares.");
				}
			}
		}
	}

	bool RaycastForGrid(out RaycastHit cRaycastHit)
	{
		Ray cRay = Camera.main.ScreenPointToRay(Input.mousePosition);

		return Physics.Raycast(cRay, out cRaycastHit, Mathf.Infinity, PhysicsLayers.GetPhysicsLayerMask(PhysicsLayers.Grid));
	}
}
