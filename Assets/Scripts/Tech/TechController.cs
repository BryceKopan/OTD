using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechController : MonoBehaviour
{
	public bool techDebugging = false;
	public List<GameObject> allTech = new List<GameObject>();

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
	private GameObject selectedTower;
	public GameObject SelectedTower
	{
		get { return selectedTower; }
		set
		{
			selectedTower = value;
			UpdateSlotedTechUI();
		}
	}

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
			if(tower is SentryTower)
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

		UpdateSlotedTechUI();
	}

	private void AddTech<T>(int index, Technology tech)
	{
		Technology[] techArray = new Technology[0];

		if(typeof(T) == typeof(MiningTower))
			techArray = miningTech;

		else if(typeof(T) == typeof(SentryTower))
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
			if(tower is T || (typeof(T) == typeof(SentryTower) && tower is Sentry))
			{
				if(techArray[index] != null)
					techArray[index].RemoveTechnologyFrom(tower);

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

		if(SelectedTower.GetComponent<MissleBattery>())
		{
			towerCount = missleCount;
		}
		else if(SelectedTower.GetComponent<MineLayer>())
		{
			towerCount = mineCount;
		}
		else if(SelectedTower.GetComponent<Railgun>())
		{
			towerCount = railgunCount;
		}
		else if(SelectedTower.GetComponent<SentryTower>())
		{
			towerCount = sentryCount;
		}
		else if(SelectedTower.GetComponent<MiningTower>())
		{
			towerCount = miningCount;
		}
		else if(SelectedTower.GetComponent<LaserTower>())
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

			if(SelectedTower.GetComponent<MissleBattery>())
			{
				AddTech<MissleBattery>(settingTowerTech - 1, newTech);
			}
			else if(SelectedTower.GetComponent<MineLayer>())
			{
				AddTech<MineLayer>(settingTowerTech - 1, newTech);
			}
			else if(SelectedTower.GetComponent<Railgun>())
			{
				AddTech<Railgun>(settingTowerTech - 1, newTech);
			}
			else if(SelectedTower.GetComponent<SentryTower>() || SelectedTower.GetComponent<Sentry>())
			{
				AddTech<SentryTower>(settingTowerTech - 1, newTech);
			}
			else if(SelectedTower.GetComponent<MiningTower>())
			{
				AddTech<MiningTower>(settingTowerTech - 1, newTech);
			}
			else if(SelectedTower.GetComponent<LaserTower>())
			{
				AddTech<LaserTower>(settingTowerTech - 1, newTech);
			}
		}
		settingTowerTech = 0;
	}

	private int GetSelectedTowerTypeCount()
	{
		int towerCount = 0;

		if(SelectedTower != null)
			if(SelectedTower.GetComponent<MissleBattery>())
			{
				towerCount = missleCount;
			}
			else if(SelectedTower.GetComponent<MineLayer>())
			{
				towerCount = mineCount;
			}
			else if(SelectedTower.GetComponent<Railgun>())
			{
				towerCount = railgunCount;
			}
			else if(SelectedTower.GetComponent<SentryTower>())
			{
				towerCount = sentryCount;
			}
			else if(SelectedTower.GetComponent<MiningTower>())
			{
				towerCount = miningCount;
			}
			else if(SelectedTower.GetComponent<LaserTower>())
			{
				towerCount = laserCount;
			}

		return towerCount;
	}

	public void UpdateSlotedTechUI()
	{
		int towerCount = GetSelectedTowerTypeCount();

		Technology[] techs = new Technology[0];
		if(SelectedTower != null)
		{
			if(SelectedTower.GetComponent<MissleBattery>())
			{
				techs = missleTech;
			}
			else if(SelectedTower.GetComponent<MineLayer>())
			{
				techs = mineTech;
			}
			else if(SelectedTower.GetComponent<Railgun>())
			{
				techs = railgunTech;
			}
			else if(SelectedTower.GetComponent<SentryTower>())
			{
				techs = sentryTech;
			}
			else if(SelectedTower.GetComponent<MiningTower>())
			{
				techs = miningTech;
			}
			else if(SelectedTower.GetComponent<LaserTower>())
			{
				techs = laserTech;
			}

			techSlot1.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = techs[0] ? techs[0].name : "Empty";
			techSlot2.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = techs[1] ? techs[1].name : "Empty";
			techSlot3.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = techs[2] ? techs[2].name : "Empty";

			techSlot1.transform.GetChild(2).GetComponent<UnityEngine.UI.Text>().text = "$" + towerCount * 1;
			techSlot2.transform.GetChild(2).GetComponent<UnityEngine.UI.Text>().text = "$" + towerCount * 2;
			techSlot3.transform.GetChild(2).GetComponent<UnityEngine.UI.Text>().text = "$" + towerCount * 3;
		}
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
		else if(settingTowerTech > 0 && SelectedTower != null)
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
