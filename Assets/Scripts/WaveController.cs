using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct WavePattern
{
	public float enemyQuantityWeight;
	public float enemySpawnInterval;
	public int enemiesPerSpawn;
	public float angleBetweenEnemies;
	public float enemyStartAngle;
	public float enemyStartSpeed;
	public float enemyAcceleration;
	public float enemyTurnSpeed;
	public float enemyTurnAcceleration;
}

public class WaveController : MonoBehaviour
{
	public GameObject gatePrefab;
	public float gateOrbitDistance;
	public int gateSeasonIncrement;
	public bool randomizePattern;

	public WavePattern currentWavePattern;

	public List<WavePattern> wavePatterns;

	Dictionary<CelestialBody, List<Gate>> gates = new Dictionary<CelestialBody, List<Gate>>();

	GameController GC;

	private void Start()
	{
		GC = FindObjectOfType<GameController>();
	}

	public void StartWave()
	{
		List<CelestialBody> populatedBodies = GC.populatedBodies;

		for(int i = 0; i < populatedBodies.Count; i++)
		{
			if(!gates.ContainsKey(populatedBodies[i]))
			{
				gates[populatedBodies[i]] = new List<Gate>();
				SpawnGate(populatedBodies[i]);
			}

			Gate firstGate = gates[populatedBodies[i]][0];
			if(GC.season - firstGate.originSeason >= gateSeasonIncrement * gates[populatedBodies[i]].Count)
			{
				SpawnGate(populatedBodies[i]);
			}
		}

		foreach(List<Gate> gates in gates.Values)
		{
			int wavesSinceGateOpened = GC.season - gates[0].originSeason;
			int enemyQuantity = GetEnemyQuantity(wavesSinceGateOpened);
			foreach(Gate gate in gates)
			{
				if(randomizePattern)
					currentWavePattern = GetRandomWavePattern();
				StartCoroutine(gate.SpawnWave(enemyQuantity/gates.Count, currentWavePattern));
			}
		}
	}

	WavePattern GetRandomWavePattern()
	{
		int randomIndex = Random.Range(0, wavePatterns.Count);
		return wavePatterns[randomIndex];
	}

	int GetEnemyQuantity(int waveNumber)
	{
		float enemyCount = 10;
		float enemyCountWaveScaler = 1 + Mathf.Pow(waveNumber - 1, 3)/100;
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
		newGateOrbit.principle = body.GetComponent<CelestialBody>();
		newGateOrbit.axisVector = new Vector2(gateOrbitDistance, gateOrbitDistance);
		newGateOrbit.SetupLine();

		Orbit firstGateOrbit = gates[body][0].GetComponent<Orbit>();

		float gatePlacementAngle = 360 / (gates[body].Count);

		for(int i = 0; i < gates[body].Count; i++)
		{
			Orbit orbit = gates[body][i].GetComponent<Orbit>();
			orbit.followOrbit = false;
			gates[body][i].transform.position = firstGateOrbit.orbitPath.GetPositionOnEllipse((gatePlacementAngle * i) + firstGateOrbit.GetCurrentAngle());
			orbit.RestartOrbit();
		}
	}
}
