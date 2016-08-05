using UnityEngine;
using System;
using System.Collections.Generic;

public class BlockManager : Singleton<BlockManager>
{
	public enum Types
	{
		Wall,
		Floor,
		Prop,
	}

	public Dictionary<Types, List<BlockInfo>> m_dictBlocks = new Dictionary<Types, List<BlockInfo>>();

	Types[] m_aeTypes;

	new void Awake()
	{
		base.Awake();

		// Get the types of blocks.
		m_aeTypes = (Types[])Enum.GetValues(typeof(Types));

		// Add all the types to the dictionary.
		for (int nType = 0; nType < m_aeTypes.Length; nType++)
		{
			if (!m_dictBlocks.ContainsKey(m_aeTypes[nType]))
			{
				m_dictBlocks.Add(m_aeTypes[nType], new List<BlockInfo>());
			}
		}
	}

	public void RegisterBlockSet(BlockSet cBlockSet)
	{
		for (int nType = 0; nType < m_aeTypes.Length; nType++)
		{
			Types eType = m_aeTypes[nType];

            if (m_dictBlocks.ContainsKey(eType) && cBlockSet.m_dictBlockSet.ContainsKey(eType))
			{
				m_dictBlocks[eType].AddRange(cBlockSet.m_dictBlockSet[eType]);
			}
		}
    }

	public BlockInfo GetBlockInfo(BlockManager.Types eType, ref int nIndex, int nIncrement)
	{
		if (m_dictBlocks.ContainsKey(eType))
		{
			return Utils.GetArrayEntry<BlockInfo>(m_dictBlocks[eType].ToArray(), ref nIndex, nIncrement);
		}

		return null;
	}
}
