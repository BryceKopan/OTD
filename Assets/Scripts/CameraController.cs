using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
	public float panSpeed = 1;
	Vector3 deltaPosition = new Vector3(0, 0, 0);

	public float zoomSpeed = 1;
	private Vector3 startingPosition;

	GameController GC;

	private void Start()
	{
		startingPosition = transform.position;
		GC = FindObjectOfType<GameController>();
	}

	private void Update()
	{
		if(GC.selectedObject)
		{
			Vector3 selectedPosition = GC.selectedObject.transform.position;
			transform.position = new Vector3(selectedPosition.x, transform.position.y, selectedPosition.z);
		}
		else
			transform.position = transform.position + (deltaPosition * panSpeed);
	}

	public void PanUp(InputAction.CallbackContext context)
	{
		GC.selectedObject = null;
		Vector3 panVector = new Vector3(0, 0, 1);

		if(context.started)
			deltaPosition += panVector;
		else if(context.canceled)
			deltaPosition -= panVector;
	}

	public void PanDown(InputAction.CallbackContext context)
	{
		GC.selectedObject = null;
		Vector3 panVector = new Vector3(0, 0, -1);

		if(context.started)
			deltaPosition += panVector;
		else if(context.canceled)
			deltaPosition -= panVector;
	}

	public void PanLeft(InputAction.CallbackContext context)
	{
		GC.selectedObject = null;
		Vector3 panVector = new Vector3(-1, 0, 0);

		if(context.started)
			deltaPosition += panVector;
		else if(context.canceled)
			deltaPosition -= panVector;
	}

	public void PanRight(InputAction.CallbackContext context)
	{
		GC.selectedObject = null;
		Vector3 panVector = new Vector3(1, 0, 0);

		if(context.started)
			deltaPosition += panVector;
		else if(context.canceled)
			deltaPosition -= panVector;
	}

	public void Zoom(InputAction.CallbackContext context)
	{
		float scrollValue = context.ReadValue<float>();

		if(scrollValue > 0 && transform.position.y > 1)
		{
			transform.position += new Vector3(0, -1, 0) * zoomSpeed;
		}
		else if(scrollValue < 0)
		{
			transform.position += new Vector3(0, 1, 0) * zoomSpeed;
		}
	}
}
