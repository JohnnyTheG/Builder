using UnityEngine;
using System.Collections;

public class CameraController : Singleton<CameraController>
{
	[SerializeField]
	GameObject Focus;

	[SerializeField]
	float RotationSpeedMultiplier = 1.0f;

	[SerializeField]
	[Tooltip("Mouse scroll wheel input is in the range of 0 to around 0.5 depending how hard it is scrolled.")]
	float ZoomSpeedMultiplier = 350.0f;

	[SerializeField]
	float PanningSpeedMultiplier = 0.5f;

	[SerializeField]
	[Tooltip("Duration of lerp towards focus point.")]
	float FocusDuration = 1.0f;

	[SerializeField]
	[Tooltip("Multiplier for the distance the camera finishes from the target when focusing.")]
    float FocusDistance = 5.0f;

	// Current lerp time when focusing.
	float m_fFocusTime = 0.0f;

	// Start position when focusing.
	Vector3 m_vecFocusStartPosition = Vector3.zero;

	// The position which is being focused upon.
	Vector3 m_vecFocusPoint = Vector3.zero;

	enum State
	{
		Free,
		Focusing,
	}

	State m_eState = State.Free;

	void Start()
	{
		MouseInput.Instance.OnLeftMouseHeld += OnLeftMouseHeld;
		MouseInput.Instance.OnMiddleMouseHeld += OnMiddleMouseHeld;
	}

	void OnLeftMouseHeld()
	{
		if (m_eState == State.Free)
		{
			if (KeyboardInput.Instance.KeyHeld(KeyCode.LeftAlt))
			{
				transform.RotateAround(Focus.transform.position, Vector3.up, MouseInput.Instance.MouseX() * RotationSpeedMultiplier);
				transform.RotateAround(Focus.transform.position, transform.right, MouseInput.Instance.MouseY(true) * RotationSpeedMultiplier);
			}
		}
	}

	void OnMiddleMouseHeld()
	{
		if (m_eState == State.Free)
		{
			// Flip the movement deltas to negative as mouse movement is "reversed".
			Vector3 vecMoveDelta = Vector3.zero;

			vecMoveDelta += transform.forward * -MouseInput.Instance.MouseY() * PanningSpeedMultiplier;
			vecMoveDelta += transform.right * -MouseInput.Instance.MouseX() * PanningSpeedMultiplier;

			// Dont want movement on Y.
			vecMoveDelta.y = 0.0f;

			transform.position += vecMoveDelta;
			Focus.transform.position += vecMoveDelta;
		}
	}

	void Update()
	{
		switch (m_eState)
		{
			case State.Free:

				Vector3 vecPositionBeforeMoveTowards = transform.position;

				float fMouseScrollWheel = MouseInput.Instance.MouseScrollWheel();

				transform.position = Vector3.MoveTowards(transform.position, Focus.transform.position, fMouseScrollWheel * ZoomSpeedMultiplier * Time.deltaTime);

				// This is primitive. Improve so that the limit is a fixed position.
				// Currently it depends on position before rolling mouse wheel.
				if ((transform.position - Focus.transform.position).sqrMagnitude < 1.0f)
				{
					transform.position = vecPositionBeforeMoveTowards;
				}

				break;

			case State.Focusing:

				m_fFocusTime += Time.deltaTime;

				if (m_fFocusTime < FocusDuration)
				{
					transform.position = Vector3.Lerp(m_vecFocusStartPosition, m_vecFocusPoint, m_fFocusTime / FocusDuration);
				}
				else
				{
					transform.position = m_vecFocusPoint;

					m_eState = State.Free;
				}

				break;
		}
	}

	public void SetFocus(Vector3 vecFocusPosition)
	{
		if (m_eState == State.Free)
		{
			m_eState = State.Focusing;

			m_fFocusTime = 0.0f;

			Focus.transform.position = vecFocusPosition;

			m_vecFocusStartPosition = transform.position;

			m_vecFocusPoint = Focus.transform.position - (transform.forward * FocusDistance);
		}
	}
}
