using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelestialBody : MonoBehaviour
{
	public string displayName;

	public int maxPopulation;
	[SerializeField]
	private int population;
	public int Population
	{
		get { return population; }
		set
		{
			if(value > maxPopulation)
				value = maxPopulation;
			else if(value < 0)
				value = 0;

			int deltaPopulation = value - population;
			if(deltaPopulation > 0 && population > 0)
				GC.SC.PopulationGained += deltaPopulation;
			else
				GC.SC.PopulationLost -= deltaPopulation;

			population = value;

			if(population1)
				population1.GetComponent<UnityEngine.UI.Text>().text = "Pop: " + population;

			if(population > 0 && !isPopulated)
			{
				isPopulated = true;
				if(unlockedTowerPrefab)
					GC.UnlockTower(unlockedTowerPrefab);
				GC.populatedBodies.Add(this);
			}
			else if(population < 0 && isPopulated)
			{
				isPopulated = false;
				GC.populatedBodies.Remove(this);
			}
		}
	}

	public GameObject unlockedTowerPrefab;

	public float gravityRadius;
	public bool isPopulated = false, isOrigin = false;
	public int resourcers, researchers;
	public int timeToPopulationGrowth, populationGrowthTime = 4;
	public GameObject population1;

	private GameController GC;
	private TechController TC;
	private Orbit planetOrbit;

	float nextSeasonAngle = 360;

	public string planetName;

	public List<GameObject> objectsToHideWithPlanet;

	private void Start()
	{
		GC = FindObjectOfType<GameController>();
		TC = FindObjectOfType<TechController>();
		planetOrbit = GetComponent<Orbit>();
		timeToPopulationGrowth = populationGrowthTime;
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
		if(isOrigin)
			GC.EndGame();
	}

	public void IncrementSeason()
	{
		GC.AddResources(resourcers);
		TC.AddResearch(researchers);

		timeToPopulationGrowth--;
		int populationPairs = Population / 2;
		if(timeToPopulationGrowth <= 0)
		{
			for(int i = 0; i < populationPairs; i++)
			{
				if(Population < maxPopulation)
				{
					Population++;
					resourcers++;
				}
			}

			timeToPopulationGrowth = populationGrowthTime;
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

	/*public void ToggleIsExplored()
	{
		if(IsExplored)
		{
			//Hide Stuff
		}
		else
		{
			//UnHide Stuff
		}
	}*/
}
