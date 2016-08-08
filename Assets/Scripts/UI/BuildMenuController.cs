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
		BuildMenuBlockInfo.SetBlockInfo(BlockManager.Instance.GetBlockInfo(m_eCurrentType, ref m_nBlockSetIndex, 0));
	}

	public void NextBlock()
	{
		BuildMenuBlockInfo.SetBlockInfo(BlockManager.Instance.GetBlockInfo(m_eCurrentType, ref m_nBlockSetIndex, 1));
	}

	public void PreviousBlock()
	{
		BuildMenuBlockInfo.SetBlockInfo(BlockManager.Instance.GetBlockInfo(m_eCurrentType, ref m_nBlockSetIndex, -1));
	}

	public void SetBlockBuildType()
	{
		BlockManager.Instance.BlockBuildType = BlockManager.Instance.GetBlockInfo(m_eCurrentType, ref m_nBlockSetIndex, 0).gameObject;
		Application.Instance.TrySetMode(Application.Mode.Build);
	}
}
