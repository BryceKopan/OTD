using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
	public GameObject bulletPrefab;
	public float cooldown = .33f;
	private bool readyToFire = true;

	private void OnTriggerStay(Collider other)
	{
		if(other.transform.gameObject.tag == "Enemy" && readyToFire)
		{
			FireAt(other.transform.gameObject);
		}
	}

	private void FireAt(GameObject enemy)
	{
		GameObject bulletObject = Instantiate(bulletPrefab, transform.position, transform.rotation);
		Bullet bullet = bulletObject.GetComponent<Bullet>();
		bullet.target = enemy;
		StartCoroutine(FireCooldown());
	}

	IEnumerator FireCooldown()
	{
		readyToFire = false;
		yield return new WaitForSeconds(cooldown);
		readyToFire = true;
	}
}
