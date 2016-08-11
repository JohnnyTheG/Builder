using UnityEngine;
using System.Collections;
using System;

public class InputActions : Singleton<InputActions>
{
#if UNITY_STANDALONE

	public bool Select()
	{
		return MouseInput.Instance.LeftMouseDown;
	}

	public bool Delete()
	{
		return MouseInput.Instance.RightMouseDown;
	}

	public bool Cancel()
	{
		return KeyboardInput.Instance.KeyDown(KeyCode.Escape);
    }

	public bool Rotate()
	{
		return KeyboardInput.Instance.KeyDown(KeyCode.R);
	}

	public bool Focus()
	{
		return KeyboardInput.Instance.KeyDown(KeyCode.F);
    }

	public bool NextBlockSetEntryCategory()
	{
		return KeyboardInput.Instance.KeyDown(KeyCode.UpArrow);
	}

	public bool PreviousBlockSetEntryCategory()
	{
		return KeyboardInput.Instance.KeyDown(KeyCode.DownArrow);
	}

	public bool NextBlockSetEntry()
	{
		return KeyboardInput.Instance.KeyDown(KeyCode.LeftArrow);
    }

	public bool PreviousBlockSetEntry()
	{
		return KeyboardInput.Instance.KeyDown(KeyCode.RightArrow);
	}

	public bool RotateCamera()
	{
		return KeyboardInput.Instance.KeyHeld(KeyCode.LeftAlt);
    }

	public float RotateCameraXAxis()
	{
		return MouseInput.Instance.MouseX();
    }

	public float RotateCameraYAxis()
	{
		return MouseInput.Instance.MouseY(true);
    }

	public float PanCameraXAxis()
	{
		return MouseInput.Instance.MouseX();
    }

	public float PanCameraYAxis()
	{
		return MouseInput.Instance.MouseY();
    }

	public float ZoomCameraAxis()
	{
		return MouseInput.Instance.MouseScrollWheel();
    }

#endif
}
