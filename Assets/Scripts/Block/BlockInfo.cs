﻿using UnityEngine;
using System;

public class BlockInfo : MonoBehaviour
{
	// Attach to any spawned block.

	public string Name = "";

	public BlockManager.Category Type;

	// Default to edging.
	public GridInfo.BuildSlots[] BuildSlots = new GridInfo.BuildSlots[4] { GridInfo.BuildSlots.North, GridInfo.BuildSlots.East, GridInfo.BuildSlots.South, GridInfo.BuildSlots.West };

	public float Height = 1.0f;
	public float Width = 1.0f;

	[NonSerialized]
	[HideInInspector]
	public GridInfo m_cGridInfo;
	GridInfo.BuildSlots m_eBuildSlot;
	[NonSerialized]
	[HideInInspector]
	public GridInfo.BuildLayer m_eBuildLayer;

	protected bool m_bIsGhost = false;

	MeshRenderer m_cMeshRenderer;

	Color m_cOriginalColor;

	public virtual void Awake()
	{
		m_cMeshRenderer = GetComponent<MeshRenderer>();
		m_cOriginalColor = m_cMeshRenderer.material.color;
	}

	public virtual void Initialise(bool bIsGhost)
	{
		m_bIsGhost = bIsGhost;

		if (m_bIsGhost)
		{
			m_cMeshRenderer.material = GameGlobals.Instance.GhostMaterial;

			Collider[] acColliders = GetComponents<Collider>();

			for (int nCollider = 0; nCollider < acColliders.Length; nCollider++)
			{
				Destroy(acColliders[nCollider]);
			}
		}
		else
		{
			BlockManager.Instance.RegisterBlock(this);
		}
	}

	public void Move(GridInfo cGridInfo, GridInfo.BuildSlots eBuildSlot, GridInfo.BuildLayer eBuildLayer)
	{
		if (!m_bIsGhost)
		{
			if (m_cGridInfo != null)
			{
				m_cGridInfo.SetUnoccupied(eBuildSlot, eBuildLayer);
			}
		}

		m_cGridInfo = cGridInfo;
		m_eBuildSlot = eBuildSlot;
		m_eBuildLayer = eBuildLayer;

		if (!m_bIsGhost)
		{
			m_cGridInfo.SetOccupied(eBuildSlot, eBuildLayer, this);
			
		}

		Vector3 vecPosition = Vector3.zero;

		switch (eBuildLayer)
		{
			case GridInfo.BuildLayer.Top:

				vecPosition = cGridInfo.TopBuildTarget.transform.position;

				break;

			case GridInfo.BuildLayer.Bottom:

				vecPosition = cGridInfo.BottomBuildTarget.transform.position;

				break;
		}

		transform.position = vecPosition;
	}

	public void Rotate(Vector3 vecAngle)
	{
		Vector3 vecRotation = transform.rotation.eulerAngles;

		vecRotation += vecAngle;

		transform.rotation = Quaternion.Euler(vecRotation);
	}

	public virtual void Destroy()
	{
		if (m_cGridInfo != null)
		{
			m_cGridInfo.SetUnoccupied(m_eBuildSlot, m_eBuildLayer);
		}

		if (!m_bIsGhost)
		{
			BlockManager.Instance.DeregisterBlock(this);
		}

		Destroy(gameObject);
	}

	public virtual void Selected()
	{
		// Do stuff for selection here.
		m_cMeshRenderer.material.SetColor("_Color", GameGlobals.Instance.SelectedBlockColor);
    }

	public virtual void Deselected()
	{
		// Do stuff for deselection here.
		m_cMeshRenderer.material.SetColor("_Color", m_cOriginalColor);
	}

	public bool IsCentreOnly()
	{
		if (BuildSlots.Length == 1 && BuildSlots[0] == GridInfo.BuildSlots.Centre)
		{
			return true;
		}

		return false;
	}
}
