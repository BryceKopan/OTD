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
			FireAt(gameObject);
			StartCoroutine(FireCooldown());
		}
	}

	public override void FireAt(GameObject target)
	{
		if(targetsInRange.Count < maxMineDensity)
		{
			Vector2 randomPosition = Random.insideUnitCircle * GetComponent<SphereCollider>().radius;
			Vector3 minePosition = transform.position + new Vector3(randomPosition.x, 0, randomPosition.y);

			GameObject mineObject = Instantiate(minePrefab, minePosition, transform.rotation);
			Mine mine = mineObject.GetComponent<Mine>();
			mine.tower = GetComponent<Tower>();
			mine.GetComponent<Orbit>().principle = orbit.principle;
		}
	}

	protected override void FireAtFarthestTarget()
	{
	}
}
