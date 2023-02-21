using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
	public float panSpeed = 1;
	//Vector3 cameraCelestialOffset = new Vector3();
	Vector3 deltaPosition = new Vector3(0, 0, 0);

	public float minZoomSpeed = 1, maxZoomSpeed = 1;
	public float minHeight, maxHeight;
	float currentZoomSpeed;

	GameController GC;

	private void Start()
	{
		GC = FindObjectOfType<GameController>();
		SetZoomSpeed();
	}

	private void Update()
	{
		transform.position += deltaPosition * panSpeed;
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

		if(scrollValue > 0 && transform.position.y > minHeight)
		{
			transform.position += new Vector3(0, -1, 0) * currentZoomSpeed;
		}
		else if(scrollValue < 0 && transform.position.y < maxHeight)
		{
			transform.position += new Vector3(0, 1, 0) * currentZoomSpeed;
		}

		SetZoomSpeed();
	}

	private void SetZoomSpeed()
	{
		float currentZoomRatio = (transform.position.y - minHeight) / (maxHeight - minHeight);
		currentZoomSpeed = currentZoomRatio * (maxZoomSpeed - minZoomSpeed) + minZoomSpeed;
	}
}
