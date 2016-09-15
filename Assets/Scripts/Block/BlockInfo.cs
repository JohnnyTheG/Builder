using UnityEngine;
using System;

public class BlockInfo : MonoBehaviour
{
	// Attach to any spawned block.
	[Space(10)]
	public string Name = "";
	public BlockManager.Category Type;
	public bool IsCorner = false;
	[NonSerialized]
	[HideInInspector]
	// If this is a corner, when created this should be set to the corresponding corner piece.
	public BlockInfo PairedCorner = null;

	[Space(10)]
	// Default to edging.
	// This is set in editor to show the possible build slots for this block.
	public GridInfo.BuildSlot[] BuildSlots = new GridInfo.BuildSlot[4] { GridInfo.BuildSlot.North, GridInfo.BuildSlot.East, GridInfo.BuildSlot.South, GridInfo.BuildSlot.West };

	// Default to both layers.
	// This is set in editor to show the possible build layers for this block.
	public GridInfo.BuildLayer[] BuildLayers = new GridInfo.BuildLayer[2] { GridInfo.BuildLayer.Top, GridInfo.BuildLayer.Bottom };

	public float Height = 1.0f;
	public float Width = 1.0f;

	[NonSerialized]
	[HideInInspector]
	public GridInfo GridInfo;
	public GridInfo.BuildSlot BuildSlot;
	[NonSerialized]
	[HideInInspector]
	public GridInfo.BuildLayer BuildLayer;

	[NonSerialized]
	[HideInInspector]
	// The block set entry which owns the prefab which created this.
	public BlockSetEntry BlockSetEntryCreatedFrom;

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
				acColliders[nCollider].enabled = false;
				Destroy(acColliders[nCollider]);
			}
		}
		else
		{
			BlockManager.Instance.RegisterBlock(this);
		}
	}

	public void Move(GridInfo cGridInfo, GridInfo.BuildSlot eBuildSlot, GridInfo.BuildLayer eBuildLayer, bool bMoveOppositeBlock)
	{
		// Clear the previous occupation.
		SetUnoccupation();

		GridInfo = cGridInfo;
		BuildSlot = eBuildSlot;
		BuildLayer = eBuildLayer;

		SetOccupation();

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

		// Attach to the grid info holding it.
		transform.parent = cGridInfo.transform;
	}

	void SetUnoccupation()
	{
		/*if (!m_bIsGhost)
		{
			if (GridInfo != null)
			{
				GridInfo.SetUnoccupied(BuildSlot, BuildLayer);

				GridInfo cTouchingGridInfo = GridSettings.Instance.GetTouchingGridInfo(GridInfo, BuildSlot, BuildLayer);

				if (cTouchingGridInfo != null)
				{
					GridInfo.BuildSlot eTouchingBuildSlot = GridUtilities.GetOppositeBuildSlot(BuildSlot);

					cTouchingGridInfo.SetUnoccupied(eTouchingBuildSlot, BuildLayer);
				}
			}
		}*/
	}

	void SetOccupation()
	{
		/*if (!m_bIsGhost)
		{
			GridInfo.SetOccupied(BuildSlot, BuildLayer, BlockSetEntryCreatedFrom);

			GridInfo cTouchingGridInfo = GridSettings.Instance.GetTouchingGridInfo(GridInfo, BuildSlot, BuildLayer);

			if (cTouchingGridInfo != null)
			{
				GridInfo.BuildSlot eTouchingBuildSlot = GridUtilities.GetOppositeBuildSlot(BuildSlot);

				cTouchingGridInfo.SetOccupied(eTouchingBuildSlot, BuildLayer, BlockSetEntryCreatedFrom);
			}
		}*/
	}

	public void Rotate(Vector3 vecAngle)
	{
		Vector3 vecRotation = transform.rotation.eulerAngles;

		vecRotation += vecAngle;

		transform.rotation = Quaternion.Euler(vecRotation);
	}

	public virtual void DestroyBlockInfo(bool bDestroyOpposite)
	{
		if (!m_bIsGhost)
		{
			SetUnoccupation();

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
