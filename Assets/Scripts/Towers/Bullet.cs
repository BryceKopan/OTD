using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
	public Tower tower;
	public GameObject target;
	public float speed = 1;

	public bool isTriggered = true, destroyWhenUnseen = true;

	public Vector3 targetDirectionUnit;
	Vector3 lastDeltaPosition;

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
		Vector3 deltaPosition = targetDirectionUnit * Time.deltaTime * speed;
		transform.position = transform.position + deltaPosition;
		transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position, new Vector3(0, 1, 0));
		lastDeltaPosition = deltaPosition;
	}

	protected virtual void MoveWithoutTarget()
	{
		transform.position = transform.position + lastDeltaPosition;
	}

	protected virtual void OnCollisionEnter(Collision collision)
	{
		if(collision.gameObject.tag == "Enemy")
		{
			if(tower.GetComponent<Tower>())
				tower.GetComponent<Tower>().GetXP();
			Destroy(collision.transform.gameObject);
			Destroy(gameObject);
		}
	}
}
