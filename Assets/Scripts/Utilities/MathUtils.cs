using UnityEngine;
using System.Collections;

public class MathUtils : MonoBehaviour
{
	public static float Angle(Vector3 v1, Vector3 v2, Vector3 n)
	{
		float fAngle = Mathf.Atan2(Vector3.Dot(n, Vector3.Cross(v1, v2)), Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;

		if (fAngle < 0.0f)
		{
			fAngle = 360.0f + fAngle;
		}

		return fAngle;
	}
}
