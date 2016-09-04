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

	[SerializeField]
	AnimationCurve SunLightIntensity;

	float m_fRevolutionEndTime = 0.0f;

	const float m_fRevolutionAngle = 360.0f;

	float m_fPercentageSegmentAdjustment = 0.0f;

	Light m_cSunLight;

	enum Stage
	{
		Midnight,
		Dawn,
		Midday,
		Dusk,
	}

	void Awake()
	{
		m_cSunLight = GetComponent<Light>();

		m_fPercentageSegmentAdjustment = (m_fRevolutionAngle / 8.0f) / m_fRevolutionAngle;

		// Get the start angle. We assume that vector3.up is "Midday".
		float fAngle = GetRevolutionAngle();

		// Sort the time as if it had started and run on to that position.
		m_fRevolutionEndTime = Time.time + (RevolutionDuration - (fAngle / m_fRevolutionAngle) * RevolutionDuration);
	}

	float GetRevolutionAngle()
	{
		return MathUtils.Angle(Vector3.up, (transform.position - FocusPoint.transform.position).normalized, Vector3.right);
	}

	void Update()
	{
		if (Time.time >= m_fRevolutionEndTime)
		{
			SetRevolutionEndTime();
		}

		Debug.Log(GetStage().ToString());

		m_cSunLight.intensity = SunLightIntensity.Evaluate(GetRevolutionPrecentageComplete());

		transform.RotateAround(FocusPoint.transform.position, Vector3.right, (Time.deltaTime / RevolutionDuration) * m_fRevolutionAngle);
		transform.LookAt(FocusPoint.transform, Vector3.up);
	}

	void SetRevolutionEndTime()
	{
		m_fRevolutionEndTime = Time.time + RevolutionDuration;
	}

	Stage GetStage()
	{
		float fPercentage = GetRevolutionPrecentageComplete();

		if (fPercentage < 0.25f - m_fPercentageSegmentAdjustment)
		{
			return Stage.Midday;
		}
		else if (fPercentage < 0.5f - m_fPercentageSegmentAdjustment)
		{
			return Stage.Dusk;
		}
		else if (fPercentage < 0.75f - m_fPercentageSegmentAdjustment)
		{
			return Stage.Midnight;
		}
		else if (fPercentage < 1.0f - m_fPercentageSegmentAdjustment)
		{
			return Stage.Dawn;
		}
		else
		{
			return Stage.Midday;
		}
	}

	float GetRevolutionPrecentageComplete()
	{
		float fPercentage = (RevolutionDuration - (m_fRevolutionEndTime - Time.time)) / RevolutionDuration;

		// Subtract the segement adjustment to shift the quarters on the clock face.
		return fPercentage;
	}
}
