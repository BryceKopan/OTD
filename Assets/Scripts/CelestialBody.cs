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

	public bool isOrigin, isTerrestrial, isGaseous, hasWater, hasAtmosphere, hasMagnetosphere, isAsteroid, isComet;
}

public class CelestialBody : MonoBehaviour
{
	public BodyInfo info = new BodyInfo();

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
}
