using UnityEngine;
using System.Collections;

public class AtmosphereGeneratorBlockInfo : BlockInfo
{
	public override void Initialise(GridInfo cGridInfo, GridInfo.BuildSlot eBuildSlot, GridInfo.BuildLayer eBuildLayer, bool bIsGhost)
	{
		base.Initialise(cGridInfo, eBuildSlot, eBuildLayer, bIsGhost);

		if (!m_bIsGhost)
		{
			// Add atmosphere generation here.
		}
	}
}
