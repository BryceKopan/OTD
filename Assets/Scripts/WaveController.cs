using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct WaveSettings
{
	public float gateDistance;
	public int wave;
	public int burstsPerWave;
	public int enemiesPerBurst;
	public float gateDistanceGrowth;
	public float waveCooldown;
	public float burstsCooldown;
	public float enemiesCooldown;
}

public class WaveController : MonoBehaviour
{
	public WaveSettings wavesSettings;

	public GameObject gatePrefab;
	public float gateOrbitDistance;
	Dictionary<Planet, List<Gate>> gates = new Dictionary<Planet, List<Gate>>();

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
		}
	}

	void SpawnGate(Planet planet)
	{
		Gate newGate = Instantiate(gatePrefab, transform.position, transform.rotation).GetComponent<Gate>();
		gates[planet].Add(newGate);

		Orbit newGateOrbit = newGate.GetComponent<Orbit>();
		newGateOrbit.principle = planet.GetComponent<CelestialBody>();
		newGateOrbit.axisVector = new Vector2(gateOrbitDistance, gateOrbitDistance);
		newGateOrbit.SetupLine();

		Orbit firstGateOrbit = gates[planet][0].GetComponent<Orbit>();

		float gatePlacementAngle = 360 / (gates[planet].Count + 1);

		for(int i = 0; i < gates[planet].Count; i++)
		{
			Orbit orbit = gates[planet][i].GetComponent<Orbit>();
			orbit.followOrbit = false;
			gates[planet][i].transform.position = firstGateOrbit.orbitPath.GetPositionOnEllipse((gatePlacementAngle * i) + firstGateOrbit.GetCurrentAngle());
			orbit.RestartOrbit();
		}
	}
}
