using UnityEngine;
using System.Collections;

public class GridUtilities : MonoBehaviour
{
	public static GridInfo GetGridInfoFromCollider(Collider cCollider)
	{
		return cCollider.transform.parent.GetComponent<GridInfo>();
	}
}
