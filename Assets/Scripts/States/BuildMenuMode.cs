using UnityEngine;
using System.Collections;
using System;

public class BuildMenuMode : BaseMode
{
	BuildMenuController m_cBuildMenuController;

	public override void Shutdown(OnShutdownCompleteCallback OnShutdownComplete)
	{
		base.Shutdown(OnShutdownComplete);

		// When fade completes, InvokeOnShutdownComplete.
		m_cBuildMenuController.CanvasGroupAnimator.FadeOut();
		m_cBuildMenuController.CanvasGroupAnimator.OnFadedOut += OnFadedOut;
	}

	void OnFadedOut()
	{
		// Remove callback at this point as this is what is called from the event.
		m_cBuildMenuController.CanvasGroupAnimator.OnFadedOut -= OnFadedOut;
		m_cBuildMenuController.gameObject.SetActive(false);
		InvokeOnShutdownComplete();
	}

	void Awake()
	{
		m_cBuildMenuController = GameGlobals.Instance.BuildMenuController;
		m_cBuildMenuController.gameObject.SetActive(true);
	}

	void Start()
	{
		m_cBuildMenuController.CanvasGroupAnimator.Show();
		m_cBuildMenuController.CanvasGroupAnimator.FadeIn();

		m_cBuildMenuController.Initialise();
	}

	void Update()
	{
		if (KeyboardInput.Instance.KeyDown(KeyCode.Escape))
		{
			Application.Instance.TrySetMode(Application.Mode.Build);
		}
	}

	public void Close()
	{
		Application.Instance.TrySetMode(Application.Mode.Build);
	}

	void OnDestroy()
	{
	}
}
