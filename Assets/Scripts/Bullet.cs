using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
	Vector3 startPosition;
	public GameObject target;
	float lerpT = 0;
	public float speed = 1;

	private GameController gameController;

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
		lerpT += Time.deltaTime * speed;

		if(!target)
			Destroy(gameObject);
		else
		{
			transform.position = Vector3.Lerp(startPosition, target.transform.position, lerpT);
			transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position, new Vector3(0, 1, 0));
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "Enemy")
		{
			gameController.AddResources(other.gameObject.GetComponent<Enemy>().resourceValue);
			Destroy(other.transform.gameObject);
			Destroy(gameObject);
		}
	}
}
