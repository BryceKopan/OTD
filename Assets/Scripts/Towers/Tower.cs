using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tower : MonoBehaviour
{
	public string displayName = "Tower";

	public float cooldown = 1f;
	protected bool readyToFire = true;

	public int prestige = 0;
	public float prestigeProgress = 0, rangeGrowth, cooldownGrowth;

	public string targetTag = "Enemy";

	protected List<GameObject> targetsInRange = new List<GameObject>();
	protected Orbit orbit;

	private void Start()
	{
		orbit = GetComponent<Orbit>();
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

		FireAtFarthestTarget();
	}

	protected virtual void FireAtFarthestTarget()
	{
		if(readyToFire && targetsInRange.Count > 0)
		{
			GameObject farthestTarget = targetsInRange[0];
			Vector3 planetPosition = FindObjectOfType<Planet>().transform.position;

			for(int i = 0; i < targetsInRange.Count; i++)
			{
				if(Vector3.Distance(targetsInRange[i].transform.position, planetPosition) < Vector3.Distance(farthestTarget.transform.position, planetPosition))
				{
					farthestTarget = targetsInRange[i];
				}
			}

			FireAt(farthestTarget);
			StartCoroutine(FireCooldown());
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

	public abstract void FireAt(GameObject target);

	protected IEnumerator FireCooldown()
	{
		readyToFire = false;
		yield return new WaitForSeconds(cooldown);
		readyToFire = true;
	}

	public virtual void GetXP()
	{
		prestigeProgress += 1f / (10f + 10f * prestige);

		if(prestigeProgress >= 1 && prestige < 10)
		{
			prestige++;
			prestigeProgress = 0;

			cooldown -= cooldownGrowth;
			GetComponent<SphereCollider>().radius += rangeGrowth;
		}
	}
}
