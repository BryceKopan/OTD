using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BodyType
{
	Star,
	TerrestrialPlanet,
	GaseousPlanet,
	DwarfPlanet,
	Moon,
	Asteroid,
	Comet
}

[System.Serializable]
public struct BodyInfo
{
	public string displayName;
	public BodyType type;
	public float gravityRadius;
	public GameObject unlockedTowerPrefab;
	public List<GameObject> unlockedTechs;

	public bool isOrigin, isTerrestrial, isGaseous, hasWater, hasAtmosphere, hasMagnetosphere, isAsteroid, isComet, isExplored;
}

public class CelestialBody : MonoBehaviour
{
	public BodyInfo info = new BodyInfo();

	protected Dictionary<CelestialBody, GameObject> travelTargets = new Dictionary<CelestialBody, GameObject>();
	public float travelSpeed = 2.5f;

	private GameController gc;
	protected GameController GC
	{
		get
		{
			if(!gc)
				gc = FindObjectOfType<GameController>();
			return gc;
		}
		set { gc = value; }
	}

	private int habitability = -1;
	public int Habitability
	{
		get
		{
			if(habitability == -1)
			{
				habitability = 0;

				if(info.hasWater)
					habitability++;

				if(info.hasAtmosphere)
					habitability++;

				if(info.hasMagnetosphere)
					habitability++;

				if(info.isTerrestrial)
					habitability += 2;

				if(info.isGaseous)
					habitability -= 3;

				if(info.isOrigin)
					habitability++;

				if(info.type == BodyType.Asteroid)
					habitability--;

				if(info.type == BodyType.Comet)
					habitability--;

				if(info.type == BodyType.Star)
					habitability = 0;

				if(SavedData.saveData.hasEfficientArchitecureTalent)
					habitability = habitability * (5 / 4);
			}
			return habitability;
		}
		set	{ habitability = value; }
	}

	protected virtual void Start()
	{
		GC = FindObjectOfType<GameController>();
	}

	protected virtual void FixedUpdate()
	{
		Travel();
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

	protected virtual void TakeDamage(GameObject enemy)
	{
		
	}

	protected virtual void BodyDestroyed()
	{

	}

	public PopulatedBody AddPopulation(int population)
	{
		PopulatedBody pb = gameObject.AddComponent<PopulatedBody>();
		pb.info = info;
		pb.Habitability = Habitability;
		pb.maxPopulation = pb.Habitability;
		pb.Population += population;
		DestroyImmediate(this);

		return pb;
	}

	void Travel()
	{
		List<CelestialBody> completedTransfers = new List<CelestialBody>();

		foreach(CelestialBody cb in travelTargets.Keys)
		{
			travelTargets[cb].transform.position = Vector3.MoveTowards(travelTargets[cb].transform.position, cb.transform.position, Time.deltaTime * travelSpeed);
			travelTargets[cb].transform.rotation = Quaternion.LookRotation(cb.transform.position - travelTargets[cb].transform.position, new Vector3(0, 1, 0));

			if(travelTargets[cb].transform.position == cb.transform.position)
				completedTransfers.Add(cb);
			else if(Vector3.Distance(travelTargets[cb].transform.position, transform.position) > Vector3.Distance(travelTargets[cb].transform.position, cb.transform.position))
				travelTargets[cb].transform.parent = cb.transform;
		}

		foreach(CelestialBody cb in completedTransfers)
			FinishTransfer(cb);
	}

	public void StartSatelliteTravel(CelestialBody body)
	{
		GameObject travelShip = Instantiate(GC.satelliteTravelShipPrefab, transform.position, Quaternion.identity, transform);
		travelTargets[body] = travelShip;
	}

	public virtual void FinishTransfer(CelestialBody body)
	{
		Destroy(travelTargets[body]);
		travelTargets.Remove(body);

		BuildSatellite(body);
	}

	protected void BuildSatellite(CelestialBody body)
	{
		body.info.isExplored = true;

		GameObject newTower = Instantiate(GC.towers[6].prefab, transform.position, transform.rotation);
		Orbit towerOrbit = newTower.GetComponent<Orbit>();
		towerOrbit.principle = body.gameObject;
		float orbitDistance = body.GetComponent<SphereCollider>().radius + 1f;
		towerOrbit.axisVector = new Vector2(orbitDistance, orbitDistance);
		towerOrbit.followOrbit = false;
		towerOrbit.RestartOrbit();
	}

	protected void UnlockTechnology()
	{
		foreach(GameObject tech in info.unlockedTechs)
		{
			tech.GetComponent<Technology>().IsDiscovered = true;
		}
	}
}
