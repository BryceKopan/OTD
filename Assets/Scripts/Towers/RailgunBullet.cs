using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailgunBullet : Bullet
{
	override protected void OnCollisionEnter(Collision collision)
	{
		if(collision.gameObject.tag == "Enemy" && !hasHit)
		{
			hasHit = true;
			if(collision.gameObject.GetComponent<Enemy>().shield.ShieldStrength <= 0)
			{
				if(tower)
					if(tower.GetComponent<Tower>())
						tower.GetComponent<Tower>().GetXP();
				collision.gameObject.GetComponent<Enemy>().Health -= damage;
				if(collision.gameObject.GetComponent<Enemy>().Health > 0)
					Destroy(gameObject);
			}
			else
			{
				collision.gameObject.GetComponent<Enemy>().shield.ShieldStrength--;

				if(GetComponent<Missle>())
				{
					Destroy(gameObject);
				}
				else
				{
					target = null;
					bool deflectedDirection = true;
					if(Random.Range(0f, 1f) > .5)
						deflectedDirection = false;

					Vector3 deflectionVector = new Vector3(0, 0, 0);
					if(deflectedDirection)
					{
						deflectionVector.x -= Random.Range(deflectionAmount / 2, deflectionAmount);
						deflectionVector.z -= Random.Range(deflectionAmount / 2, deflectionAmount);
					}
					else
					{
						deflectionVector.x += Random.Range(deflectionAmount / 2, deflectionAmount);
						deflectionVector.z += Random.Range(deflectionAmount / 2, deflectionAmount);
					}

					lastDeltaPosition = (new Vector3(-lastDeltaPosition.x, 0, -lastDeltaPosition.z) + deflectionVector).normalized;
				}
			}
		}
	}
}
