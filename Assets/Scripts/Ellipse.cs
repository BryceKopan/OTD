using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Ellipse : MonoBehaviour
{
	public Vector2 axisVector;
	public Vector2 center;
	public float startAngle = 0;

	public int resolution = 1000;
	private Vector3[] positions;

	public void DrawEllipse()
	{
		LineRenderer lr = gameObject.GetComponent<LineRenderer>();
		lr.positionCount = resolution + 1;
		for(int i = 0; i <= resolution; i++)
		{
			lr.SetPosition(i, positions[i]);
		}
	}

	public Vector3 GetPositionOnEllipse(float angle)
	{
		angle = angle * Mathf.Deg2Rad;

		if(axisVector.x * Mathf.Cos(angle) + center.x == 0)
			Debug.Log(axisVector.x +":"+ Mathf.Cos(angle) +":"+ center.x);

		return new Vector3(axisVector.x * Mathf.Cos(angle) + center.x, 0.0f, axisVector.y * Mathf.Sin(angle) + center.y);
	}

	public void RecalculateEllipse()
	{
		CreateEllipse(axisVector.x, axisVector.y, center.x, center.y, startAngle, resolution);
	}

	void CreateEllipse(float a, float b, float h, float k, float theta, int resolution)
	{

		positions = new Vector3[resolution + 1];
		Quaternion q = Quaternion.AngleAxis(theta, Vector3.up);
		Vector3 center = new Vector3(h, 0.0f, k);

		for(int i = 0; i <= resolution; i++)
		{
			float angle = (float)i / (float)resolution * 2.0f * Mathf.PI;
			positions[i] = new Vector3(a * Mathf.Cos(angle), 0.0f, b * Mathf.Sin(angle));
			positions[i] = q * positions[i] + center;
		}
	}
}
