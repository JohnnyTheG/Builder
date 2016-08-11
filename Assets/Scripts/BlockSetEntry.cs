using UnityEngine;
using System.Collections;

public class BlockSetEntry : MonoBehaviour
{
	public BlockInfo BlockInfo;

	public int BlockCost;

	[SerializeField]
	bool BlockUnlocked = true;

	public void Unlock()
	{
		BlockUnlocked = true;
	}

	public bool IsUnlocked()
	{
		return BlockUnlocked;
	}
}
