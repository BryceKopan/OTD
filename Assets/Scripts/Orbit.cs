using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
	public bool followOrbit = true, varySpeed = true;
	public bool useStartingPositionAsOrbit = false;

	//Visual Elements of Orbit
	public Material orbitPathMaterial;
	public Color color = Color.white;
	public float trailLength = .33f;
	public LineRenderer orbitPath;

	public CelestialBody principle;
	public Vector2 axisVector;
	public float baseSpeed = 25f;
	public float currentSpeed = 25f;

	private float targetAngle;


	private void Start()
	{
		targetAngle = GetCurrentAngle();

		if(!orbitPath)
			SetupLine();

		if(useStartingPositionAsOrbit)
			SetCurrentPositionAsOrbit();
	}

	private void Update()
	{
		if(Time.deltaTime == 0)
		{
			targetAngle = GetCurrentAngle();
			RenderOrbitPath();
		}
	}

	private void FixedUpdate()
	{
		if(varySpeed)
			CalculateSpeed();
		targetAngle -= Time.deltaTime * currentSpeed;
		if(targetAngle < 0)
			targetAngle = 360 + targetAngle;

		if(followOrbit)
			transform.position = GetPositionAt(targetAngle);

		RenderOrbitPath();
	}

	public Vector3 GetPositionAt(float angle)
	{
		angle = angle * Mathf.Deg2Rad;
		Vector3 newPosition = new Vector3(axisVector.x * Mathf.Cos(angle) + principle.transform.position.x, 0.0f, axisVector.y * Mathf.Sin(angle) + principle.transform.position.z);
		return newPosition;
	}

	public void SetupLine()
	{
		orbitPath = gameObject.AddComponent<LineRenderer>();
		orbitPath.material = orbitPathMaterial;
		orbitPath.sortingOrder = -10;

		orbitPath.startColor = color;
		orbitPath.endColor = color;

		AnimationCurve curve = new AnimationCurve();
		curve.AddKey(0f, .1f);
		curve.AddKey(1f, 0f);
		orbitPath.widthCurve = curve;
		RenderOrbitPath();
	}

	private void RenderOrbitPath()
	{
		//Calculate line points
		int pointCount = (int)(360 * trailLength);
		orbitPath.positionCount = pointCount;
		for(int i = 0; i < pointCount; i++)
		{
			orbitPath.SetPosition(i, GetPositionAt(targetAngle + i));
		}
	}

	public float GetCurrentAngle()
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

	public void SetCurrentPositionAsOrbit()
	{
		if(principle)
		{
			float orbitSize = Vector3.Distance(transform.position, principle.transform.position);
			axisVector = new Vector2(orbitSize, orbitSize);
		}
	}
}
