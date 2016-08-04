using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class EditorCoroutine
{
	public static EditorCoroutine Start(IEnumerator _routine)
	{
		EditorCoroutine cCoroutine = new EditorCoroutine(_routine);
		cCoroutine.Start();
		return cCoroutine;
	}

	readonly IEnumerator m_cRoutine;
	EditorCoroutine(IEnumerator cRoutine)
	{
		m_cRoutine = cRoutine;
	}

	void Start()
	{
		EditorApplication.update += Update;
	}
	public void Stop()
	{
		EditorApplication.update -= Update;
	}

	void Update()
	{
		if (!m_cRoutine.MoveNext())
		{
			Stop();
		}
	}
}