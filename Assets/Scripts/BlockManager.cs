﻿using UnityEngine;
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

	public Dictionary<Types, List<BlockSetEntry>> m_dictBlocks = new Dictionary<Types, List<BlockSetEntry>>();

	Types[] m_aeTypes;

	public bool Initialised
	{
		get;

		private set;
	}

	int m_nCurrentBlockSetEntryIndex = 0;
	Types m_eCurrentBlockSetEntryType = Types.Wall;

	BlockSetEntry CurrentBlockSetEntry = null;

	public BlockSetEntry GetCurrentBlockSetEntry()
	{
		return CurrentBlockSetEntry;
	}

	public BlockSetEntry GetNextBlock(bool bSetCurrentBlockSetEntry)
	{
		BlockSetEntry cBlockSetEntry = GetBlockSetEntry(m_eCurrentBlockSetEntryType, ref m_nCurrentBlockSetEntryIndex, 1);

		if (bSetCurrentBlockSetEntry)
		{
			CurrentBlockSetEntry = cBlockSetEntry;
		}

		return cBlockSetEntry;
	}

	public BlockSetEntry GetPreviousBlock(bool bSetCurrentBlockSetEntry)
	{
		BlockSetEntry cBlockSetEntry = GetBlockSetEntry(m_eCurrentBlockSetEntryType, ref m_nCurrentBlockSetEntryIndex, -1);

		if (bSetCurrentBlockSetEntry)
		{
			CurrentBlockSetEntry = cBlockSetEntry;
		}

		return cBlockSetEntry;
	}

	public BlockSetEntry GetCurrentBlock()
	{
		BlockSetEntry cBlockSetEntry = GetBlockSetEntry(m_eCurrentBlockSetEntryType, ref m_nCurrentBlockSetEntryIndex, 0);

		return cBlockSetEntry;
	}

	public void SetCurrentBlock()
	{
		CurrentBlockSetEntry = GetBlockSetEntry(m_eCurrentBlockSetEntryType, ref m_nCurrentBlockSetEntryIndex, 0);
	}

	public Types GetNextBlockSetType(bool bSetCurrentBlockSetEntry)
	{
		int nCurrentIndex = (int)m_eCurrentBlockSetEntryType + 1;

		if (nCurrentIndex > m_aeTypes.Length - 1)
		{
			nCurrentIndex = 0;
		}

		m_eCurrentBlockSetEntryType = (Types)nCurrentIndex;

		if (bSetCurrentBlockSetEntry)
		{
			SetCurrentBlock();
		}

		return m_aeTypes[nCurrentIndex];
	}

	public Types GetPreviousBlockSetType(bool bSetCurrentBlockSetEntry)
	{
		int nCurrentIndex = (int)m_eCurrentBlockSetEntryType - 1;

		if (nCurrentIndex < 0)
		{
			nCurrentIndex = m_aeTypes.Length - 1;
		}

		m_eCurrentBlockSetEntryType = (Types)nCurrentIndex;

		if (bSetCurrentBlockSetEntry)
		{
			SetCurrentBlock();
		}

		return m_aeTypes[nCurrentIndex];
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
				m_dictBlocks.Add(m_aeTypes[nType], new List<BlockSetEntry>());
			}
		}
	}

	IEnumerator Start()
	{
		while (CurrentBlockSetEntry == null)
		{
			SetInitialBlockSetEntry();

			yield return new WaitForEndOfFrame();
		}

		Initialised = true;
	}

	public void RegisterBlockSetEntry(BlockSet cBlockSet)
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

	public void SetInitialBlockSetEntry()
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
						CurrentBlockSetEntry = m_dictBlocks[m_aeTypes[nType]][nBlock];

						return;
					}
				}
			}
		}
	}

	public BlockSetEntry GetBlockSetEntry(Types eType, ref int nIndex, int nIncrement)
	{
		if (m_dictBlocks.ContainsKey(eType))
		{
			if (m_dictBlocks[eType].Count > 0)
			{
				return Utils.GetArrayEntry(m_dictBlocks[eType].ToArray(), ref nIndex, nIncrement);
			}
		}

		return null;
	}

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
