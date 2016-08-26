using UnityEngine;
using System.Linq;

public class BlockSetEntry : MonoBehaviour
{
	public BlockInfo BlockInfo;

	public int BlockCost;

	[SerializeField]
	bool BlockUnlocked = true;

	[HideInInspector]
	public GridInfo.BuildSlots[] BlockBuildSlots;

	[HideInInspector]
	public GridInfo.BuildLayer[] BlockBuildLayers;

	void Awake()
	{
		if (BlockInfo != null)
		{
			BlockBuildSlots = BlockInfo.BuildSlots;
			BlockBuildLayers = BlockInfo.BuildLayers;
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

	public bool CanBeBuilt(GridInfo.BuildSlots eBuildSlot, GridInfo.BuildLayer eBuildLayer)
	{
		if (BlockUnlocked)
		{
			if (CurrencyManager.Instance.CurrencyAvailable(BlockCost))
			{
				// Check to see if that block can be built on the proposed slot.
				// Also check to see if it can be built on that layer.
				if (BlockBuildSlots.Contains(eBuildSlot) && BlockBuildLayers.Contains(eBuildLayer))
				{
					return true;
				}
			}
		}

		return false;
	}

	public void Build()
	{
		CurrencyManager.Instance.SpendCurrency(BlockCost);
	}

	public bool IsCentreOnly()
	{
		if (BlockBuildSlots.Length == 1 && BlockBuildSlots[0] == GridInfo.BuildSlots.Centre)
		{
			return true;
		}

		return false;
	}
}
