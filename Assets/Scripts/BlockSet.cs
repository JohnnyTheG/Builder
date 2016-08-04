using UnityEngine;
using System.Collections;

public class BlockSet : Singleton<BlockSet>
{
	public BlockInfo[] WallBlocks;

	public BlockInfo GetBlockInfo(BlockInfo[] acBlocks, ref int nIndex, int nIncrement)
	{
		return Utils.GetArrayEntry(acBlocks, ref nIndex, nIncrement);
	}
}
