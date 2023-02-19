using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
	public Tower tower;
	Vector3 startPosition;
	public GameObject target;
	public float speed = 1;

	public bool isTriggered = true;

	private GameController gameController;

	Vector3 lastDeltaPosition;

	// Start is called before the first frame update
	void Start()
    {
		startPosition = transform.position;
		gameController = FindObjectOfType<GameController>();
	}

    // Update is called once per frame
    void Update()
    {
		if(!target && isTriggered)
		{
			transform.position = transform.position + lastDeltaPosition;

			if(!GetComponentInChildren<SpriteRenderer>().isVisible)
				Destroy(gameObject);
		}
		else if(isTriggered)
		{
			Vector3 directionUnit = (target.transform.position - transform.position).normalized;
			Vector3 deltaPosition = directionUnit * Time.deltaTime * speed;
			transform.position = transform.position + deltaPosition;
			transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position, new Vector3(0, 1, 0));
			lastDeltaPosition = deltaPosition;
		}
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
