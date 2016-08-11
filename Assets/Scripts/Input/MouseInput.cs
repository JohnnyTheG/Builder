using UnityEngine;
using System.Collections;

public class MouseInput : Singleton<MouseInput>
{
	public bool LeftMouseDown
	{
		get;

		private set;
	}

	public bool LeftMouseUp
	{
		get;

		private set;
	}

	public bool LeftMouseHeld
	{
		get;

		private set;
	}

	public bool RightMouseDown
	{
		get;

		private set;
	}

	public bool RightMouseUp
	{
		get;

		private set;
	}

	public bool RightMouseHeld
	{
		get;

		private set;
	}

	public bool MiddleMouseDown
	{
		get;

		private set;
	}

	public bool MiddleMouseUp
	{
		get;

		private set;
	}

	public bool MiddleMouseHeld
	{
		get;

		private set;
	}

	public delegate void OnLeftMouseDownCallback();
	public delegate void OnRightMouseDownCallback();
	public delegate void OnMiddleMouseDownCallback();

	public event OnLeftMouseDownCallback OnLeftMouseDown;
	public event OnRightMouseDownCallback OnRightMouseDown;
	public event OnMiddleMouseDownCallback OnMiddleMouseDown;

	public delegate void OnLeftMouseUpCallback();
	public delegate void OnRightMouseUpCallback();
	public delegate void OnMiddleMouseUpCallback();

	public event OnLeftMouseUpCallback OnLeftMouseUp;
	public event OnRightMouseUpCallback OnRightMouseUp;
	public event OnMiddleMouseUpCallback OnMiddleMouseUp;

	public delegate void OnLeftMouseHeldCallback();
	public delegate void OnRightMouseHeldCallback();
	public delegate void OnMiddleMouseHeldCallback();

	public event OnLeftMouseHeldCallback OnLeftMouseHeld;
	public event OnRightMouseHeldCallback OnRightMouseHeld;
	public event OnMiddleMouseHeldCallback OnMiddleMouseHeld;

	public float MouseX(bool InvertMouseX = false)
	{
		float fMouseX = Input.GetAxis("Mouse X");

		switch (InvertMouseX)
		{
			case true:

				return -fMouseX;

			default:

				return fMouseX;
		}
	}

	public float MouseY(bool InvertMouseY = false)
	{
		float fMouseY = Input.GetAxis("Mouse Y");

		switch (InvertMouseY)
		{
			case true:

				return -fMouseY;

			default:

				return fMouseY;
		}

	}

	public Vector3 MouseMovement
	{
		get
		{
			return new Vector3(MouseX(), 0.0f, MouseY());
		}
	}

	public float MouseScrollWheel()
	{
		return Input.GetAxis("Mouse ScrollWheel");
	}

	void Update()
	{
		LeftMouseDown = false;
		LeftMouseUp = false;
		LeftMouseHeld = false;

		RightMouseDown = false;
		RightMouseUp = false;
		RightMouseHeld = false;

		MiddleMouseDown = false;
		MiddleMouseUp = false;
		MiddleMouseHeld = false;

		if (Input.GetMouseButtonDown(0))
		{
			LeftMouseDown = true;

			if (OnLeftMouseDown != null)
			{
				OnLeftMouseDown();
			}
		}

		if (Input.GetMouseButtonUp(0))
		{
			LeftMouseUp = true;

			if (OnLeftMouseUp != null)
			{
				OnLeftMouseUp();
			}
		}

		if (Input.GetMouseButton(0))
		{
			LeftMouseHeld = true;

			if (OnLeftMouseHeld != null)
			{
				OnLeftMouseHeld();
			}
		}

		if (Input.GetMouseButtonDown(1))
		{
			RightMouseDown = true;

			if (OnRightMouseDown != null)
			{
				OnRightMouseDown();
			}
		}

		if (Input.GetMouseButtonUp(1))
		{
			RightMouseUp = true;

			if (OnRightMouseUp != null)
			{
				OnRightMouseUp();
			}
		}

		if (Input.GetMouseButton(1))
		{
			RightMouseHeld = true;

			if (OnRightMouseHeld != null)
			{
				OnRightMouseHeld();
			}
		}

		if (Input.GetMouseButtonDown(2))
		{
			MiddleMouseDown = true;

			if (OnMiddleMouseDown != null)
			{
				OnMiddleMouseDown();
			}
		}

		if (Input.GetMouseButtonUp(2))
		{
			MiddleMouseUp = true;

			if (OnMiddleMouseUp != null)
			{
				OnMiddleMouseUp();
			}
		}

		if (Input.GetMouseButton(2))
		{
			MiddleMouseHeld = true;

			if (OnMiddleMouseHeld != null)
			{
				OnMiddleMouseHeld();
			}
		}
	}
}
