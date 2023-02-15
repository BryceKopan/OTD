using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTower : Tower
{
	public GameObject bulletPrefab;

	public override void FireAt(GameObject target)
	{
		GameObject bulletObject = Instantiate(bulletPrefab, transform.position, transform.rotation, FindObjectOfType<Planet>().transform);
		Bullet bullet = bulletObject.GetComponent<Bullet>();
		bullet.tower = gameObject;
		bullet.target = target;
	}
}
