using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : Bullet
{
	public string targetTag = "Enemy";
	public float explosionRadius;

	protected List<GameObject> targetsInRange = new List<GameObject>();

	override protected void OnCollisionEnter(Collision collision)
	{
		if(collision.gameObject.tag == "Enemy")
		{
			foreach(GameObject obj in targetsInRange)
			{
				if(Vector3.Distance(transform.position, obj.transform.position) < explosionRadius)
				{
					Destroy(obj);
					tower.GetXP();
				}
			}
			Destroy(gameObject);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.transform.gameObject.tag == targetTag)
		{
			targetsInRange.Add(other.transform.gameObject);
		}
	}
	private void OnTriggerExit(Collider other)
	{
		if(other.transform.gameObject.tag == targetTag)
		{
			targetsInRange.Remove(other.transform.gameObject);
		}
	}

	private void FixedUpdate()
	{
		for(int i = 0; i < targetsInRange.Count; i++)
		{
			if(targetsInRange[i] == null)
			{
				targetsInRange.RemoveAt(i);
				i--;
			}
		}

		FireAtNearestTarget();
	}

	protected virtual void FireAtNearestTarget()
	{
		Orbit orbit = GetComponent<Orbit>();

		if(targetsInRange.Count > 0)
		{
			orbit.followOrbit = false;
			transform.parent = orbit.principle.transform;
			orbit.orbitPath.enabled = false;
			isTriggered = true;
			GameObject nearestTarget = targetsInRange[0];

			for(int i = 0; i < targetsInRange.Count; i++)
			{
				if(Vector3.Distance(targetsInRange[i].transform.position, transform.position) < Vector3.Distance(nearestTarget.transform.position, transform.position))
				{
					nearestTarget = targetsInRange[i];
				}
			}

			target = nearestTarget;
		}
		else if(isTriggered)
		{
			orbit.SetCurrentPositionAsOrbit();
			orbit.orbitPath.enabled = true;
			isTriggered = false;
			transform.parent = null;
			orbit.RestartOrbit();
		}
	}
}
