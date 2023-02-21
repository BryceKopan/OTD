using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissleBattery : Tower
{
	public GameObject misslePrefab;
	public float delayBetweenMissles, spread;
	public int misslesPerVolley;

	public override void FireAt(GameObject target)
	{
		StartCoroutine(FireVolley(target));
	}

	IEnumerator FireVolley(GameObject target)
	{
		Vector3 direction = (target.transform.position - transform.position).normalized;
		Debug.Log("firing");
		for(int i = 0; i < misslesPerVolley; i++)
		{
			Debug.Log("pewsh" + i);
			GameObject bulletObject = Instantiate(misslePrefab, transform.position, transform.rotation, FindObjectOfType<Planet>().transform);
			Bullet bullet = bulletObject.GetComponent<Bullet>();
			bullet.tower = GetComponent<Tower>();

			direction = Quaternion.Euler(0, Random.Range(-spread/2, spread/2), 0) * direction;
			bullet.targetDirectionUnit = direction.normalized;

			yield return new WaitForSeconds(delayBetweenMissles);
		}
	}
}
