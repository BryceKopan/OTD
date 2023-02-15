using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
	public GameObject tower;
	Vector3 startPosition;
	public GameObject target;
	public float speed = 1;

	private GameController gameController;

	Vector3 lastDeltaPosition;

	// Start is called before the first frame update
	void Start()
    {
		//orbit = gameObject.GetComponent<Orbit>();
		startPosition = transform.position;
		gameController = FindObjectOfType<GameController>();
	}

    // Update is called once per frame
    void Update()
    {
		if(!target)
		{
			transform.position = transform.position + lastDeltaPosition;

			if(!GetComponentInChildren<SpriteRenderer>().isVisible)
				Destroy(gameObject);
		}
		else
		{
			Vector3 directionUnit = (target.transform.position - transform.position).normalized;
			Vector3 deltaPosition = directionUnit * Time.deltaTime * speed;
			transform.position = transform.position + deltaPosition;
			transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position, new Vector3(0, 1, 0));
			lastDeltaPosition = deltaPosition;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "Enemy")
		{
			tower.GetComponent<Tower>().GetXP();
			Destroy(other.transform.gameObject);
			Destroy(gameObject);
		}
	}
}
