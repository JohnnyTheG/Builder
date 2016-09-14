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

	public class CornerInfo
	{
		public GridInfo.BuildSlot m_eLeftCornerBuildSlot;
		public GridInfo.BuildSlot m_eRightCornerBuildSlot;

		public CornerInfo()
		{
			m_eLeftCornerBuildSlot = GridInfo.BuildSlot.Centre;
			m_eRightCornerBuildSlot = GridInfo.BuildSlot.Centre;
		}

		public CornerInfo(GridInfo.BuildSlot eLeftCornerBuildSlot, GridInfo.BuildSlot eRightCornerBuildSlot)
		{
			m_eLeftCornerBuildSlot = eLeftCornerBuildSlot;
			m_eRightCornerBuildSlot = eRightCornerBuildSlot;
		}
	}

	public static CornerInfo GetCornerInfo(GridInfo.BuildSlot eBuildSlotA, GridInfo.BuildSlot eBuildSlotB)
	{
		//Debug.Log("GridUtilities: GetCornerBuildSlot for " + eBuildSlotA.ToString() + " and " + eBuildSlotB.ToString());

		switch (eBuildSlotA)
		{
			case GridInfo.BuildSlot.North:

				switch (eBuildSlotB)
				{
					case GridInfo.BuildSlot.East:

						//return GridInfo.BuildSlot.East;

						return new CornerInfo(GridInfo.BuildSlot.East, GridInfo.BuildSlot.North);

					case GridInfo.BuildSlot.West:

						return new CornerInfo(GridInfo.BuildSlot.North, GridInfo.BuildSlot.West);
				}

				break;

			case GridInfo.BuildSlot.East:

				switch (eBuildSlotB)
				{
					case GridInfo.BuildSlot.North:

						return new CornerInfo(GridInfo.BuildSlot.East, GridInfo.BuildSlot.North);

					case GridInfo.BuildSlot.South:

						return new CornerInfo(GridInfo.BuildSlot.South, GridInfo.BuildSlot.East);
				}

				break;

			case GridInfo.BuildSlot.South:

				switch (eBuildSlotB)
				{
					case GridInfo.BuildSlot.East:

						return new CornerInfo(GridInfo.BuildSlot.South, GridInfo.BuildSlot.East);

					case GridInfo.BuildSlot.West:

						return new CornerInfo(GridInfo.BuildSlot.West, GridInfo.BuildSlot.South);
				}

				break;

			case GridInfo.BuildSlot.West:

				switch (eBuildSlotB)
				{
					case GridInfo.BuildSlot.North:

						return new CornerInfo(GridInfo.BuildSlot.North, GridInfo.BuildSlot.West);

					case GridInfo.BuildSlot.South:

						return new CornerInfo(GridInfo.BuildSlot.West, GridInfo.BuildSlot.South);
				}

				break;
		}

		return new CornerInfo();
	}

	public static GridInfo.BuildSlot GetOppositeBuildSlot(GridInfo.BuildSlot eBuildSlot)
	{
		switch (eBuildSlot)
		{
			case GridInfo.BuildSlot.North:

				return GridInfo.BuildSlot.South;

			case GridInfo.BuildSlot.East:

				return GridInfo.BuildSlot.West;

			case GridInfo.BuildSlot.South:

				return GridInfo.BuildSlot.North;

			case GridInfo.BuildSlot.West:

				return GridInfo.BuildSlot.East;

			case GridInfo.BuildSlot.Centre:

				return GridInfo.BuildSlot.Centre;

			default:

				return GridInfo.BuildSlot.Undefined;
		}
	}
}
