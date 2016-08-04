using UnityEngine;
using System.Collections;

public class BuildMenuController : Singleton<BuildMenuController>
{
	public CanvasGroupAnimator CanvasGroupAnimator;

	public BuildMenuBlockInfo BuildMenuBlockInfo;

	BlockSet m_cBlockSet;

	int m_nBlockSetIndex = 0;

	void Awake()
	{
		m_cBlockSet = BlockSet.Instance;
	}

	public void Initialise()
	{
		if (m_cBlockSet.WallBlocks[m_nBlockSetIndex] != null)
		{
			BuildMenuBlockInfo.SetBlockInfo(m_cBlockSet.WallBlocks[m_nBlockSetIndex]);
		}
	}

	public void NextBlock()
	{
		BuildMenuBlockInfo.SetBlockInfo(m_cBlockSet.GetBlockInfo(m_cBlockSet.WallBlocks, ref m_nBlockSetIndex, 1));
	}

	public void PreviousBlock()
	{
		BuildMenuBlockInfo.SetBlockInfo(m_cBlockSet.GetBlockInfo(m_cBlockSet.WallBlocks, ref m_nBlockSetIndex, -1));
    }

	public void SetBlockBuildType()
	{
		Application.Instance.BlockBuildType = m_cBlockSet.WallBlocks[m_nBlockSetIndex].gameObject;
		Application.Instance.TrySetMode(Application.Mode.Build);
	}
}
