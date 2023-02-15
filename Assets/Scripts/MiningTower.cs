using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiningTower : MonoBehaviour
{
	public string displayName = "Mining Tower";
	public GameObject shipPrefab;
	public float cooldown = 5f;
	private bool readyToMine = false;

	private GameController gameController;
	private GameObject target;
	private GameObject ship;

	bool hasResources = false;
	Vector3 startPosition;
	float lerpT = 0;
	public float speed = 1;

	public int prestige = 0;
	public float prestigeProgress = 0;

	public float rangeGrowth, cooldownGrowth;

	private void Start()
	{
		gameController = FindObjectOfType<GameController>();
		if(!readyToMine)
			StartCoroutine(MineCooldown());
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

	private void OnTriggerStay(Collider other)
	{
		if(other.transform.gameObject.tag == "Asteroid" && readyToMine)
		{
			Mine(other.transform.gameObject);
		}
	}

	private void Mine(GameObject asteroid)
	{
		target = asteroid;
		startPosition = transform.position;
		ship = Instantiate(shipPrefab, transform.position, transform.rotation);
		StartCoroutine(MineCooldown());
		lerpT = 0;
	}

	IEnumerator MineCooldown()
	{
		readyToMine = false;
		yield return new WaitForSeconds(cooldown);
		readyToMine = true;
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
