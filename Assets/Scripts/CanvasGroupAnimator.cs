using UnityEngine;
using System.Collections;

public class CanvasGroupAnimator : MonoBehaviour
{
	public delegate void OnFadedOutCallback();
	public delegate void OnFadedInCallback();

	public event OnFadedOutCallback OnFadedOut;
	public event OnFadedInCallback OnFadedIn;

	enum State
	{
		Visible,
		Invisible,
	}

	State m_eState = State.Invisible;

	enum FadeState
	{
		FadeIn,
		FadeOut,
		FadedIn,
		FadedOut,
	}

	FadeState m_eFadeState = FadeState.FadeOut;

	CanvasGroup m_cCanvasGroup;

	[SerializeField]
	[Tooltip("Duration of fade in/out")]
	float m_fFadeDuration = 1.0f;

	[SerializeField]
	[Tooltip("Minimum Alpha")]
	float m_fMinAlpha = 0.0f;

	[SerializeField]
	[Tooltip("Maximum Alpha")]
	float m_fMaxAlpha = 1.0f;

	float m_fFadeTime = 0.0f;

	[SerializeField]
	bool m_bPulse = true;

	void Awake()
	{
		m_cCanvasGroup = GetComponent<CanvasGroup>();

		// Start invisible.
		m_cCanvasGroup.alpha = 0.0f;
	}

	public void Show()
	{
		m_eState = State.Visible;
	}

	public void Hide()
	{
		m_eState = State.Invisible;
	}

	public void FadeIn()
	{
		if (!m_bPulse)
		{
			if (m_eState == State.Visible)
			{
				m_eFadeState = FadeState.FadeIn;
			}
		}
	}

	public void FadeOut()
	{
		if (!m_bPulse)
		{
			if (m_eState == State.Visible)
			{
				m_eFadeState = FadeState.FadeOut;
			}
		}
	}

	public bool FadingIn
	{
		get
		{
			return m_eState == State.Visible && m_eFadeState == FadeState.FadeIn;
		}
	}

	public bool FadingOut
	{
		get
		{
			return m_eState == State.Visible && m_eFadeState == FadeState.FadeOut;
		}
	}

	void Update()
	{
		switch (m_eState)
		{
			case State.Visible:

				switch (m_eFadeState)
				{
					case FadeState.FadeIn:

						m_fFadeTime += Time.unscaledDeltaTime;

						if (m_fFadeTime >= m_fFadeDuration)
						{
							m_fFadeTime = m_fFadeDuration;

							if (m_bPulse)
							{
								// If pulsing change now.
								m_eFadeState = FadeState.FadeOut;
							}
							else
							{
								m_eFadeState = FadeState.FadedIn;

								if (OnFadedIn != null)
								{
									OnFadedIn();
								}
							}
						}

						break;

					case FadeState.FadeOut:

						m_fFadeTime -= Time.unscaledDeltaTime;

						if (m_fFadeTime <= 0.0f)
						{
							m_fFadeTime = 0.0f;

							if (m_bPulse)
							{
								// If pulsing change now.
								m_eFadeState = FadeState.FadeIn;
							}
							else
							{
								m_eFadeState = FadeState.FadedOut;

								if (OnFadedOut != null)
								{
									OnFadedOut();
								}
							}
						}

						break;
				}

				m_cCanvasGroup.alpha = Mathf.Lerp(m_fMinAlpha, m_fMaxAlpha, m_fFadeTime / m_fFadeDuration);

				break;

			case State.Invisible:

				m_cCanvasGroup.alpha = 0.0f;

				m_fFadeTime = 0.0f;

				m_eFadeState = FadeState.FadeOut;

				break;
		}
	}
}
