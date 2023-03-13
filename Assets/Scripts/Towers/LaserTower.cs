using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTower : Tower
{
	public GameObject bulletPrefab;
	public float attackCooldownGrowthPercent;

	public override IEnumerator FireAt(GameObject target)
	{
		Vector3 targetPosition = target.transform.position;

		int curProjectilesPerFire = projectilesPerVolley;
		int shortShots = 1;
		if(burstFire)
		{
			curProjectilesPerFire = curProjectilesPerFire / 3;
			shortShots = 3;
		}

		for(int b = 0; b < shortShots; b++)
		{
			List<GameObject> bullets = new List<GameObject>();

			if(parallelFire)
			{
				Vector3 adjustment = GetParallellFireVector(target);

				for(int i = 0; i < curProjectilesPerFire; i++)
				{
					GameObject newObj = Instantiate(bulletPrefab, transform.position - (adjustment * (distanceBetweenProjectiles / 2)) + (adjustment * distanceBetweenProjectiles * i), transform.rotation, GetComponent<Orbit>().principle.transform);
					bullets.Add(newObj);
				}
			}
			else
			{
				GameObject newObj = Instantiate(bulletPrefab, transform.position, transform.rotation, GetComponent<Orbit>().principle.transform);
				bullets.Add(newObj);
			}

			foreach(GameObject bulletObj in bullets)
			{
				Bullet bullet = bulletObj.GetComponent<Bullet>();
				bullet.tower = GetComponent<Tower>();
				bullet.target = target;
				bullet.lastDeltaPosition = (targetPosition - transform.position).normalized;
			}

			if(target != null)
				targetPosition = target.transform.position;

			if(burstFire)
				yield return new WaitForSeconds(.1f);
		}

		StartCoroutine(FireCooldown());
	}

	public override void SetRankStats()
	{
		base.SetRankStats();
		cooldown = BaseCooldown - (Rank * (BaseCooldown * attackCooldownGrowthPercent / 100));
	}
}
