using UnityEngine;
using System.Collections;

public class PhysicsLayers
{
	public static int Grid = 8;
	public static int Block = 9;

	public static int GetPhysicsLayerMask(params int[] anPhysicsLayers)
	{
		int nResultantMask = 1 << anPhysicsLayers[0];

		if (anPhysicsLayers.Length >= 2)
		{
			for (int nPhysicsMask = 1; nPhysicsMask < anPhysicsLayers.Length; nPhysicsMask++)
			{
				nResultantMask |= (1 << anPhysicsLayers[nPhysicsMask]);
			}
		}

		return nResultantMask;
	}
}
