using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class BlockManager : Singleton<BlockManager>
{
	public enum Types
	{
		Wall,
		Floor,
		Door,
	}

	public Dictionary<Types, List<BlockInfo>> m_dictBlocks = new Dictionary<Types, List<BlockInfo>>();

	Types[] m_aeTypes;

	public bool Initialised
	{
		get;

		private set;
	}

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

	IEnumerator Start()
	{
		while (BlockBuildType == null)
		{
			SetInitialBlock();

			yield return new WaitForEndOfFrame();
		}

		Initialised = true;
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

	public void SetInitialBlock()
	{
		// Add all the types to the dictionary.
		for (int nType = 0; nType < m_aeTypes.Length; nType++)
		{
			if (m_dictBlocks.ContainsKey(m_aeTypes[nType]))
			{
				for (int nBlock = 0; nBlock < m_dictBlocks[m_aeTypes[nType]].Count; nBlock++)
				{
					if (m_dictBlocks[m_aeTypes[nType]][nBlock] != null)
					{
						BlockBuildType = m_dictBlocks[m_aeTypes[nType]][nBlock].gameObject;

						return;
					}
				}
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

	public GameObject BlockBuildType = null;

	BlockInfo SelectedBlock
	{
		get;

		set;
	}

	public BlockInfo GetSelectedBlock()
	{
		return SelectedBlock;
	}

	public void SetSelectedBlock(BlockInfo cBlockInfo)
	{
		if (SelectedBlock != null)
		{
			SelectedBlock.Deselected();
		}

		SelectedBlock = cBlockInfo;

		if (SelectedBlock != null)
		{
			SelectedBlock.Selected();
		}
	}
}
