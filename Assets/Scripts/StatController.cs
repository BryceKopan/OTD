using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatController : MonoBehaviour
{
	public float seasonScoreAmount = 100f;
	public float researchScoreAmount = 5f;
	public float resourceScoreAmount = 5f;

	[SerializeField]
	private float score;
    public float Score
	{
		get { return score; }
		set { score = value;}
	}

	[SerializeField]
	private int season = 0;
	public int Season
	{
		get { return season; }
		set
		{
			Score += ((value - season) * seasonScoreAmount);
			season = value;
		}
	}

	[SerializeField]
	private float enemiesKilled;
	public float EnemiesKilled
	{
		get { return enemiesKilled; }
		set { enemiesKilled = value; }
	}

	[SerializeField]
	private float enemiesKilledScore;
	public float EnemiesKilledScore
	{
		get { return enemiesKilledScore; }
		set
		{
			Score += value - enemiesKilledScore;
			enemiesKilledScore = value;
		}
	}

	[SerializeField]
	private float resourcesGained;
	public float ResourcesGained
	{
		get { return resourcesGained; }
		set
		{
			Score += ((value - resourcesGained) * resourceScoreAmount);
			resourcesGained = value;
		}
	}

	[SerializeField]
	private float researchGained;
	public float ResearchGained
	{
		get { return researchGained; }
		set
		{
			Score += ((value - researchGained) * researchScoreAmount);
			researchGained = value;
		}
	}

	[SerializeField]
	private float populationGained;
	public float PopulationGained
	{
		get { return populationGained; }
		set { populationGained = value; }
	}

	[SerializeField]
	private float populationLost;
	public float PopulationLost
	{
		get { return populationLost; }
		set { populationLost = value; }
	}

	[SerializeField]
	private float towersBuilt;
	public float TowersBuilt
	{
		get { return towersBuilt; }
		set { towersBuilt = value; }
	}
}
