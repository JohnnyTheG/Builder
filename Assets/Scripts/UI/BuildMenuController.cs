using UnityEngine;
using System.Collections;

public class BuildMenuController : Singleton<BuildMenuController>
{
	public CanvasGroupAnimator CanvasGroupAnimator;

	public BuildMenuBlockInfo BuildMenuBlockInfo;

	int m_nBlockSetIndex = 0;

	BlockManager.Types m_eCurrentType = BlockManager.Types.Wall;

	public void Initialise()
	{
		BuildMenuBlockInfo.SetBlockInfo(BlockManager.Instance.GetBlockInfo(m_eCurrentType, ref m_nBlockSetIndex, 0).BlockInfo);
	}

	public void NextBlock()
	{
		BuildMenuBlockInfo.SetBlockInfo(BlockManager.Instance.GetBlockInfo(m_eCurrentType, ref m_nBlockSetIndex, 1).BlockInfo);
	}

	public void PreviousBlock()
	{
		BuildMenuBlockInfo.SetBlockInfo(BlockManager.Instance.GetBlockInfo(m_eCurrentType, ref m_nBlockSetIndex, -1).BlockInfo);
	}

	public void SetBlockBuildType()
	{
		BlockManager.Instance.CurrentBlockSetEntry = BlockManager.Instance.GetBlockInfo(m_eCurrentType, ref m_nBlockSetIndex, 0);
		Application.Instance.TrySetMode(Application.Mode.Build);
	}
}
