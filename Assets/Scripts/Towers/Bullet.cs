using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
	public Tower tower;
	public GameObject target;
	public float speed = 1;

	public bool isTriggered = true, destroyWhenUnseen = true, hasHit = false;

	public Vector3 targetDirectionUnit;
	public Vector3 lastDeltaPosition;

	public int damage = 1;

	public float deflectionAmount = .1f;

    // Update is called once per frame
    void Update()
    {
		if(!target && isTriggered)
		{
			MoveWithoutTarget();

			if(!GetComponentInChildren<SpriteRenderer>().isVisible && destroyWhenUnseen)
				Destroy(gameObject);
		}
		else if(isTriggered)
		{
			MoveTowardsTarget();
		}
	}

	protected virtual void MoveTowardsTarget()
	{
		targetDirectionUnit = (target.transform.position - transform.position).normalized;
		Vector3 deltaPosition = targetDirectionUnit;
		transform.position = transform.position + deltaPosition * Time.deltaTime * speed;
		transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position, new Vector3(0, 1, 0));
		lastDeltaPosition = deltaPosition;
	}

	protected virtual void MoveWithoutTarget()
	{
		transform.rotation = Quaternion.LookRotation(lastDeltaPosition, new Vector3(0, 1, 0));
		transform.position = transform.position + lastDeltaPosition * Time.deltaTime * speed;
	}

	protected virtual void OnCollisionEnter(Collision collision)
	{
		if(collision.gameObject.tag == "Enemy" && !hasHit)
		{
			hasHit = true;
			if(collision.gameObject.GetComponent<Enemy>().shield.ShieldStrength <= 0)
			{
				Destroy(gameObject);
				if(tower)
					if(tower.GetComponent<Tower>())
						tower.GetComponent<Tower>().GetXP();
				collision.gameObject.GetComponent<Enemy>().Health -= damage;
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
