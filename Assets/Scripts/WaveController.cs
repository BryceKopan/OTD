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

	Dictionary<Planet, List<Gate>> gates = new Dictionary<Planet, List<Gate>>();

	GameController GC;

	private void Start()
	{
		GC = FindObjectOfType<GameController>();
	}

	public void StartWave()
	{
		Planet[] planets = FindObjectsOfType<Planet>();

		for(int i = 0; i < planets.Length; i++)
		{
			if(!gates.ContainsKey(planets[i]))
			{
				gates[planets[i]] = new List<Gate>();
				SpawnGate(planets[i]);
			}

			Gate firstGate = gates[planets[i]][0];
			if(GC.season - firstGate.originSeason >= gateSeasonIncrement * gates[planets[i]].Count)
			{
				SpawnGate(planets[i]);
			}
		}

		int enemyQuantity = GetEnemyQuantity();
		foreach(List<Gate> gates in gates.Values)
		{
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

	int GetEnemyQuantity()
	{
		float enemyCount = 15;
		float enemyCountWaveScaler = 1 + Mathf.Pow(GC.season - 1, 3)/100;
		float enemyCountRandomization = Random.Range(.75f, 1.25f);
		enemyCount *= enemyCountWaveScaler;
		enemyCount *= enemyCountRandomization;
		Debug.Log(enemyCount);
		return Mathf.RoundToInt(enemyCount);
	}

	public void SpawnGate(Planet planet)
	{
		Gate newGate = Instantiate(gatePrefab, transform.position, transform.rotation).GetComponent<Gate>();
		gates[planet].Add(newGate);

		Orbit newGateOrbit = newGate.GetComponent<Orbit>();
		newGateOrbit.principle = planet.GetComponent<CelestialBody>();
		newGateOrbit.axisVector = new Vector2(gateOrbitDistance, gateOrbitDistance);
		newGateOrbit.SetupLine();

		Orbit firstGateOrbit = gates[planet][0].GetComponent<Orbit>();

		float gatePlacementAngle = 360 / (gates[planet].Count);

		for(int i = 0; i < gates[planet].Count; i++)
		{
			Orbit orbit = gates[planet][i].GetComponent<Orbit>();
			orbit.followOrbit = false;
			gates[planet][i].transform.position = firstGateOrbit.orbitPath.GetPositionOnEllipse((gatePlacementAngle * i) + firstGateOrbit.GetCurrentAngle());
			orbit.RestartOrbit();
		}
	}
}
