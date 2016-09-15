using UnityEngine;
using System.Collections;

public class PowerCapacitorBlockInfo : BlockInfo
{
	public int PowerCapacity = 100;

	public override void Initialise(GridInfo cGridInfo, GridInfo.BuildSlot eBuildSlot, GridInfo.BuildLayer eBuildLayer, bool bIsGhost)
	{
		base.Initialise(cGridInfo, eBuildSlot, eBuildLayer, bIsGhost);

		if (!m_bIsGhost)
		{
			PowerManager.Instance.AddPowerCapacity(PowerCapacity);
		}
	}

	public override void DestroyBlockInfo(bool bDestroyOpposite)
	{
		base.DestroyBlockInfo(bDestroyOpposite);

		if (!m_bIsGhost)
		{
			PowerManager.Instance.RemovePowerCapacity(PowerCapacity);
		}
	}
}
