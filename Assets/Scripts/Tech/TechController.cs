using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechController : MonoBehaviour
{
	public Technology selectedTechnology;
	public List<GameObject> allTech = new List<GameObject>();

	List<Tower> towers = new List<Tower>();
	List<Technology> laserTech = new List<Technology>();
	List<Technology> miningTech = new List<Technology>();
	List<Technology> sentryTech = new List<Technology>();
	List<Technology> railgunTech = new List<Technology>();
	List<Technology> mineTech = new List<Technology>();
	List<Technology> missleTech = new List<Technology>();

	public void AddTower(Tower tower)
	{
		towers.Add(tower);

		if(tower is LaserTower)
			foreach(Technology tech in laserTech)
				tech.AddTechnologyTo(tower);

		if(tower is MiningTower)
			foreach(Technology tech in miningTech)
				tech.AddTechnologyTo(tower);

		if(tower is SentryTower)
			foreach(Technology tech in sentryTech)
				tech.AddTechnologyTo(tower);

		if(tower is Railgun)
			foreach(Technology tech in railgunTech)
				tech.AddTechnologyTo(tower);

		if(tower is MineLayer)
			foreach(Technology tech in mineTech)
				tech.AddTechnologyTo(tower);

		if(tower is MissleBattery)
			foreach(Technology tech in missleTech)
				tech.AddTechnologyTo(tower);
	}

	public void AddTech<T>(Technology tech)
	{
		foreach(Tower tower in towers)
		{
			if(tower is T)
			{
				tech.AddTechnologyTo(tower);
			}
		}

		if(typeof(T) == typeof(LaserTower))
			laserTech.Add(tech);

		if(typeof(T) == typeof(MiningTower))
			miningTech.Add(tech);

		if(typeof(T) == typeof(SentryTower))
			sentryTech.Add(tech);

		if(typeof(T) == typeof(Railgun))
			railgunTech.Add(tech);

		if(typeof(T) == typeof(MineLayer))
			mineTech.Add(tech);

		if(typeof(T) == typeof(MissleBattery))
			missleTech.Add(tech);
	}

	public void RemoveTech<T>(Technology tech)
	{
		foreach(Tower tower in towers)
		{
			if(tower is T)
			{
				tech.RemoveTechnologyFrom(tower);
			}
		}

		if(typeof(T) == typeof(LaserTower))
			laserTech.Remove(tech);

		if(typeof(T) == typeof(MiningTower))
			miningTech.Remove(tech);

		if(typeof(T) == typeof(SentryTower))
			sentryTech.Remove(tech);

		if(typeof(T) == typeof(Railgun))
			railgunTech.Remove(tech);

		if(typeof(T) == typeof(MineLayer))
			mineTech.Remove(tech);

		if(typeof(T) == typeof(MissleBattery))
			missleTech.Remove(tech);
	}

	public void SelectRapidFire()
	{
		selectedTechnology = allTech.Find(x => x.GetComponent<RapidFire>()).GetComponent<Technology>();
	}
}
