using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
	public bool followOrbit = true;

	//Visual Elements of Orbit
	public Material orbitPathMaterial;
	public Ellipse orbitPath;

	public CelestialBody principle;
	public Vector2 axisVector;
	public float baseSpeed = 25f;
	public float currentSpeed = 25f;
	private float distanceSpeed = 1f;

	private float targetAngle;

	public int resolution = 1000;


	private void Start()
	{
		targetAngle = GetCurrentAngle();

		SetupLine();
		RenderOrbitPath();
	}

	private void Update()
	{
		CalculateSpeed();
		targetAngle -= Time.deltaTime * currentSpeed;
		if(targetAngle < 0)
			targetAngle = 360 + targetAngle;

		//orbitPath.RecalculateEllipse();
		if(followOrbit)
			transform.position = orbitPath.GetPositionOnEllipse(targetAngle) + principle.transform.position;

		RenderOrbitPath();
	}

	private void RenderOrbitPath()
	{
		CalculatePath();
		
		orbitPath.DrawEllipse();
	}

	private void CalculatePath()
	{
		Vector3 principleDiff = transform.position - principle.transform.position;
		orbitPath.axisVector = axisVector;
		orbitPath.center = new Vector2(principle.transform.position.x, principle.transform.position.z);
		orbitPath.resolution = resolution;

		float ang = GetCurrentAngle();
	
		orbitPath.startAngle = -ang;
		orbitPath.RecalculateEllipse();
	}

	private void SetupLine()
	{
		LineRenderer line = gameObject.AddComponent<LineRenderer>();
		orbitPath = gameObject.AddComponent<Ellipse>();
		line.material = orbitPathMaterial;
		line.sortingOrder = -2;

		AnimationCurve curve = new AnimationCurve();
		curve.AddKey(0f, .1f);
		curve.AddKey(.33f, 0f);
		line.widthCurve = curve;
	}

	private float GetCurrentAngle()
	{
		Vector3 principleDiff = transform.position - principle.transform.position;
		float ang = Vector2.Angle(new Vector2(1, 0), new Vector2(principleDiff.x, principleDiff.z));
		Vector3 cross = Vector3.Cross(new Vector2(1, 0), new Vector2(principleDiff.x, principleDiff.z));

		if(cross.z < 0)
			ang = 360 - ang;

		return ang;
	}

	public void RestartOrbit()
	{
		followOrbit = true;
		targetAngle = GetCurrentAngle();
	}

	private void CalculateSpeed()
	{
		float distanceRatio = 1 - (Vector3.Distance(transform.position, principle.transform.position) / principle.gravityRadius);
		currentSpeed = baseSpeed * (distanceRatio * 2);
	}
}
