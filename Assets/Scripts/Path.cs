using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Path : MonoBehaviour
{
	public int resolution = 500;
	private Vector3[] positions;
	public GameObject target;
	LineRenderer lr;

	private void Start()
	{
		lr = gameObject.GetComponent<LineRenderer>();
		lr.positionCount = resolution + 1;
	}

	public void DrawPath()
	{
		CalculatePath();

		for(int i = 0; i <= resolution; i++)
		{
			lr.SetPosition(i, positions[i]);
		}
	}

	void CalculatePath()
	{
		positions = new Vector3[resolution + 1];

		for(int i = 0; i <= resolution; i++)
		{
			positions[i] = Vector3.Lerp(transform.position, target.transform.position, i / resolution);
		}
	}
}
