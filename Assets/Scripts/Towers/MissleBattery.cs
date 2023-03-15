using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissleBattery : Tower
{
	public GameObject misslePrefab;
	public float delayBetweenMissles, spread;
	public int baseMissles, missleGrowth;
	private int missles;

	public new void Start()
	{
		base.Start();
		missles = baseMissles;
	}

	public override IEnumerator FireAt(GameObject target)
	{
		Vector3 direction = (target.transform.position - transform.position).normalized;
		Vector3 adjustment = GetParallellFireVector(target);

		int misslesPerVolley = missles * projectilesPerVolley;
		int curProjectilesPerFire = projectilesPerVolley;
		if(burstFire)
			misslesPerVolley *= 3;

		for(int i = 0; i < misslesPerVolley; i += curProjectilesPerFire)
		{
			List<GameObject> bullets = new List<GameObject>();

			if(parallelFire)
			{
				for(int j = 0; j < projectilesPerVolley; j++)
				{
					GameObject bulletObject = Instantiate(misslePrefab, transform.position - (adjustment * (distanceBetweenProjectiles / 2)) + (adjustment * distanceBetweenProjectiles * j), transform.rotation, GetComponent<Orbit>().principle.transform);
					bullets.Add(bulletObject);
				}
			}
			else
			{
				GameObject bulletObject = Instantiate(misslePrefab, transform.position, transform.rotation, GetComponent<Orbit>().principle.transform);
				bullets.Add(bulletObject);
			}

			foreach(GameObject bulletObject in bullets)
			{
				Bullet bullet = bulletObject.GetComponent<Bullet>();
				bullet.tower = GetComponent<Tower>();

				direction = Quaternion.Euler(0, Random.Range(-spread / 2, spread / 2), 0) * direction;
				bullet.targetDirectionUnit = direction.normalized;
			}

			float actualDelay = delayBetweenMissles;

			if(!burstFire || (burstFire && (i+1) % 3 != 0))
			{
				yield return new WaitForSeconds(delayBetweenMissles);
			}
			else
				yield return new WaitForSeconds(delayBetweenMissles * 3);
		}

		StartCoroutine(FireCooldown());
	}

	public override void SetRankStats()
	{
		base.SetRankStats();
		missles = baseMissles + (Rank * missleGrowth);
	}
}
