using UnityEngine;
using System.Collections;

public class AtmosphereGeneratorBlockInfo : BlockInfo
{
	public override void Initialise(bool bIsGhost)
	{
		base.Initialise(bIsGhost);

		if (!m_bIsGhost)
		{
			// Add atmosphere generation here.
		}
	}
}
