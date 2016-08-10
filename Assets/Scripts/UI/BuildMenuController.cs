using UnityEngine;
using System.Collections;

public class BuildMenuController : Singleton<BuildMenuController>
{
	public CanvasGroupAnimator CanvasGroupAnimator;

	public BuildMenuBlockInfo BuildMenuBlockInfo;

	public void Initialise()
	{
		BuildMenuBlockInfo.SetBlockSetEntryUI(BlockManager.Instance.GetCurrentBlock());
	}

	public void NextBlock()
	{
		BuildMenuBlockInfo.SetBlockSetEntryUI(BlockManager.Instance.GetNextBlock());
	}

	public void PreviousBlock()
	{
		BuildMenuBlockInfo.SetBlockSetEntryUI(BlockManager.Instance.GetPreviousBlock());
	}

	public void SetBlockBuildType()
	{
		BlockManager.Instance.CurrentBlockSetEntry = BlockManager.Instance.GetCurrentBlock();
		Application.Instance.TrySetMode(Application.Mode.Build);
	}
}
