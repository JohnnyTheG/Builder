using UnityEngine;
using System;
using System.Collections.Generic;

public class BlockSet : MonoBehaviour
{
	public Dictionary<BlockInfo.Types, List<BlockInfo>> m_dictBlocks = new Dictionary<BlockInfo.Types, List<BlockInfo>>();

	void Awake()
	{
		// Get the types of blocks.
		BlockInfo.Types[] aeBlockInfoTypes = (BlockInfo.Types[])Enum.GetValues(typeof(BlockInfo.Types));

		// Add all the types to the dictionary.
		for (int nType = 0; nType < aeBlockInfoTypes.Length; nType++)
		{
			if (!m_dictBlocks.ContainsKey(aeBlockInfoTypes[nType]))
			{
				m_dictBlocks.Add(aeBlockInfoTypes[nType], new List<BlockInfo>());
			}
		}

		// Get all the set entries on the set gameobject.
		BlockSetEntry[] acBlockSetEntries = GetComponents<BlockSetEntry>();

		// For each entry, find the type and add it to the correct category in the dictionary.
		for (int nBlockSetEntry = 0; nBlockSetEntry < acBlockSetEntries.Length; nBlockSetEntry++)
		{
			BlockInfo cBlockInfo = acBlockSetEntries[nBlockSetEntry].BlockInfo;

            if (m_dictBlocks.ContainsKey(cBlockInfo.Type))
			{
				m_dictBlocks[cBlockInfo.Type].Add(cBlockInfo);
			}
		}
	}
}
