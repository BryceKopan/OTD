using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
	public int population = 3, resourcers = 3, researchers = 0;
	public int timeToPopulationGrowth = 5;
	public GameObject healthText, population1, population2, resoucersText, researchersText;

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
		if(planetOrbit.GetCurrentAngle() <= nextSeasonAngle || nextSeasonAngle == 0 && planetOrbit.GetCurrentAngle() > 90)
		{
			if(nextSeasonAngle == 0)
				nextSeasonAngle = 360;
			nextSeasonAngle -= 90;
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
		population1.GetComponent<UnityEngine.UI.Text>().text = "Pop: " + population;
		population2.GetComponent<UnityEngine.UI.Text>().text = "Population: " + population;

		if(population <= 0)
		{
			PlanetDestroyed();
		}
	}

	private void PlanetDestroyed()
	{
		FindObjectOfType<GameController>().endGameUI.SetActive(true);
	}

	public void IncrementSeason()
	{
		GC.AddResources(resourcers);


		timeToPopulationGrowth--;
		int populationPairs = population / 2;
		if(timeToPopulationGrowth <= 0)
		{
			for(int i = 0; i < populationPairs; i++)
			{
				population++;
			}

			timeToPopulationGrowth = 4;
		}

		healthText.GetComponent<UnityEngine.UI.Text>().text = "Health: " + population;
		population1.GetComponent<UnityEngine.UI.Text>().text = "Pop: " + population;
		population2.GetComponent<UnityEngine.UI.Text>().text = "Population: " + population;
	}

	public void IncreaseResourcers()
	{
		if(resourcers < population)
			resourcers++;
		if(resourcers + researchers > population)
			researchers--;

		resoucersText.GetComponent<UnityEngine.UI.Text>().text = "Resource Gatherers: " + resourcers;
		researchersText.GetComponent<UnityEngine.UI.Text>().text = "Researchers: " + researchers;
	}

	public void IncreaseResearchers()
	{
		if(researchers < population)
			researchers++;
		if(resourcers + researchers > population)
			resourcers--;

		resoucersText.GetComponent<UnityEngine.UI.Text>().text = "Resource Gatherers: " + resourcers;
		researchersText.GetComponent<UnityEngine.UI.Text>().text = "Researchers: " + researchers;
	}
}
