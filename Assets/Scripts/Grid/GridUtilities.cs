using UnityEngine;
using System.Collections;

public class GridUtilities : MonoBehaviour
{
	public static GridInfo GetGridInfoFromCollider(Collider cCollider)
	{
		return cCollider.transform.parent.GetComponent<GridInfo>();
	}

	public static GridLayer GetGridLayerFromCollider(Collider cCollider)
	{
		return cCollider.GetComponent<GridLayer>();
	}

	public static GridInfo.BuildLayer GetOppositeBuildLayer(GridInfo.BuildLayer eBuildLayer)
	{
		switch (eBuildLayer)
		{
			case GridInfo.BuildLayer.Bottom:

				return GridInfo.BuildLayer.Top;

			case GridInfo.BuildLayer.Top:
			default:

				return GridInfo.BuildLayer.Bottom;
		}
	}
}
