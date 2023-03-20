using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubOrbital : MonoBehaviour
{
	public GameObject target;

	public Vector3 directionUnit;
	public float speed;
	public float acceleration;
	public float turnSpeed;
	public float turnAcceleration;

	private void FixedUpdate()
	{
		Vector3 deltaPosition = directionUnit * Time.deltaTime * speed;
		transform.position = transform.position + deltaPosition;
		transform.rotation = Quaternion.LookRotation(directionUnit, new Vector3(0, 1, 0));

		directionUnit = Vector3.RotateTowards(directionUnit, (target.transform.position - transform.position), turnSpeed * Time.deltaTime, 0).normalized;
		speed += Time.deltaTime * acceleration;
		turnSpeed += Time.deltaTime * turnAcceleration;
	}
}
