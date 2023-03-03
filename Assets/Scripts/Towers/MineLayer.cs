using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineLayer : Tower
{
	public GameObject minePrefab;
	public int maxMineDensity;

	private void Update()
	{
		if(readyToFire && Time.timeScale > 0)
		{
			readyToFire = false;
			StartCoroutine(FireAt(gameObject));
		}
	}

	public override IEnumerator FireAt(GameObject target)
	{
		int curProjectilesPerFire = projectilesPerVolley;
		int shortShots = 1;
		if(burstFire)
		{
			curProjectilesPerFire = curProjectilesPerFire / 3;
			shortShots = 3;
		}

		for(int b = 0; b < shortShots; b++)
		{
			if(targetsInRange.Count < maxMineDensity)
			{
				List<GameObject> mines = new List<GameObject>();

				Vector2 randomPosition = Random.insideUnitCircle * GetComponent<SphereCollider>().radius;
				for(int i = 0; i < curProjectilesPerFire; i++)
				{
					if(i > 0)
						randomPosition += Random.insideUnitCircle * distanceBetweenProjectiles;
					Vector3 minePosition = transform.position + new Vector3(randomPosition.x, 0, randomPosition.y);
					GameObject mineObject = Instantiate(minePrefab, minePosition, transform.rotation);
					mines.Add(mineObject);
				}

				foreach(GameObject mineObject in mines)
				{
					Mine mine = mineObject.GetComponent<Mine>();
					mine.tower = GetComponent<Tower>();
					mine.GetComponent<Orbit>().principle = orbit.principle;
				}
			}

			if(burstFire)
				yield return new WaitForSeconds(.2f);
		}

		StartCoroutine(FireCooldown());
	}

	protected override void FireAtFarthestTarget()
	{
	}
}
