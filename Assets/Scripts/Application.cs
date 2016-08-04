﻿using UnityEngine;
using System;
using System.Collections.Generic;

public class Application : Singleton<Application>
{
	public enum Mode
	{
		Undefined,
		Build,
		BuildEdit,
		BuildMenu,
	}

	Mode m_eMode = Mode.Undefined;
	Mode m_eNextMode = Mode.Undefined;

	BaseMode m_cMode = null;

	[SerializeField]
	GameObject MouseHighlight;

	readonly Dictionary<Mode, Type> m_dictModes = new Dictionary<Mode, Type>()
	{
		{Mode.Build, typeof(BuildMode)},
		{Mode.BuildEdit, typeof(BuildEditMode)},
		{Mode.BuildMenu, typeof(BuildMenuMode)},
	};

	void SetMode(Mode eMode)
	{
		if (m_eMode == eMode)
		{
			Debug.Log("Application: Mode is already " + m_eMode.ToString());

			return;
		}

		m_eNextMode = eMode;

		State = States.ChangingMode;

		if (m_cMode != null)
		{
			m_cMode.Shutdown(OnModeShutdownComplete);
		}
		else
		{
			// No mode exists, so go straight to the new mode.
			OnModeShutdownComplete();
		}
	}


	public void TrySetMode(Mode eMode)
	{
		if (State == States.RunningMode)
		{
			SetMode(eMode);
		}
	}

	void OnModeShutdownComplete()
	{
		m_eMode = m_eNextMode;
		m_eNextMode = Mode.Undefined;

		State = States.RunningMode;

		if (m_cMode != null)
		{
			Destroy(m_cMode.gameObject);
		}

		if (m_dictModes.ContainsKey(m_eMode))
		{
			GameObject cMode = new GameObject(m_eMode.ToString() + "Mode");
			m_cMode = (BaseMode)cMode.AddComponent(m_dictModes[m_eMode]);
			m_cMode.transform.parent = transform;
		}
	}

	public enum States
	{
		RunningMode,
		ChangingMode,
	}

	public States State
	{
		get;

		private set;
	}

	new void Awake()
	{
		base.Awake();

		// Start in build mode.
		SetMode(Mode.Build);
	}

	public void UpdateMouseHighlight()
	{
		RaycastHit cRaycastHit;

		Ray cRay = Camera.main.ScreenPointToRay(Input.mousePosition);

		if (Physics.Raycast(cRay, out cRaycastHit, Mathf.Infinity, PhysicsLayers.GetPhysicsLayerMask(PhysicsLayers.Grid)))
		{
			GridInfo cGridInfo = cRaycastHit.collider.GetComponent<GridInfo>();

			MouseHighlight.SetActive(true);
			MouseHighlight.transform.position = cRaycastHit.collider.transform.position + new Vector3(0.0f, (cGridInfo.Height * 0.5f) + (MouseHighlight.transform.localScale.y * 0.5f), 0.0f);
		}
		else
		{
			MouseHighlight.SetActive(false);
		}
	}

	void OnGUI()
	{
		GUI.Label(new Rect(0.0f, 0.0f, 300.0f, 20.0f), "Mode: " + m_eMode.ToString());
		GUI.Label(new Rect(0.0f, 20.0f, 300.0f, 20.0f), "SelectedBlock: " + (SelectedBlock != null ? SelectedBlock.name : "None"));
	}

	public GameObject BlockBuildType;

	BlockInfo SelectedBlock
	{
		get;

		set;
	}


	public BlockInfo GetSelectedBlock()
	{
		return SelectedBlock;
	}

	public void SetSelectedBlock(BlockInfo cBlockInfo)
	{
		if (SelectedBlock != null)
		{
			SelectedBlock.Deselected();
		}

		SelectedBlock = cBlockInfo;

		if (SelectedBlock != null)
		{
			SelectedBlock.Selected();
		}
	}
}