using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulatedBody : CelestialBody
{
	public int maxPopulation;
	[SerializeField]
	private int population = 0;
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
			if(deltaPopulation > 0)
			{
				resourcers += deltaPopulation;
				if(population > 0)
					GC.SC.PopulationGained += deltaPopulation;
			}
			else if(deltaPopulation < 0)
				GC.SC.PopulationLost -= deltaPopulation;

			population = value;

			if(population > 0 && !isPopulated)
			{
				isPopulated = true;
				if(info.unlockedTechs.Count > 0)
					UnlockTechnology();
				GC.populatedBodies.Add(this);
			}
			else if(population < 0 && isPopulated)
			{
				isPopulated = false;
				GC.populatedBodies.Remove(this);
			}
		}
	}

	public bool isPopulated = false;
	public int resourcers, researchers;
	public float resourceRatio = 1, researchRatio = 1;
	public int timeToPopulationGrowth, populationGrowthTime = 4;

	float nextSeasonAngle = 360;
	private Orbit planetOrbit;

	protected override void Start()
	{
		base.Start();

		timeToPopulationGrowth = populationGrowthTime;
		planetOrbit = GetComponent<Orbit>();

		if(SavedData.saveData.hasMiningPlanetsTalent && info.type == BodyType.TerrestrialPlanet)
		{
			resourceRatio += .25f;
		}

		if(SavedData.saveData.hasResearchPlanetsTalent && info.type == BodyType.TerrestrialPlanet)
		{
			researchRatio += .25f;
		}

		if(SavedData.saveData.hasMiningMoonsTalent && info.type == BodyType.Moon)
		{
			resourceRatio += .25f;
		}

		if(SavedData.saveData.hasResearchMoonsTalent && info.type == BodyType.Moon)
		{
			researchRatio += .25f;
		}

		info.isExplored = true;
		if(info.unlockedTowerPrefab)
			GC.UnlockTower(info.unlockedTowerPrefab);
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();

		if(info.isOrigin)
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

	protected override void TakeDamage(GameObject enemy)
	{
		base.TakeDamage(enemy);

		Destroy(enemy);
		Population--;

		if(resourcers > researchers)
			resourcers--;
		else
			researchers--;

		if(Population <= 0)
		{
			BodyDestroyed();
		}
	}

	protected override void BodyDestroyed()
	{
		base.BodyDestroyed();

		if(info.isOrigin)
			GC.EndGame();
	}

	public void IncrementSeason()
	{
		GC.AddResources(resourcers * resourceRatio);
		GC.TC.AddResearch((int) (researchers * researchRatio));

		timeToPopulationGrowth--;
		int populationPairs = Population / 2;
		if(timeToPopulationGrowth <= 0)
		{
			for(int i = 0; i < populationPairs; i++)
			{
				if(Population < maxPopulation)
				{
					Population++;
				}
			}

			timeToPopulationGrowth = populationGrowthTime;
		}
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

	public void StartPopulationTransfer(GameObject body)
	{
		if(!travelTargets.ContainsKey(body))
			travelTargets[body] = new List<GameObject>();

		Population--;
		if(resourcers > researchers)
			resourcers--;
		else
			researchers--;
		GameObject travelShip = Instantiate(GC.populationTransferShipPrefab, transform.position, Quaternion.identity, body.transform);
		travelTargets[body].Add(travelShip);
	}

	public override void FinishTransfer(GameObject body, GameObject ship)
	{
		if(ship.tag == "PopulationShip")
		{
			Destroy(ship);
			travelTargets[body].Remove(ship);

			PopulatedBody pb = body.GetComponent<PopulatedBody>();
			if(pb)
				pb.Population++;
			else
				body.GetComponent<CelestialBody>().AddPopulation(1);
		}
		else
		{
			Destroy(ship);
			travelTargets[body].Remove(ship);

			BuildSatellite(body.GetComponent<CelestialBody>());
		}
	}
}
