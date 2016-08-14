using UnityEngine;
using System.Collections;

public class PowerGeneratorBlockInfo : BlockInfo
{
	public int PowerOutput = 100;

	public override void Initialise(bool bIsGhost)
	{
		base.Initialise(bIsGhost);

		if (!bIsGhost)
		{
			PowerManager.Instance.AddPowerOutput(PowerOutput);
		}
	}

	public override void Destroy()
	{
		base.Destroy();

		PowerManager.Instance.RemovePowerOutput(PowerOutput);
	}
}
