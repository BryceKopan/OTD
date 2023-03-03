using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechController : MonoBehaviour
{
	public bool techDebugging = false;

	private Technology selectedTechnology;
	public Technology SelectedTechnology
	{
		get { return selectedTechnology; }
		set
		{
			selectedTechnology = value;
			if(selectedTechnology != null)
			{
				selectedTechUI.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = selectedTechnology.name;
				selectedTechUI.transform.GetChild(1).GetComponent<UnityEngine.UI.Text>().text = selectedTechnology.ResearchProgress + "/" + selectedTechnology.researchCost;
			}
			else
			{
				selectedTechUI.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = "No Tech Selected";
				selectedTechUI.transform.GetChild(1).GetComponent<UnityEngine.UI.Text>().text = "NA"; 
			}
		}
	}

	private int settingTowerTech = 0;
	public GameObject selectedTower;
	public List<GameObject> allTech = new List<GameObject>();

	public GameObject researchTowerImage, selectedTechUI, techSlot1, techSlot2, techSlot3;

	List<Tower> towers = new List<Tower>();
	Technology[] laserTech = new Technology[3];
	Technology[] miningTech = new Technology[3];
	Technology[] sentryTech = new Technology[3];
	Technology[] railgunTech = new Technology[3];
	Technology[] mineTech = new Technology[3];
	Technology[] missleTech = new Technology[3];
	int laserCount = 0, miningCount = 0, sentryCount = 0, railgunCount = 0, mineCount = 0, missleCount = 0;

	private GameController GC;

	private void Start()
	{
		GC = FindObjectOfType<GameController>();
	}

	public void SetTowerTech1()
	{
		settingTowerTech = 1;
	}

	public void SetTowerTech2()
	{
		settingTowerTech = 2;
	}

	public void SetTowerTech3()
	{
		settingTowerTech = 3;
	}

	public void AddTower(Tower tower)
	{
		towers.Add(tower);

		
		if(tower is MiningTower)
		{
			miningCount++;
			foreach(Technology tech in miningTech)
				if(tech != null)
					tech.AddTechnologyTo(tower);
		}
		else if(tower is SentryTower || tower is Sentry)
		{
			sentryCount++;
			foreach(Technology tech in sentryTech)
				if(tech != null)
					tech.AddTechnologyTo(tower);
		}
		else if(tower is Railgun)
		{
			railgunCount++;
			foreach(Technology tech in railgunTech)
				if(tech != null)
					tech.AddTechnologyTo(tower);
		}
		else if(tower is MineLayer)
		{
			mineCount++;
			foreach(Technology tech in mineTech)
				if(tech != null)
					tech.AddTechnologyTo(tower);
		}
		else if(tower is MissleBattery)
		{
			missleCount++;
			foreach(Technology tech in missleTech)
				if(tech != null)
					tech.AddTechnologyTo(tower);
		}
		else if(tower is LaserTower)
		{
			laserCount++;
			foreach(Technology tech in laserTech)
				if(tech != null)
					tech.AddTechnologyTo(tower);
		}

		UpdateSelectedTechUI();
	}

	private void AddTech<T>(int index, Technology tech)
	{
		Technology[] techArray = new Technology[0];

		if(typeof(T) == typeof(MiningTower))
			techArray = miningTech;

		else if(typeof(T) == typeof(SentryTower) || typeof(T) == typeof(Sentry))
			techArray = sentryTech;

		else if(typeof(T) == typeof(Railgun))
			techArray = railgunTech;

		else if(typeof(T) == typeof(MineLayer))
			techArray = mineTech;

		else if(typeof(T) == typeof(MissleBattery))
			techArray = missleTech;

		else if(typeof(T) == typeof(LaserTower))
			techArray = laserTech;

		foreach(Tower tower in towers)
		{
			if(tower is T)
			{
				if(techArray[index] != null)
					tech.RemoveTechnologyFrom(tower);

				tech.AddTechnologyTo(tower);
			}
		}

		techArray[index] = tech;
	}

	private void RemoveTech<T>(int index, Technology tech)
	{
		foreach(Tower tower in towers)
		{
			if(tower is T)
			{
				tech.RemoveTechnologyFrom(tower);
			}
		}

		if(typeof(T) == typeof(LaserTower))
			laserTech[index] = null;

		if(typeof(T) == typeof(MiningTower))
			miningTech[index] = null;

		if(typeof(T) == typeof(SentryTower) || typeof(T) == typeof(Sentry))
			sentryTech[index] = null;

		if(typeof(T) == typeof(Railgun))
			railgunTech[index] = null;

		if(typeof(T) == typeof(MineLayer))
			mineTech[index] = null;

		if(typeof(T) == typeof(MissleBattery))
			missleTech[index] = null;
	}

	private void SetTowerSlot(Technology newTech)
	{
		int towerCount = 0;

		if(selectedTower.GetComponent<MissleBattery>())
		{
			towerCount = missleCount;
		}
		else if(selectedTower.GetComponent<MineLayer>())
		{
			towerCount = mineCount;
		}
		else if(selectedTower.GetComponent<Railgun>())
		{
			towerCount = railgunCount;
		}
		else if(selectedTower.GetComponent<SentryTower>())
		{
			towerCount = sentryCount;
		}
		else if(selectedTower.GetComponent<MiningTower>())
		{
			towerCount = miningCount;
		}
		else if(selectedTower.GetComponent<LaserTower>())
		{
			towerCount = laserCount;
		}

		if(GC.resources >= settingTowerTech * towerCount)
		{
			GameObject techSlot;
			if(settingTowerTech == 1)
				techSlot = techSlot1;
			else if(settingTowerTech == 2)
				techSlot = techSlot2;
			else
				techSlot = techSlot3;

			techSlot.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = newTech.techName;
			GC.AddResources(-settingTowerTech * towerCount);

			if(selectedTower.GetComponent<MissleBattery>())
			{
				AddTech<MissleBattery>(settingTowerTech - 1, newTech);
			}
			else if(selectedTower.GetComponent<MineLayer>())
			{
				AddTech<MineLayer>(settingTowerTech - 1, newTech);
			}
			else if(selectedTower.GetComponent<Railgun>())
			{
				AddTech<Railgun>(settingTowerTech - 1, newTech);
			}
			else if(selectedTower.GetComponent<SentryTower>() || selectedTower.GetComponent<Sentry>())
			{
				AddTech<SentryTower>(settingTowerTech - 1, newTech);
			}
			else if(selectedTower.GetComponent<MiningTower>())
			{
				AddTech<MiningTower>(settingTowerTech - 1, newTech);
			}
			else if(selectedTower.GetComponent<LaserTower>())
			{
				AddTech<LaserTower>(settingTowerTech - 1, newTech);
			}
		}
		settingTowerTech = 0;
	}

	private int GetSelectedTowerTypeCount()
	{
		int towerCount = 0;

		if(selectedTower != null)
			if(selectedTower.GetComponent<MissleBattery>())
			{
				towerCount = missleCount;
			}
			else if(selectedTower.GetComponent<MineLayer>())
			{
				towerCount = mineCount;
			}
			else if(selectedTower.GetComponent<Railgun>())
			{
				towerCount = railgunCount;
			}
			else if(selectedTower.GetComponent<SentryTower>())
			{
				towerCount = sentryCount;
			}
			else if(selectedTower.GetComponent<MiningTower>())
			{
				towerCount = miningCount;
			}
			else if(selectedTower.GetComponent<LaserTower>())
			{
				towerCount = laserCount;
			}

		return towerCount;
	}

	public void UpdateSelectedTechUI()
	{
		int towerCount = GetSelectedTowerTypeCount();

		techSlot1.transform.GetChild(2).GetComponent<UnityEngine.UI.Text>().text = "$" + towerCount * 1;
		techSlot2.transform.GetChild(2).GetComponent<UnityEngine.UI.Text>().text = "$" + towerCount * 2;
		techSlot3.transform.GetChild(2).GetComponent<UnityEngine.UI.Text>().text = "$" + towerCount * 3;
	}

	public void SelectTech(Technology newTech)
	{
		if(!newTech.isResearched)
		{
			SelectedTechnology = newTech;
			settingTowerTech = 0;
			if(techDebugging)
				SelectedTechnology.ResearchProgress = SelectedTechnology.researchCost;
		}
		else if(settingTowerTech > 0 && selectedTower != null)
			SetTowerSlot(newTech);
	}

	public void SelectRapidFire()
	{
		Technology newTech = allTech.Find(x => x.GetComponent<RapidFire>()).GetComponent<Technology>();
		SelectTech(newTech);
	}

	public void SelectLongRange()
	{
		Technology newTech = allTech.Find(x => x.GetComponent<LongRange>()).GetComponent<Technology>();
		SelectTech(newTech);
	}

	public void SelectPointDefense()
	{
		Technology newTech = allTech.Find(x => x.GetComponent<PointDefense>()).GetComponent<Technology>();
		SelectTech(newTech);
	}

	public void SelectAreaDefense()
	{
		Technology newTech = allTech.Find(x => x.GetComponent<AreaDefense>()).GetComponent<Technology>();
		SelectTech(newTech);
	}

	public void SelectTraining()
	{
		Technology newTech = allTech.Find(x => x.GetComponent<Training>()).GetComponent<Technology>();
		SelectTech(newTech);
	}

	public void SelectExpertise()
	{
		Technology newTech = allTech.Find(x => x.GetComponent<Expertise>()).GetComponent<Technology>();
		SelectTech(newTech);
	}

	public void SelectParallelFire()
	{
		Technology newTech = allTech.Find(x => x.GetComponent<ParallelFire>()).GetComponent<Technology>();
		SelectTech(newTech);
	}

	public void SelectBurstFire()
	{
		Technology newTech = allTech.Find(x => x.GetComponent<BurstFire>()).GetComponent<Technology>();
		SelectTech(newTech);
	}
}
