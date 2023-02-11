using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class RangeToolTip : MonoBehaviour
{
	public bool indicatorIsActive = false;
	float range;

	public int resolution = 500;
	public Material rangeIndicatoMaterial;

	private Ellipse rangeIndicator;
	private LineRenderer line;
	private Vector2 axisVector;

	private void Start()
	{
		range = GetComponentInParent<SphereCollider>().radius;
		axisVector = new Vector2(range, range);
		SetupLine();
		CalculateRange();
		rangeIndicator.DrawEllipse();
	}

	private void Update()
	{
		if(!indicatorIsActive)
		{
			line.enabled = false;
			Physics.SyncTransforms();
		}
		else
		{
			line.enabled = true;
			CalculateRange();
			rangeIndicator.DrawEllipse();
		}

		indicatorIsActive = false;
	}

	private void SetupLine()
	{
		line = gameObject.AddComponent<LineRenderer>();
		rangeIndicator = gameObject.AddComponent<Ellipse>();
		line.material = rangeIndicatoMaterial;
		line.sortingOrder = -1;

		line.startColor = Color.blue;
		line.endColor = Color.blue;

		AnimationCurve widthCurve = new AnimationCurve();
		widthCurve.AddKey(0f, .1f);
		widthCurve.AddKey(1f, .1f);
		line.widthCurve = widthCurve;
	}

	private void CalculateRange()
	{
		rangeIndicator.axisVector = axisVector;
		rangeIndicator.center = new Vector2(transform.position.x, transform.position.z);
		rangeIndicator.resolution = resolution;
		rangeIndicator.RecalculateEllipse();
	}
}
