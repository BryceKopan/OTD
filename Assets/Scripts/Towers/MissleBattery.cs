using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissleBattery : Tower
{
	public GameObject misslePrefab;
	public int misslesPerVolley;

	public override void FireAt(GameObject target)
	{
		for(int i = 0; i < misslesPerVolley; i++)
		{
			GameObject bulletObject = Instantiate(misslePrefab, transform.position, transform.rotation, FindObjectOfType<Planet>().transform);
			Bullet bullet = bulletObject.GetComponent<Bullet>();
			bullet.tower = GetComponent<Tower>();
			bullet.lastDeltaPosition = new Vector3(Random.Range(-1,1), 0, Random.Range(-1,1));
			bullet.isTriggered = true;
		}
	}
}
