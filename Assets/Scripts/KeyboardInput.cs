using UnityEngine;
using System;
using System.Collections.Generic;

public class KeyboardInput : Singleton<KeyboardInput>
{
	public delegate void OnKeyDownCallback();
	public delegate void OnKeyUpCallback();
	public delegate void OnKeyHeld();

	KeyCode[] m_aeKeyCodes;

	class KeyInfo
	{
		public bool KeyDown = false;
		public bool KeyUp = false;
		public bool KeyHeld = false;

		public event OnKeyDownCallback OnKeyDown;
		public event OnKeyUpCallback OnKeyUp;
		public event OnKeyHeld OnKeyHeld;

		public void InvokeOnKeyDown()
		{
			if (OnKeyDown != null)
			{
				OnKeyDown();
			}
		}

		public void InvokeOnKeyUp()
		{
			if (OnKeyUp != null)
			{
				OnKeyUp();
			}
		}

		public void InvokeOnKeyHeld()
		{
			if (OnKeyHeld != null)
			{
				OnKeyHeld();
			}
		}

		public void Reset()
		{
			KeyDown = false;
			KeyUp = false;
			KeyHeld = false;
		}
	}

	Dictionary<KeyCode, KeyInfo> m_dictKeys = new Dictionary<KeyCode, KeyInfo>();

	new void Awake()
	{
		base.Awake();

		m_aeKeyCodes = (KeyCode[])Enum.GetValues(typeof(KeyCode));

        foreach (KeyCode eKeyCode in m_aeKeyCodes)
		{
			if (!m_dictKeys.ContainsKey(eKeyCode))
			{
				m_dictKeys.Add(eKeyCode, new KeyInfo());
			}
		}
	}

	void Update()
	{
		foreach (KeyCode eKeyCode in m_aeKeyCodes)
		{
			if (m_dictKeys.ContainsKey(eKeyCode))
			{
				KeyInfo cKeyInfo = m_dictKeys[eKeyCode];

				cKeyInfo.Reset();

				if (Input.GetKeyDown(eKeyCode))
				{
					cKeyInfo.InvokeOnKeyDown();
					cKeyInfo.KeyDown = true;
                }

				if (Input.GetKeyUp(eKeyCode))
				{
					m_dictKeys[eKeyCode].InvokeOnKeyUp();
					cKeyInfo.KeyUp = true;
				}

				if (Input.GetKey(eKeyCode))
				{
					m_dictKeys[eKeyCode].InvokeOnKeyHeld();
					cKeyInfo.KeyHeld = true;
				}
			}
		}
    }

	public void RegisterKeyCallbacks(KeyCode eKeyCode, OnKeyDownCallback cOnKeyDown = null, OnKeyUpCallback cOnKeyUp = null, OnKeyHeld cOnKeyHeld = null)
	{
		if (m_dictKeys.ContainsKey(eKeyCode))
		{
			if (cOnKeyDown != null)
			{
				m_dictKeys[eKeyCode].OnKeyDown += cOnKeyDown;
			}

			if (cOnKeyUp != null)
			{
				m_dictKeys[eKeyCode].OnKeyUp += cOnKeyUp;
			}

			if (cOnKeyHeld != null)
			{
				m_dictKeys[eKeyCode].OnKeyHeld += cOnKeyHeld;
			}
		}
	}

	public bool KeyDown(KeyCode eKeyCode)
	{
		if (m_dictKeys.ContainsKey(eKeyCode))
		{
			return m_dictKeys[eKeyCode].KeyDown;
		}

		return false;
	}

	public bool KeyUp(KeyCode eKeyCode)
	{
		if (m_dictKeys.ContainsKey(eKeyCode))
		{
			return m_dictKeys[eKeyCode].KeyUp;
		}

		return false;
	}

	public bool KeyHeld(KeyCode eKeyCode)
	{
		if (m_dictKeys.ContainsKey(eKeyCode))
		{
			return m_dictKeys[eKeyCode].KeyHeld;
		}

		return false;
	}
}
