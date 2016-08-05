﻿using UnityEngine;
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

	public void Move(GridInfo cGridInfo)
	{
		if (m_cGridInfo != null)
		{
			m_cGridInfo.Occupied = false;
		}

		m_cGridInfo = cGridInfo;
		m_cGridInfo.Occupied = true;

		transform.position = m_cGridInfo.transform.position + new Vector3(0.0f, (Height * 0.5f) + (cGridInfo.Height * 0.5f), 0.0f);
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
