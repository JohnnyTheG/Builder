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

	public static bool RaycastForGridFromMouse(out RaycastHit cRaycastHit)
	{
		Ray cRay = Camera.main.ScreenPointToRay(Input.mousePosition);

		return Physics.Raycast(cRay, out cRaycastHit, Mathf.Infinity, PhysicsLayers.GetPhysicsLayerMask(PhysicsLayers.Grid));
	}

	public static GridInfo.BuildSlot GetCornerBuildSlot(GridInfo.BuildSlot eBuildSlotA, GridInfo.BuildSlot eBuildSlotB)
	{
		Debug.Log("GridUtilities: GetCornerBuildSlot for " + eBuildSlotA.ToString() + " and " + eBuildSlotB.ToString());

		switch (eBuildSlotA)
		{
			case GridInfo.BuildSlot.North:

				switch (eBuildSlotB)
				{
					case GridInfo.BuildSlot.East:

						return GridInfo.BuildSlot.East;

					case GridInfo.BuildSlot.West:

						return GridInfo.BuildSlot.North;
				}

				break;

			case GridInfo.BuildSlot.East:

				switch (eBuildSlotB)
				{
					case GridInfo.BuildSlot.North:

						return GridInfo.BuildSlot.East;

					case GridInfo.BuildSlot.South:

						return GridInfo.BuildSlot.South;
				}

				break;

			case GridInfo.BuildSlot.South:

				switch (eBuildSlotB)
				{
					case GridInfo.BuildSlot.East:

						return GridInfo.BuildSlot.South;

					case GridInfo.BuildSlot.West:

						return GridInfo.BuildSlot.West;
				}

				break;

			case GridInfo.BuildSlot.West:

				switch (eBuildSlotB)
				{
					case GridInfo.BuildSlot.North:

						return GridInfo.BuildSlot.North;

					case GridInfo.BuildSlot.South:

						return GridInfo.BuildSlot.West;
				}

				break;
		}

		return GridInfo.BuildSlot.Centre;
	}
}
