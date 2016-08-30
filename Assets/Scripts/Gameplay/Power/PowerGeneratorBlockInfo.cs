using UnityEngine;
using System.Collections;

public class PowerGeneratorBlockInfo : BlockInfo
{
	public int PowerOutput = 100;

	public override void Initialise(bool bIsGhost)
	{
		base.Initialise(bIsGhost);

		if (!m_bIsGhost)
		{
			PowerManager.Instance.AddPowerOutput(PowerOutput);
		}
	}

	public override void DestroyBlockInfo(bool bDestroyOpposite)
	{
		base.DestroyBlockInfo(bDestroyOpposite);

		if (!m_bIsGhost)
		{
			PowerManager.Instance.RemovePowerOutput(PowerOutput);
		}
	}
}
