using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiningTower : Tower
{
	public GameObject shipPrefab;

	private GameController gameController;
	private GameObject target;
	private GameObject ship;

	bool hasResources = false;
	Vector3 startPosition;
	float lerpT = 0;
	public float speed = 1;

	private new void Start()
	{
		base.Start();
		gameController = FindObjectOfType<GameController>();
		StartCoroutine(FireCooldown());
	}

	private void Update()
	{
		lerpT += Time.deltaTime * speed;

		if(ship && target)
		{
			ship.transform.position = Vector3.Lerp(startPosition, target.transform.position, lerpT);
			ship.transform.rotation = Quaternion.LookRotation(target.transform.position - ship.transform.position, new Vector3(0, 1, 0));

			if(ship.transform.position == target.transform.position && !hasResources)
			{
				target.GetComponent<Asteroid>().GatherResources(.25f);
				hasResources = true;
				target = gameObject;
				startPosition = ship.transform.position;
				lerpT = 0;
			}
			else if (ship.transform.position == target.transform.position && hasResources)
			{
				GetXP();
				gameController.AddResources(.25f);
				Destroy(ship);
				hasResources = false;
			}
		}
	}

	public override IEnumerator FireAt(GameObject asteroid)
	{
		target = asteroid;
		startPosition = transform.position;
		ship = Instantiate(shipPrefab, transform.position, transform.rotation);
		lerpT = 0;

		yield return null;
	}
}
