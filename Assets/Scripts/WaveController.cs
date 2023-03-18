using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct WavePattern
{
	public int minSeason, maxSeason;
	public float enemyStrengthWeight;
	public float enemySpawnInterval;

	public List<SpawnPattern> spawnPatterns;
	public List<int> patternCycle;
}

[System.Serializable]
public struct SpawnPattern
{
	public int enemiesPerSpawn;
	public float angleBetweenEnemies;
	public float enemyStartAngle;
	public float enemyStartSpeed;
	public float enemyAcceleration;
	public float enemyTurnSpeed;
	public float enemyTurnAcceleration;
	public int[] shieldPerEnemy;
	public int[] sizePerEnemy;
	public bool[] spawnOnDeathPerEnemy;
	public bool[] spawningEnemyPerEnemy;
}

public class WaveController : MonoBehaviour
{
	public GameObject gatePrefab;
	public GameObject enemyPrefab;
	public float gateOrbitDistance;
	public int gateSeasonIncrement;
	public bool lockPattern, randomizePatternProgression;
	public int waveStrengthExponent = 4;

	public WavePattern currentWavePattern;

	public List<WavePattern> newWavePatterns;

	Dictionary<CelestialBody, List<Gate>> gates = new Dictionary<CelestialBody, List<Gate>>();

	GameController GC;

	private void Start()
	{
		GC = FindObjectOfType<GameController>();
	}

	public void StartWave()
	{
		List<PopulatedBody> populatedBodies = GC.populatedBodies;

		// look at all populated bodies and spawn initial gates
		for(int i = 0; i < populatedBodies.Count; i++)
		{
			if(populatedBodies[i].info.type != BodyType.Moon)
			{
				if(!gates.ContainsKey(populatedBodies[i]))
				{
					gates[populatedBodies[i]] = new List<Gate>();
					SpawnGate(populatedBodies[i]);
				}

				Gate firstGate = gates[populatedBodies[i]][0];
				if(GC.SC.Season - firstGate.originSeason >= gateSeasonIncrement * gates[populatedBodies[i]].Count)
				{
					SpawnGate(populatedBodies[i]);
				}
			}
		}

		//Decide wave strength and start waves
		foreach(List<Gate> gates in gates.Values)
		{
			int wavesSinceGateOpened = GC.SC.Season - gates[0].originSeason;
			int waveTotalStrength = GetWaveStrength(wavesSinceGateOpened);
			foreach(Gate gate in gates)
			{
				if(!lockPattern)
					currentWavePattern = GetRandomWavePattern();
				StartCoroutine(gate.SpawnWave(waveTotalStrength/gates.Count, currentWavePattern));
			}
		}
	}

	WavePattern GetRandomWavePattern()
	{
		bool foundPattern = false;
		WavePattern newPattern;

		do
		{
			int randomIndex = Random.Range(0, newWavePatterns.Count);
			newPattern = newWavePatterns[randomIndex];
			if((GC.SC.Season > newPattern.minSeason && GC.SC.Season < newPattern.maxSeason) || randomizePatternProgression)
				foundPattern = true;
			else if(GC.SC.Season > newPattern.maxSeason)
				newWavePatterns.RemoveAt(randomIndex);
		} while(!foundPattern);

		return newPattern;
	}

	int GetWaveStrength(int seasonsSinceFirstWave)
	{
		float enemyCount = 10;
		float enemyCountWaveScaler = 1 + Mathf.Pow(seasonsSinceFirstWave - 1, waveStrengthExponent)/100;
		float enemyCountRandomization = Random.Range(.75f, 1.25f);
		enemyCount *= enemyCountWaveScaler;
		enemyCount *= enemyCountRandomization;
		return Mathf.RoundToInt(enemyCount);
	}

	public void SpawnGate(CelestialBody body)
	{
		Gate newGate = Instantiate(gatePrefab, transform.position, transform.rotation).GetComponent<Gate>();
		gates[body].Add(newGate);

		Orbit newGateOrbit = newGate.GetComponent<Orbit>();
		newGateOrbit.principle = body.GetComponent<CelestialBody>().gameObject;
		newGateOrbit.axisVector = new Vector2(gateOrbitDistance, gateOrbitDistance);
		//newGateOrbit.SetupLine();

		Orbit firstGateOrbit = gates[body][0].GetComponent<Orbit>();

		float gatePlacementAngle = 360 / (gates[body].Count);

		for(int i = 0; i < gates[body].Count; i++)
		{
			Orbit orbit = gates[body][i].GetComponent<Orbit>();
			orbit.followOrbit = false;
			gates[body][i].transform.position = firstGateOrbit.GetPositionAt((gatePlacementAngle * i) + firstGateOrbit.GetCurrentAngle());
			orbit.RestartOrbit();
		}
	}
}
