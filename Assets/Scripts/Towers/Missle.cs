using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missle : Bullet
{
	public float turnSpeed;
	Vector3 directionUnit;

	private void Start()
	{
		transform.rotation = Quaternion.LookRotation(targetDirectionUnit, Vector3.up);
		directionUnit = transform.forward;
	}

	protected override void MoveTowardsTarget()
	{
		Vector3 deltaPosition = directionUnit * Time.deltaTime * speed;
		transform.position = transform.position + deltaPosition;
		transform.rotation = Quaternion.LookRotation(directionUnit, new Vector3(0, 1, 0));

		directionUnit = Vector3.RotateTowards(directionUnit, (target.transform.position - transform.position), turnSpeed * Time.deltaTime, 0).normalized;
	}

	protected override void MoveWithoutTarget()
	{
		Vector3 deltaPosition = directionUnit * Time.deltaTime * speed;
		transform.position = transform.position + deltaPosition;
		transform.rotation = Quaternion.LookRotation(directionUnit, new Vector3(0, 1, 0));

		directionUnit = Vector3.RotateTowards(directionUnit, directionUnit + new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1)), turnSpeed * Time.deltaTime, 0).normalized;
	}

	private void FixedUpdate()
	{
		Enemy[] enemies = FindObjectsOfType<Enemy>();
		if(enemies.Length > 0)
		{
			Enemy closest = enemies[0];

			for(int i = 0; i < enemies.Length; i++)
			{
				if(Vector3.Distance(enemies[i].transform.position, transform.position) < Vector3.Distance(closest.transform.position, transform.position))
				{
					closest = enemies[i];
				}
			}

			target = closest.gameObject;
		}
	}
}
