using UnityEngine;
using System.Collections;

public class PowerCapacitorBlockInfo : BlockInfo
{
	public int PowerCapacity = 100;

	public override void Initialise(bool bIsGhost)
	{
		base.Initialise(bIsGhost);

		if (!bIsGhost)
		{
			PowerManager.Instance.AddPowerCapacity(PowerCapacity);
		}
	}

	public override void Destroy()
	{
		base.Destroy();

		PowerManager.Instance.RemovePowerCapacity(PowerCapacity);
	}
}
