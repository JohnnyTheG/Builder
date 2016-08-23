using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class BlockManager : Singleton<BlockManager>
{
	public enum Category
	{
		Wall,
		Floor,
		Door,
		Power,
	}

	public Dictionary<Category, List<BlockSetEntry>> m_dictBlocks = new Dictionary<Category, List<BlockSetEntry>>();

	Category[] m_aeCategories;

	public bool Initialised
	{
		get;

		private set;
	}

	List<BlockInfo> m_lstBlocks = new List<BlockInfo>();

	public void RegisterBlock(BlockInfo cBlockInfo)
	{
		m_lstBlocks.Add(cBlockInfo);
	}

	public void DeregisterBlock(BlockInfo cBlockInfo)
	{
		m_lstBlocks.Remove(cBlockInfo);
	}

	int m_nCurrentBlockSetEntryIndex = 0;
	Category m_eCurrentBlockSetEntryCategory = Category.Wall;

	BlockSetEntry CurrentBlockSetEntry = null;

	public BlockSetEntry GetCurrentBlockSetEntry()
	{
		return CurrentBlockSetEntry;
	}

	public BlockSetEntry GetNextBlock(bool bSetCurrentBlockSetEntry)
	{
		BlockSetEntry cBlockSetEntry = GetBlockSetEntry(m_eCurrentBlockSetEntryCategory, ref m_nCurrentBlockSetEntryIndex, 1);

		if (bSetCurrentBlockSetEntry)
		{
			CurrentBlockSetEntry = cBlockSetEntry;
		}

		return cBlockSetEntry;
	}

	public BlockSetEntry GetPreviousBlock(bool bSetCurrentBlockSetEntry)
	{
		BlockSetEntry cBlockSetEntry = GetBlockSetEntry(m_eCurrentBlockSetEntryCategory, ref m_nCurrentBlockSetEntryIndex, -1);

		if (bSetCurrentBlockSetEntry)
		{
			CurrentBlockSetEntry = cBlockSetEntry;
		}

		return cBlockSetEntry;
	}

	public BlockSetEntry GetCurrentBlock()
	{
		BlockSetEntry cBlockSetEntry = GetBlockSetEntry(m_eCurrentBlockSetEntryCategory, ref m_nCurrentBlockSetEntryIndex, 0);

		return cBlockSetEntry;
	}

	public void SetCurrentBlock()
	{
		CurrentBlockSetEntry = GetBlockSetEntry(m_eCurrentBlockSetEntryCategory, ref m_nCurrentBlockSetEntryIndex, 0);
	}

	public Category GetNextBlockSetEntryCategory(bool bSetCurrentBlockSetEntry)
	{
		int nCurrentIndex = (int)m_eCurrentBlockSetEntryCategory + 1;

		if (nCurrentIndex > m_aeCategories.Length - 1)
		{
			nCurrentIndex = 0;
		}

		m_eCurrentBlockSetEntryCategory = (Category)nCurrentIndex;

		if (bSetCurrentBlockSetEntry)
		{
			SetCurrentBlock();
		}

		return m_aeCategories[nCurrentIndex];
	}

	public Category GetPreviousBlockSetEntryCategory(bool bSetCurrentBlockSetEntry)
	{
		int nCurrentIndex = (int)m_eCurrentBlockSetEntryCategory - 1;

		if (nCurrentIndex < 0)
		{
			nCurrentIndex = m_aeCategories.Length - 1;
		}

		m_eCurrentBlockSetEntryCategory = (Category)nCurrentIndex;

		if (bSetCurrentBlockSetEntry)
		{
			SetCurrentBlock();
		}

		return m_aeCategories[nCurrentIndex];
	}

	new void Awake()
	{
		base.Awake();

		// Get the types of blocks.
		m_aeCategories = (Category[])Enum.GetValues(typeof(Category));

		// Add all the types to the dictionary.
		for (int nCategory = 0; nCategory < m_aeCategories.Length; nCategory++)
		{
			if (!m_dictBlocks.ContainsKey(m_aeCategories[nCategory]))
			{
				m_dictBlocks.Add(m_aeCategories[nCategory], new List<BlockSetEntry>());
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

	public void RegisterBlockSet(BlockSet cBlockSet)
	{
		for (int nCategory = 0; nCategory < m_aeCategories.Length; nCategory++)
		{
			Category eCategory = m_aeCategories[nCategory];

			if (m_dictBlocks.ContainsKey(eCategory) && cBlockSet.m_dictBlockSet.ContainsKey(eCategory))
			{
				m_dictBlocks[eCategory].AddRange(cBlockSet.m_dictBlockSet[eCategory]);
			}
		}
	}

	public void SetInitialBlockSetEntry()
	{
		// Add all the types to the dictionary.
		for (int nCategory = 0; nCategory < m_aeCategories.Length; nCategory++)
		{
			if (m_dictBlocks.ContainsKey(m_aeCategories[nCategory]))
			{
				for (int nBlock = 0; nBlock < m_dictBlocks[m_aeCategories[nCategory]].Count; nBlock++)
				{
					if (m_dictBlocks[m_aeCategories[nCategory]][nBlock] != null)
					{
						CurrentBlockSetEntry = m_dictBlocks[m_aeCategories[nCategory]][nBlock];
						m_eCurrentBlockSetEntryCategory = m_aeCategories[nCategory];
						m_nCurrentBlockSetEntryIndex = nBlock;

						return;
					}
				}
			}
		}
	}

	public BlockSetEntry GetBlockSetEntry(Category eCategory, ref int nIndex, int nIncrement)
	{
		if (m_dictBlocks.ContainsKey(eCategory))
		{
			if (m_dictBlocks[eCategory].Count > 0)
			{
				return Utils.GetArrayEntry(m_dictBlocks[eCategory].ToArray(), ref nIndex, nIncrement);
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
