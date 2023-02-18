using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
	public int population = 3;
	public GameObject healthText;

	private GameController GC;
	private Orbit planetOrbit;

	float nextSeasonAngle = 360;

	private void Start()
	{
		GC = FindObjectOfType<GameController>();
		planetOrbit = GetComponent<Orbit>();
	}

	private void FixedUpdate()
	{
		if(planetOrbit.GetCurrentAngle() <= nextSeasonAngle)
		{
			nextSeasonAngle -= 90;
			if(nextSeasonAngle < 0)
				nextSeasonAngle = 360;
			GC.StartNextSeason();
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if(collision.transform.gameObject.tag == "Enemy")
		{
			TakeDamage(collision.transform.gameObject);
		}
	}

	private void TakeDamage(GameObject enemy)
	{
		Destroy(enemy);
		population--;
		healthText.GetComponent<UnityEngine.UI.Text>().text = "Health: " + population;

		if(population <= 0)
		{
			PlanetDestroyed();
		}
	}

	private void PlanetDestroyed()
	{
		FindObjectOfType<GameController>().endGameUI.SetActive(true);
	}
}
