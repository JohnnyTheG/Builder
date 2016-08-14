using UnityEngine;
using System.Collections;

public class Singleton<T> : MonoBehaviour where T : Component
{
	static T m_cInstance;

	public static T Instance
	{
		get
		{
			return m_cInstance;
		}
	}

	public void Awake()
	{
		if (m_cInstance != null)
		{
			Destroy(this);

			return;
		}

		m_cInstance = this as T;
	}
}
