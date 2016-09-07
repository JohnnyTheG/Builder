using UnityEngine;
using System.Linq;

public class BlockSetEntry : MonoBehaviour
{
	public BlockInfo BlockInfo;

	[Space(10)]
	[Header("Block Settings")]
    [SerializeField]
	bool BlockUnlocked = true;

	public int BlockCost;

	[Space(10)]
	[Header("Build Settings")]
    public bool CanDragBuild = false;
	public bool AutomaticCorners = false;
	public BlockSetEntry LeftCorner;
	public BlockSetEntry RightCorner;

	[HideInInspector]
	public GridInfo.BuildSlot[] BlockBuildSlots;

	[HideInInspector]
	public GridInfo.BuildLayer[] BlockBuildLayers;

	public BlockInfo OppositeBlockInfo;

	[HideInInspector]
	public GridInfo.BuildSlot[] OppositeBlockBuildSlots;

	[HideInInspector]
	public GridInfo.BuildLayer[] OppositeBlockBuildLayers;

	void Awake()
	{
		if (BlockInfo != null)
		{
			BlockBuildSlots = BlockInfo.BuildSlots;
			BlockBuildLayers = BlockInfo.BuildLayers;
		}

		if (OppositeBlockInfo != null)
		{
			OppositeBlockBuildSlots = OppositeBlockInfo.BuildSlots;
			OppositeBlockBuildLayers = OppositeBlockInfo.BuildLayers;
		}
	}

	public void Unlock()
	{
		BlockUnlocked = true;
	}

	public bool IsUnlocked()
	{
		return BlockUnlocked;
	}

	public bool CanBeBuilt(GridInfo.BuildSlot eBuildSlot, GridInfo.BuildLayer eBuildLayer)
	{
		bool bCanBeBuilt = false;
		// If there is no opposite block, this goes to true just to allow final return to be true.
		bool bOppositeCanBeBuilt = OppositeBlockInfo == null;

		if (BlockUnlocked)
		{
			if (CurrencyManager.Instance.CurrencyAvailable(BlockCost))
			{
				// Check to see if that block can be built on the proposed slot.
				// Also check to see if it can be built on that layer.
				if (BlockBuildSlots.Contains(eBuildSlot) && BlockBuildLayers.Contains(eBuildLayer))
				{
					bCanBeBuilt = true;
				}

				// If there is an opposite block to be built.
				if (OppositeBlockInfo != null)
				{
					if (OppositeBlockBuildSlots.Contains(eBuildSlot) && OppositeBlockBuildLayers.Contains(GridUtilities.GetOppositeBuildLayer(eBuildLayer)))
					{
						bOppositeCanBeBuilt = true;
					}
				}
			}
		}

		return bCanBeBuilt && bOppositeCanBeBuilt;
	}

	public void Build()
	{
		CurrencyManager.Instance.SpendCurrency(BlockCost);
	}

	public bool IsCentreOnly()
	{
		if (BlockBuildSlots.Length == 1 && BlockBuildSlots[0] == GridInfo.BuildSlot.Centre)
		{
			return true;
		}

		return false;
	}

	public bool HasOppositeBlock()
	{
		return OppositeBlockInfo != null;
	}
}
