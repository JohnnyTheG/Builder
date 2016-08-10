using UnityEngine;
using System;

public class BlockInfo : MonoBehaviour
{
	// Attach to any spawned block.

	public string Name = "";

	public BlockManager.Types Type;

	public float Height = 1.0f;
	public float Width = 1.0f;

	[NonSerialized]
	[HideInInspector]
	public GridInfo m_cGridInfo;

	bool m_bIsGhost = false;

	public void Initialise(bool bIsGhost)
	{
		m_bIsGhost = bIsGhost;

		if (m_bIsGhost)
		{
			GetComponent<MeshRenderer>().material = GameGlobals.Instance.GhostMaterial;

			Collider[] acColliders = GetComponents<Collider>();

			for (int nCollider = 0; nCollider < acColliders.Length; nCollider++)
			{
				Destroy(acColliders[nCollider]);
			}
		}
	}

	public void Move(GridInfo cGridInfo)
	{
		if (m_cGridInfo != null)
		{
			m_cGridInfo.Occupied = false;
		}

		m_cGridInfo = cGridInfo;

		if (!m_bIsGhost)
		{
			m_cGridInfo.Occupied = true;
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

	public void Destroy()
	{
		if (m_cGridInfo != null)
		{
			m_cGridInfo.Occupied = false;
		}

		Destroy(gameObject);
	}

	public void Selected()
	{
		// Do stuff for selection here.
	}

	public void Deselected()
	{
		// Do stuff for deselection here.
	}
}
