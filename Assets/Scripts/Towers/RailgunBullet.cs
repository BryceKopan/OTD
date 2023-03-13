using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailgunBullet : Bullet
{
	override protected void OnCollisionEnter(Collision collision)
	{
		if(collision.gameObject.tag == "Enemy")
		{
			tower.GetXP();
			collision.gameObject.GetComponent<Enemy>().Health -= damage;
		}
	}
}
