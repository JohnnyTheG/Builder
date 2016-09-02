using UnityEngine;
using System.Collections.Generic;

public class DayNightCycleSun : MonoBehaviour
{
	[SerializeField]
	[Tooltip("Centre that the sun revolves around.")]
	GameObject FocusPoint = null;

	[SerializeField]
	[Tooltip("Duration of a full revolution around the FocusPoint in seconds.")]
	float RevolutionDuration = 10.0f;

	float m_fRevolutionEndTime = 0.0f;

	float m_fRevolutionAngle = 360.0f;

	float m_fStageSegmentAdjustment = 0.0f;

	enum Stage
	{
		Midnight,
		Dawn,
		Midday,
		Dusk,
	}

	void Awake()
	{
		m_fStageSegmentAdjustment = (m_fRevolutionAngle / 8.0f) / m_fRevolutionAngle;

		// Get the start angle. We assume that vector3.up is "Midday".
		float fAngle = Angle(Vector3.up, (transform.position - FocusPoint.transform.position).normalized, Vector3.right);

		// Sort the time as if it had started and run on to that position.
		m_fRevolutionEndTime = Time.time + (RevolutionDuration - (fAngle / m_fRevolutionAngle) * RevolutionDuration);
	}

	public static float Angle(Vector3 v1, Vector3 v2, Vector3 n)
	{
		float fAngle = Mathf.Atan2(Vector3.Dot(n, Vector3.Cross(v1, v2)), Vector3.Dot(v1, v2)) * Mathf.Rad2Deg;

		if (fAngle < 0.0f)
		{
			fAngle = 360.0f + fAngle;
		}

		return fAngle;
	}

	void Update()
	{
		if (Time.time >= m_fRevolutionEndTime)
		{
			SetRevolutionEndTime();
		}

		Debug.Log(GetStage().ToString());

		transform.RotateAround(FocusPoint.transform.position, Vector3.right, (Time.deltaTime / RevolutionDuration) * m_fRevolutionAngle);
	}

	void SetRevolutionEndTime()
	{
		m_fRevolutionEndTime = Time.time + RevolutionDuration;
	}

	Stage GetStage()
	{
		float fPercentage = (RevolutionDuration - (m_fRevolutionEndTime - Time.time)) / RevolutionDuration;

		if (fPercentage < 0.25f - m_fStageSegmentAdjustment)
		{
			return Stage.Midday;
		}
		else if (fPercentage < 0.5f - m_fStageSegmentAdjustment)
		{
			return Stage.Dusk;
		}
		else if (fPercentage < 0.75f - m_fStageSegmentAdjustment)
		{
			return Stage.Midnight;
		}
		else if (fPercentage < 1.0f - m_fStageSegmentAdjustment)
		{
			return Stage.Dawn;
		}
		else
		{
			return Stage.Midday;
		}
	}
}
