using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tower : MonoBehaviour
{
	public string displayName = "Tower";

	public float cooldown = 1f;
	private bool readyToFire = true;

	public int prestige = 0;
	public float prestigeProgress = 0, rangeGrowth, cooldownGrowth;

	public string targetTag = "Enemy";


	private void OnTriggerStay(Collider other)
	{
		if(other.transform.gameObject.tag == targetTag && readyToFire)
		{
			FireAt(other.transform.gameObject);
			StartCoroutine(FireCooldown());
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
