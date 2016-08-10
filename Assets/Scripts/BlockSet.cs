using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class BlockSet : MonoBehaviour
{
	public Dictionary<BlockManager.Types, List<BlockSetEntry>> m_dictBlockSet = new Dictionary<BlockManager.Types, List<BlockSetEntry>>();

	void Awake()
	{
		// Get the types of blocks.
		BlockManager.Types[] aeBlockInfoTypes = (BlockManager.Types[])Enum.GetValues(typeof(BlockManager.Types));

		// Add all the types to the dictionary.
		for (int nType = 0; nType < aeBlockInfoTypes.Length; nType++)
		{
			if (!m_dictBlockSet.ContainsKey(aeBlockInfoTypes[nType]))
			{
				m_dictBlockSet.Add(aeBlockInfoTypes[nType], new List<BlockSetEntry>());
			}
		}

		// Get all the set entries on the set gameobject.
		BlockSetEntry[] acBlockSetEntries = GetComponents<BlockSetEntry>();

		// For each entry, find the type and add it to the correct category in the dictionary.
		for (int nBlockSetEntry = 0; nBlockSetEntry < acBlockSetEntries.Length; nBlockSetEntry++)
		{
			BlockInfo cBlockInfo = acBlockSetEntries[nBlockSetEntry].BlockInfo;

            if (m_dictBlockSet.ContainsKey(cBlockInfo.Type))
			{
				m_dictBlockSet[cBlockInfo.Type].Add(acBlockSetEntries[nBlockSetEntry]);
			}
		}
	}

	IEnumerator Start()
	{
		while (BlockManager.Instance == null)
		{
			yield return new WaitForEndOfFrame();
		}

		// Register with the block manager.
		BlockManager.Instance.RegisterBlockSetEntry(this);
	}
}
