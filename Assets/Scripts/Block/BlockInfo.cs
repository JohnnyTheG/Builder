using UnityEngine;
using System;

public class BlockInfo : MonoBehaviour
{
	// Attach to any spawned block.

	public string Name = "";

	public BlockManager.Category Type;

	public float Height = 1.0f;
	public float Width = 1.0f;

	[NonSerialized]
	[HideInInspector]
	public GridInfo m_cGridInfo;

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
	}

	public void Move(GridInfo cGridInfo)
	{
		if (!m_bIsGhost)
		{
			if (m_cGridInfo != null)
			{
				m_cGridInfo.SetUnoccupied();
			}
		}

		m_cGridInfo = cGridInfo;

		if (!m_bIsGhost)
		{
			m_cGridInfo.SetOccupied(this);
			
		}

		// Y is half height of the block plus half height of the floor.
		// If pivot is set correctly at base of the mesh, then the half height isnt needed.
		transform.position = m_cGridInfo.transform.position + new Vector3(0.0f, /*(Height * 0.5f) +*/ (cGridInfo.Height * 0.5f), 0.0f);
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
			m_cGridInfo.SetUnoccupied();
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
}
