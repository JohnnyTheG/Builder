﻿using UnityEngine;
using System.Collections;

public class BuildMenuController : Singleton<BuildMenuController>
{
	public CanvasGroupAnimator CanvasGroupAnimator;

	public BuildMenuBlockInfo BuildMenuBlockInfo;

	public void Initialise()
	{
		BuildMenuBlockInfo.SetBlockSetEntryUI(BlockManager.Instance.GetCurrentBlock(false));
	}

	public void NextBlock()
	{
		BuildMenuBlockInfo.SetBlockSetEntryUI(BlockManager.Instance.GetNextBlock(false));
	}

	public void PreviousBlock()
	{
		BuildMenuBlockInfo.SetBlockSetEntryUI(BlockManager.Instance.GetPreviousBlock(false));
	}

	public void SetCurrentBlockSetEntry()
	{
		BlockManager.Instance.GetCurrentBlock(true);
		Application.Instance.TrySetMode(Application.Mode.Build);
	}
}
