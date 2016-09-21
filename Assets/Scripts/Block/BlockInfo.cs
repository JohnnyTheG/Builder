using UnityEngine;
using System;

public class BlockInfo : MonoBehaviour
{
	// Attach to any spawned block.
	[Space(10)]
	public string Name = "";
	public BlockManager.Category Type;

	[Space(10)]
	// Default to edging.
	// This is set in editor to show the possible build slots for this block.
	public GridInfo.BuildSlot[] BuildSlots = new GridInfo.BuildSlot[4] { GridInfo.BuildSlot.North, GridInfo.BuildSlot.East, GridInfo.BuildSlot.South, GridInfo.BuildSlot.West };

	// Default to both layers.
	// This is set in editor to show the possible build layers for this block.
	public GridInfo.BuildLayer[] BuildLayers = new GridInfo.BuildLayer[2] { GridInfo.BuildLayer.Top, GridInfo.BuildLayer.Bottom };

	[NonSerialized]
	[HideInInspector]
	public GridInfo GridInfo;
	[NonSerialized]
	[HideInInspector]
	public GridInfo.BuildSlot BuildSlot;
	[NonSerialized]
	[HideInInspector]
	public GridInfo.BuildLayer BuildLayer;

	protected bool m_bIsGhost = false;

	MeshRenderer m_cMeshRenderer;

	Color m_cOriginalColor;

	public virtual void Awake()
	{
		m_cMeshRenderer = GetComponent<MeshRenderer>();
		m_cOriginalColor = m_cMeshRenderer.material.color;
	}

	public virtual void Initialise(GridInfo cGridInfo, GridInfo.BuildSlot eBuildSlot, GridInfo.BuildLayer eBuildLayer, bool bIsGhost)
	{
		GridInfo = cGridInfo;

		BuildSlot = eBuildSlot;

		BuildLayer = eBuildLayer;

		m_bIsGhost = bIsGhost;

		if (m_bIsGhost)
		{
			m_cMeshRenderer.material = GameGlobals.Instance.GhostMaterial;

			Collider[] acColliders = GetComponents<Collider>();

			for (int nCollider = 0; nCollider < acColliders.Length; nCollider++)
			{
				acColliders[nCollider].enabled = false;
				Destroy(acColliders[nCollider]);
			}
		}
		else
		{
			BlockManager.Instance.RegisterBlock(this);
		}
	}

	public virtual void DestroyBlockInfo(bool bDestroyOpposite)
	{
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
		if (BuildSlots.Length == 1 && BuildSlots[0] == GridInfo.BuildSlot.Centre)
		{
			return true;
		}

		return false;
	}
}
