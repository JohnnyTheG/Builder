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

	public bool SelectHeld()
	{
		return MouseInput.Instance.LeftMouseHeld;
	}

	public bool SelectReleased()
	{
		return MouseInput.Instance.LeftMouseUp;
	}

	public bool Delete()
	{
		return MouseInput.Instance.RightMouseDown || KeyboardInput.Instance.KeyDown(KeyCode.Delete);
	}

	public bool DeleteHeld()
	{
		return MouseInput.Instance.RightMouseHeld;
	}

	public bool DeleteReleased()
	{
		return MouseInput.Instance.RightMouseUp;
	}


	public bool Cancel()
	{
		return KeyboardInput.Instance.KeyDown(KeyCode.Escape);
    }

	public bool RotateAnticlockwise()
	{
		return KeyboardInput.Instance.KeyDown(KeyCode.A);
	}

	public bool RotateClockwise()
	{
		return KeyboardInput.Instance.KeyDown(KeyCode.D) || KeyboardInput.Instance.KeyDown(KeyCode.R);
	}

	public bool Focus()
	{
		return KeyboardInput.Instance.KeyDown(KeyCode.F);
    }

	public bool NextBlockSetEntryCategory()
	{
		return KeyboardInput.Instance.KeyDown(KeyCode.Tab);
	}

	public bool PreviousBlockSetEntryCategory()
	{
		//return KeyboardInput.Instance.KeyDown(KeyCode.DownArrow);

		// Removing this to test tabbing through categories.
		return false;
	}

	public bool NextBlockSetEntry()
	{
		return KeyboardInput.Instance.KeyDown(KeyCode.Q);
    }

	public bool PreviousBlockSetEntry()
	{
		return KeyboardInput.Instance.KeyDown(KeyCode.E);
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

	public bool FlipGrid()
	{
		return KeyboardInput.Instance.KeyDown(KeyCode.Z);
	}

#endif
}
