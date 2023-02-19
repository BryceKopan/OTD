using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailgunBullet : Bullet
{
	override protected void OnCollisionEnter(Collision collision)
	{
		if(collision.gameObject.tag == "Enemy")
		{
			if(tower.GetComponent<Tower>())
				tower.GetComponent<Tower>().GetXP();
			Destroy(collision.transform.gameObject);
		}
	}
}
