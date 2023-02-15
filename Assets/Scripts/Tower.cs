using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
	public string displayName = "Laser Tower";
	public GameObject bulletPrefab;
	public float cooldown = .33f;
	private bool readyToFire = true;

	public int prestige = 0;
	public float prestigeProgress = 0;
	public float rangeGrowth, cooldownGrowth;


	private void OnTriggerStay(Collider other)
	{
		if(other.transform.gameObject.tag == "Enemy" && readyToFire)
		{
			FireAt(other.transform.gameObject);
		}
	}

	private void FireAt(GameObject enemy)
	{
		GameObject bulletObject = Instantiate(bulletPrefab, transform.position, transform.rotation, FindObjectOfType<Planet>().transform);
		Bullet bullet = bulletObject.GetComponent<Bullet>();
		bullet.tower = gameObject;
		bullet.target = enemy;
		StartCoroutine(FireCooldown());
	}

	IEnumerator FireCooldown()
	{
		readyToFire = false;
		yield return new WaitForSeconds(cooldown);
		readyToFire = true;
	}

	public void GetXP()
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
