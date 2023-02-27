using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelestialBody : MonoBehaviour
{
	[SerializeField]
	private int population;
	public int Population
	{
		get { return population; }
		set
		{
			population = value;
			population1.GetComponent<UnityEngine.UI.Text>().text = "Pop: " + population;
			if(population > 0)
				isPopulated = true;
			else
				isPopulated = false;
		}
	}

	[SerializeField]
	private bool isExplored = true;
	public bool IsExplored
	{
		get { return isExplored; }
		set
		{
			isExplored = value;
		}
	}

	public float gravityRadius;
	public bool isPopulated = false, isOrigin = false;
	public int resourcers, researchers;
	public int timeToPopulationGrowth;
	public GameObject population1;

	private GameController GC;
	private Orbit planetOrbit;

	float nextSeasonAngle = 360;

	public string planetName;

	public List<GameObject> objectsToHideWithPlanet;

	private void Start()
	{
		GC = FindObjectOfType<GameController>();
		planetOrbit = GetComponent<Orbit>();
	}

	private void OnCollisionEnter(Collision collision)
	{
		if(collision.transform.gameObject.tag == "Enemy" || collision.transform.gameObject.tag == "Bullet" || collision.transform.gameObject.tag == "Mine")
		{
			Destroy(collision.transform.gameObject);
		}
		if(collision.transform.gameObject.tag == "Enemy")
		{
			TakeDamage(collision.transform.gameObject);
		}
	}

	private void FixedUpdate()
	{
		if(isOrigin)
		{
			if(planetOrbit.GetCurrentAngle() <= nextSeasonAngle || nextSeasonAngle == 0 && planetOrbit.GetCurrentAngle() > 90)
			{
				if(nextSeasonAngle == 0)
					nextSeasonAngle = 360;
				nextSeasonAngle -= 90;
				GC.StartNextSeason();
			}
		}
	}

	private void TakeDamage(GameObject enemy)
	{
		if(isPopulated)
		{
			Destroy(enemy);
			Population--;

			if(resourcers > researchers)
				resourcers--;
			else
				researchers--;

			if(Population <= 0)
			{
				PlanetDestroyed();
			}
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
		int populationPairs = Population / 2;
		if(timeToPopulationGrowth <= 0)
		{
			for(int i = 0; i < populationPairs; i++)
			{
				Population++;
			}

			timeToPopulationGrowth = 4;
		}

		population1.GetComponent<UnityEngine.UI.Text>().text = "Pop: " + Population;
	}

	public void IncreaseResourcers()
	{
		if(resourcers < Population)
			resourcers++;
		if(resourcers + researchers > Population)
			researchers--;
	}

	public void IncreaseResearchers()
	{
		if(researchers < Population)
			researchers++;
		if(resourcers + researchers > Population)
			resourcers--;
	}

	public void TransferPopulationTo(CelestialBody body)
	{
		Population--;
		body.Population++;
	}

	public void ToggleIsExplored()
	{
		if(IsExplored)
		{
			//Hide Stuff
		}
		else
		{
			//UnHide Stuff
		}
	}
}
