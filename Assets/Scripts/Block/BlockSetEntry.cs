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

	void Awake()
	{
		if (BlockInfo != null)
		{
			BlockBuildSlots = BlockInfo.BuildSlots;
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

	public bool CanBeBuilt(GridInfo.BuildSlots eBuildSlot)
	{
		if (BlockUnlocked)
		{
			if (CurrencyManager.Instance.CurrencyAvailable(BlockCost))
			{
				// Check to see if that block can be built on the proposed slot.
				if (BlockBuildSlots.Contains(eBuildSlot))
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
