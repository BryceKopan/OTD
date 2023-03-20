using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public struct TowerStruct
{
	public GameObject prefab;
	public bool isUnlocked;
}

public class GameController : MonoBehaviour
{
	public bool IS_DEBUGGING = false;

	public GameObject selectedObject;
	public CelestialBody lastSelectedCelestialBody;

	public WaveController WC;
	public TechController TC;
	public StatController SC;

	public float resources = 1f;

	public bool pause = true, skipToNextSeason = false;
	public bool showAllRanges = false;
	private float timeScale;

	public GameObject seasonCounter, resourceCounter, pauseSymbol, playSymbol, speedToggle, endGameUI, menu;
	int speedToggleValue = 1;

	public TowerStruct[] towers;
	GameObject unbuiltTower;
	bool isPlacementValid = true;
	Color originalColor;
	Orbit unbuiltTowerOrbit;

	public GameObject Sun, TargetPlanet;

	RaycastHit mouseHit;

	public float orbitIncrement, angleIncrement;

	public GameObject tooltip;
	public GameObject ark, arkButton;

	public List<GameObject> tabBodies = new List<GameObject>();
	public List<GameObject> tabs = new List<GameObject>();
	public List<PopulatedBody> populatedBodies = new List<PopulatedBody>(); 

	public GameObject totalHealth, planetDetail, planetDetailName, planetDetailPopulation, planetDetailResourcers, planetDetailResearchers, planetDetailTTPG, planetDetailTransferMessage;
	public GameObject planetDetailTransferButton, planetDetailSatelliteButton, planetDetailArkButton, planetDetailPopulated, planetDetailUnpopulated, planetDetailUnlocks, planetDetailSatelliteSent;
	public GameObject researchPanel;
	public OrbitalMenu orbitalMenu;
	public Canvas canvas;

	public Text postGameScore, postGameSeason, postGameEnemyScore, postGameResources, postGameResearch, postGameEnemiesKilled, postGamePopGain, postGamePopLost, postGameTowersBuilt;

	private bool isTransferingPopulation = false, isSelectingTower = false;

	public float travelTimeScalar = 1f;
	public GameObject populationTransferShipPrefab, satelliteTravelShipPrefab;

	// Start is called before the first frame update
	void Start()
    {
		SavedData.IS_DEBUGGING = IS_DEBUGGING;
		SavedData.LoadFile();

		SetOriginPlanet();

		resourceCounter.GetComponent<Text>().text = "Resources: " + resources;
		populatedBodies = GetPopulatedBodies();

		Camera.main.transform.parent = populatedBodies[0].transform;

		if(pause)
		{
			pause = false;
			Pause();
		}

		ApplyGameModes();
		ApplyTalents();
	}

    // Update is called once per frame
    void Update()
    {
		if(skipToNextSeason)
			Time.timeScale += .1f;

		if(lastSelectedCelestialBody != null)
			UpdateUI();

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if(Physics.Raycast(ray, out mouseHit, Mathf.Infinity, -1, QueryTriggerInteraction.Ignore))
		{
			RangeToolTip rToolTip = mouseHit.transform.GetComponentInChildren<RangeToolTip>();
			if(rToolTip)
				rToolTip.indicatorIsActive = true;

			ToolTip toolTip = mouseHit.transform.GetComponent<ToolTip>();
			if(toolTip && toolTip.enabled)
				toolTip.ActivateTooltip();
		}
		
		if(unbuiltTower != null && !researchPanel.activeSelf)
		{
			Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.y));
			mousePosition.y = 0;
			unbuiltTower.transform.position = mousePosition;
			float orbitSize = Vector3.Distance(unbuiltTower.transform.position, unbuiltTowerOrbit.principle.transform.position);

			float orbitOffset = orbitSize % orbitIncrement;
			if(orbitOffset >= orbitIncrement / 2)
				orbitSize += orbitIncrement - orbitOffset;
			else
				orbitSize -= orbitOffset;

			unbuiltTowerOrbit.axisVector = new Vector2(orbitSize, orbitSize);
			unbuiltTowerOrbit.principle = GetClosestCelestialBody(unbuiltTower).gameObject;
			float angle = unbuiltTowerOrbit.GetCurrentAngle();
			float angleOffset = angle % angleIncrement;

			if(angleOffset >= angleIncrement / 2)
				angle += angleIncrement - angleOffset;
			else
				angle -= angleOffset;

			unbuiltTower.transform.position = unbuiltTowerOrbit.GetPositionAt(angle);

			isPlacementValid = true;
			Vector3 halfExtents = unbuiltTower.GetComponent<BoxCollider>().size/2;
			Collider[] colliders = Physics.OverlapBox(unbuiltTower.transform.position, halfExtents, Quaternion.identity, ~0, QueryTriggerInteraction.Ignore);
			foreach(Collider collider in colliders)
			{
				if(collider.gameObject != unbuiltTower)
				{
					isPlacementValid = false;
				}
			}

			CelestialBody closestBody = GetClosestCelestialBody(unbuiltTower);
			if(!closestBody.info.isExplored)
				isPlacementValid = false;

			if(isPlacementValid)
				unbuiltTower.GetComponentInChildren<SpriteRenderer>().color = originalColor;
			else
				unbuiltTower.GetComponentInChildren<SpriteRenderer>().color = Color.red;
		}
		else if(unbuiltTower != null)
		{
			Image image = TC.researchTowerImage.GetComponent<Image>();
			SpriteRenderer sr = unbuiltTower.GetComponentInChildren<SpriteRenderer>();
			image.sprite = sr.sprite;
			image.color = sr.color;
			if(TC.SelectedTower != null)
				Destroy(TC.SelectedTower);
			TC.SelectedTower = unbuiltTower;
			unbuiltTower.SetActive(false);
			unbuiltTower = null;
			TC.UpdateSlotedTechUI();
		}
	}

	public void Pause()
	{
		if(!pause)
		{
			pause = true;
			timeScale = Time.timeScale;
			Time.timeScale = 0f;

			pauseSymbol.SetActive(true);
			playSymbol.SetActive(false);
		}
		else if(unbuiltTower == null)
		{
			pause = false;
			Time.timeScale = timeScale;

			pauseSymbol.SetActive(false);
			playSymbol.SetActive(true);
		}
	}

	public void AddResources(float r)
	{
		resources += r;
		resourceCounter.GetComponent<UnityEngine.UI.Text>().text = "Resources: " + resources;
		if(r > 0)
			SC.ResourcesGained += r;
	}

	private void BuildObject(GameObject prefab)
	{
		if(!pause)
			Pause();

		if(unbuiltTower)
			Destroy(unbuiltTower);

		unbuiltTower = Instantiate(prefab, transform.position, transform.rotation);
		unbuiltTowerOrbit = unbuiltTower.GetComponent<Orbit>();
		unbuiltTowerOrbit.followOrbit = false;
		unbuiltTowerOrbit.principle = GetClosestCelestialBody(unbuiltTower).gameObject;
		originalColor = unbuiltTower.GetComponentInChildren<SpriteRenderer>().color;

		ToolTip tt = unbuiltTower.GetComponent<ToolTip>();
		if(tt)
			tt.enabled = false;

		orbitalMenu.IsSelectingTower = false;
		tooltip.SetActive(false);
	}

	public void BuildLaserTower()
	{
		if(resources >= towers[0].prefab.GetComponent<Tower>().resourceCost && towers[0].isUnlocked)
			BuildObject(towers[0].prefab);
	}

	public void BuildMiningTower()
	{
		if(resources >= towers[5].prefab.GetComponent<Tower>().resourceCost && towers[5].isUnlocked)
			BuildObject(towers[5].prefab);
	}

	public void BuildSentry()
	{
		if(resources >= towers[2].prefab.GetComponent<Tower>().resourceCost && towers[2].isUnlocked)
			BuildObject(towers[2].prefab);
	}

	public void BuildRailgun()
	{
		if(resources >= towers[1].prefab.GetComponent<Tower>().resourceCost && towers[1].isUnlocked)
			BuildObject(towers[1].prefab);
	}

	public void BuildMissleBattery()
	{
		if(resources >= towers[3].prefab.GetComponent<Tower>().resourceCost && towers[3].isUnlocked)
			BuildObject(towers[3].prefab);
	}

	public void BuildMineLayer()
	{
		if(resources >= towers[4].prefab.GetComponent<Tower>().resourceCost && towers[4].isUnlocked)
			BuildObject(towers[4].prefab);
	}

	public void BuildArk()
	{
		if(resources > 100)
		{
			ark.SetActive(true);
			resources -= 100;
		}
	}

	public void LeftClick(InputAction.CallbackContext context)
	{
		if(context.started)
		{
			if(unbuiltTower != null && isPlacementValid)
			{
				Tower t = unbuiltTower.GetComponent<Tower>();
				if(t)
				{
					TC.AddTower(unbuiltTower.GetComponent<Tower>());
					AddResources(-t.resourceCost);
					SC.TowersBuilt++;
				}

				unbuiltTowerOrbit.RestartOrbit();
				unbuiltTower.GetComponent<ToolTip>().enabled = true;
				CelestialBody cb = GetClosestPopulatedBody(unbuiltTower);
				t.StartTravel(cb.gameObject);
				unbuiltTower = null;
			}
			else if(unbuiltTower != null && !isPlacementValid)
			{
				Debug.Log("Invalid Placement");
			}
			else if(mouseHit.transform)
			{
				GameObject obj = mouseHit.transform.gameObject;
				if(obj)
				{
					Select(obj);
				}
			}
			else
			{
				Select(null);
			}
		}
	}

	public void RightClick(InputAction.CallbackContext context)
	{
		if(context.started)
		{
			if(unbuiltTower != null)
			{
				Destroy(unbuiltTower);
				unbuiltTower = null;
			}
			else if(isTransferingPopulation)
			{
				isTransferingPopulation = false;
				planetDetailTransferMessage.SetActive(false);
			}
			else if(orbitalMenu.IsSelectingTower)
			{
				orbitalMenu.IsSelectingTower = false;
			}
			else
			{
				orbitalMenu.OpenTowerMenu();
			}
		}
	}

	private CelestialBody GetClosestCelestialBody(GameObject origin)
	{
		CelestialBody[] bodies = FindObjectsOfType<CelestialBody>();
		CelestialBody closest = FindObjectOfType<Sun>().GetComponent<CelestialBody>();

		foreach(CelestialBody body in bodies)
		{
			float currentDistance = Vector3.Distance(origin.transform.position, closest.transform.position);
			float newDistance = Vector3.Distance(origin.transform.position, body.transform.position);

			if(newDistance <  currentDistance && newDistance < body.info.gravityRadius)
				closest = body;
		}

		return closest;
	}

	private PopulatedBody GetClosestPopulatedBody(GameObject origin)
	{
		PopulatedBody[] bodies = FindObjectsOfType<PopulatedBody>();
		PopulatedBody closest = populatedBodies[0];

		foreach(PopulatedBody body in bodies)
		{
			float currentDistance = Vector3.Distance(origin.transform.position, closest.transform.position);
			float newDistance = Vector3.Distance(origin.transform.position, body.transform.position);

			if(newDistance < currentDistance && newDistance < body.info.gravityRadius)
				closest = body;
		}

		return closest;
	}

	public void RestartGame()
	{
		Scene scene = SceneManager.GetActiveScene();
		SceneManager.LoadScene(scene.name);
	}

	public void ToggleRanges(InputAction.CallbackContext context)
	{
		if(context.started)
			showAllRanges = true;
		else if(context.canceled)
			showAllRanges = false;
	}

	private void Select(GameObject obj)
	{
		if(obj == null)
		{
			selectedObject = null;
			return;
		}

		if(obj.GetComponent<RangeToolTip>())
			obj = obj.transform.parent.gameObject;
		
		if(obj.GetComponent<CelestialBody>())
		{
			if(isTransferingPopulation)
			{
				isTransferingPopulation = false;
				planetDetailTransferMessage.SetActive(false);

				((PopulatedBody)lastSelectedCelestialBody).StartPopulationTransfer(obj.GetComponent<CelestialBody>());
			}

			Camera.main.transform.position = new Vector3(obj.transform.position.x, Camera.main.transform.position.y, obj.transform.position.z);
			Camera.main.transform.parent = obj.transform;

			if(lastSelectedCelestialBody == obj.GetComponent<CelestialBody>())
			{
				ShowPlanetDetails();
			}
			else
			{
				planetDetail.SetActive(false);
			}

			lastSelectedCelestialBody = obj.GetComponent<CelestialBody>();
		}

		selectedObject = obj;
	}

	public void DeleteSelectedTower()
	{
		ToolTipWindow toolTipWindow = tooltip.GetComponent<ToolTipWindow>();
		Tower tower = toolTipWindow.currentToolTip.GetComponent<Tower>();
		if(tower)
		{
			toolTipWindow.gameObject.SetActive(false);
			Destroy(tower.gameObject);
		}
	}

	public void StartNextSeason()
	{
		if(skipToNextSeason)
		{
			skipToNextSeason = false;
			SetSpeed(speedToggleValue);
		}

		SC.Season++;
		seasonCounter.GetComponent<Text>().text = "Season: " + SC.Season;

		if((SC.Season > 1 && !SavedData.saveData.isQuickStartMode) || (SC.Season > 8 && SavedData.saveData.isQuickStartMode))
			for(int i = 0; i < populatedBodies.Count; i++)
			{
				populatedBodies[i].IncrementSeason();
			}

		WC.StartWave();
	}

	public void SelectPlanet1()
	{
		Select(tabBodies[0]);
	}

	public void SelectPlanet2()
	{
		Select(tabBodies[1]);
	}

	public void SelectPlanet3()
	{
		Select(tabBodies[2]);
	}

	public void SelectPlanet4()
	{
		Select(tabBodies[3]);
	}

	public void ShowPlanetDetails()
	{
		if(!planetDetail.activeSelf)
		{
			planetDetail.SetActive(true);
			Text nameText = planetDetailName.GetComponent<Text>();
			nameText.text = lastSelectedCelestialBody.info.displayName;
			nameText.color = lastSelectedCelestialBody.transform.GetChild(0).GetComponent<SpriteRenderer>().color;

			if(lastSelectedCelestialBody.info.isOrigin)
			{
				planetDetailArkButton.SetActive(true);
			}
			else
			{
				planetDetailArkButton.SetActive(false);
			}

			if(lastSelectedCelestialBody is PopulatedBody)
			{
				planetDetailPopulated.SetActive(true);
				planetDetailUnpopulated.SetActive(false);
				planetDetailTransferButton.SetActive(true);
			}
			else
			{
				planetDetailPopulated.SetActive(false);
				planetDetailUnpopulated.SetActive(true);
				planetDetailTransferButton.SetActive(false);
				planetDetailUnpopulated.transform.GetChild(0).GetComponent<Text>().text = "Max pop: " + lastSelectedCelestialBody.Habitability;
			}

			if(lastSelectedCelestialBody.info.isExplored)
				planetDetailSatelliteButton.SetActive(false);
			else
				planetDetailSatelliteButton.SetActive(true);

			if(lastSelectedCelestialBody.info.unlockedTowerPrefab || lastSelectedCelestialBody.info.unlockedTechs.Count > 0)
			{
				planetDetailUnlocks.SetActive(true);
				if(lastSelectedCelestialBody.info.unlockedTowerPrefab)
					planetDetailUnlocks.transform.GetChild(1).GetComponent<Text>().text = lastSelectedCelestialBody.info.unlockedTowerPrefab.GetComponent<Tower>().displayName;
				for(int i = 0; i < lastSelectedCelestialBody.info.unlockedTechs.Count; i++)
				{
					if(i == 0)
						planetDetailUnlocks.transform.GetChild(2).GetComponent<Text>().text = lastSelectedCelestialBody.info.unlockedTechs[i].GetComponent<Technology>().techName;
					else
						planetDetailUnlocks.transform.GetChild(2).GetComponent<Text>().text += "\n" + lastSelectedCelestialBody.info.unlockedTechs[i].GetComponent<Technology>().techName;
				}
			}
			else
				planetDetailUnlocks.SetActive(false);
		}
		else
		{
			planetDetail.SetActive(false);
		}
	}

	public List<PopulatedBody> GetPopulatedBodies()
	{
		return new List<PopulatedBody>(FindObjectsOfType<PopulatedBody>());
	}

	public void UpdateUI()
	{
		int health = 0;

		foreach(PopulatedBody pb in populatedBodies)
		{
			health += pb.Population;
		}

		totalHealth.GetComponent<Text>().text = "Health: " + health;

		PopulatedBody lastSelectedPopulatedBody = lastSelectedCelestialBody.GetComponent<PopulatedBody>();
		if(lastSelectedPopulatedBody)
		{
			planetDetailPopulation.GetComponent<Text>().text = "Population: " + lastSelectedPopulatedBody.Population + " / " + lastSelectedPopulatedBody.maxPopulation;
			planetDetailTTPG.GetComponent<Text>().text = "Time to population growth: " + lastSelectedPopulatedBody.timeToPopulationGrowth;
			planetDetailResourcers.GetComponent<Text>().text = "Resource Gatherers: " + lastSelectedPopulatedBody.resourcers;
			planetDetailResearchers.GetComponent<Text>().text = "Researchers: " + lastSelectedPopulatedBody.researchers;
		}

		arkButton.GetComponent<Button>().interactable = resources >= 100;

		//Tech UI
		if(TC.SelectedTechnology)
			TC.selectedTechUI.transform.GetChild(1).GetComponent<UnityEngine.UI.Text>().text = TC.SelectedTechnology.ResearchProgress + "/" + TC.SelectedTechnology.researchCost;

		for(int i = 0; i < tabs.Count; i++)
		{
			if(tabBodies[i].GetComponent<PopulatedBody>())
				tabs[i].transform.GetChild(2).GetComponent<Text>().text = "Pop: " + tabBodies[i].GetComponent<PopulatedBody>().Population;
		}
	}

	public void SetBodyTab(GameObject bodyTab, GameObject celestialBody)
	{
		if(celestialBody == null)
		{
			bodyTab.transform.GetChild(1).GetComponent<UnityEngine.UI.Image>().color = Color.black;
			bodyTab.transform.GetChild(2).gameObject.SetActive(false);
		}
		else
		{
			bodyTab.transform.GetChild(1).GetComponent<UnityEngine.UI.Image>().color = celestialBody.GetComponent<UnityEngine.UI.Image>().color;
			bodyTab.transform.GetChild(2).gameObject.SetActive(true);
		}
	}

	public void IncreaseSelectedCBResourcers()
	{
		((PopulatedBody)lastSelectedCelestialBody).IncreaseResourcers();
	}

	public void IncreaseSelectedCBResearchers()
	{
		((PopulatedBody)lastSelectedCelestialBody).IncreaseResearchers();
	}

	public void TransferPopulation()
	{
		planetDetailTransferMessage.SetActive(true);
		isTransferingPopulation = true;
	}

	public void BuildSatellite()
	{
		AddResources(-2);
		PopulatedBody pb = GetClosestPopulatedBody(lastSelectedCelestialBody.gameObject);
		pb.StartSatelliteTravel(lastSelectedCelestialBody);
	}

	public void ShowResearchPanel()
	{
		if(researchPanel.activeSelf)
			researchPanel.SetActive(false);
		else
			researchPanel.SetActive(true);
	}

	public void SkipToNextSeason()
	{
		skipToNextSeason = !skipToNextSeason;
		if(!skipToNextSeason)
		{
			Time.timeScale = 1;
			Pause();
		}
		else if(pause)
			Pause();
	}

	public void ToggleSpeed()
	{
		speedToggleValue++;
		if(speedToggleValue > 3)
			speedToggleValue = 1;

		SetSpeed(speedToggleValue);
	}

	public void SetSpeed(int speed)
	{
		switch(speed)
		{
			case 1:
				Time.timeScale = 1;
				speedToggle.transform.GetChild(1).GetComponent<Image>().color = Color.black;
				speedToggle.transform.GetChild(2).GetComponent<Image>().color = Color.black;
				break;
			case 2:
				Time.timeScale = 2;
				speedToggle.transform.GetChild(1).GetComponent<Image>().color = Color.white;
				speedToggle.transform.GetChild(2).GetComponent<Image>().color = Color.black;
				break;
			case 3:
				Time.timeScale = 3;
				speedToggle.transform.GetChild(1).GetComponent<Image>().color = Color.white;
				speedToggle.transform.GetChild(2).GetComponent<Image>().color = Color.white;
				break;
		}
	}

	public void EndGame()
	{
		if(!GetComponent<MainMenuController>() && !SavedData.IS_DEBUGGING)
		{
			endGameUI.SetActive(true);

			postGameScore.text = ((int) SC.Score).ToString();
			postGameSeason.text = "Seasons Survived: " + SC.Season;
			postGameEnemyScore.text = "Enemies Killed Score: " + (int) SC.EnemiesKilledScore;
			postGameResources.text = "Resources Gained: " + SC.ResourcesGained;
			postGameResearch.text = "Research Gained: " + SC.ResearchGained;
			postGameEnemiesKilled.text = "Enemies Killed: " + SC.EnemiesKilled;
			postGamePopGain.text = "Population Gained: " + SC.PopulationGained;
			postGamePopLost.text = "Population Lost: " + SC.PopulationLost;
			postGameTowersBuilt.text = "Towers Built: " + SC.TowersBuilt;
		}
	}

	public void LoadMainMenu()
	{
		Time.timeScale = 1;
		SceneManager.LoadScene(0);
	}

	public void UnlockTower(GameObject towerPrefab)
	{
		for(int i = 0; i < towers.Length; i++)
		{
			if(towers[i].prefab == towerPrefab)
				towers[i].isUnlocked = true;
		}
	}

	public void UpdatePopulationXP(int score)
	{
		SavedData.saveData.currentPopXP += score;

		if(SavedData.saveData.currentPopXP >= 500 * Mathf.Pow(2, SavedData.saveData.popLevel))
		{
			SavedData.saveData.popLevel++;
			SavedData.saveData.unspentTalentPoints++;
			SavedData.saveData.currentPopXP = 0;
			SavedData.SaveFile();
			Debug.Log("LEvel Up!");
		}
	}

	private void ApplyGameModes()
	{
		if(SavedData.saveData.isQuickStartMode)
		{
			SC.Season = 7;
			populatedBodies[0].Population += 3;
			AddResources(28);
			seasonCounter.GetComponent<Text>().text = "Season: " + SC.Season;
		}

		if(SavedData.saveData.isWildPatternsMode)
			WC.randomizePatternProgression = true;

		if(SavedData.saveData.isHardMode)
			WC.waveStrengthExponent++;
	}

	private void ApplyTalents()
	{
		if(SavedData.saveData.hasPlannedWorldTalent)
		{

			populatedBodies[0].maxPopulation = (populatedBodies[0].maxPopulation * 5 / 4);

		}

		if(SavedData.saveData.hasGrowthVatsTalent)
		{
			populatedBodies[0].populationGrowthTime--;
			populatedBodies[0].timeToPopulationGrowth = populatedBodies[0].populationGrowthTime;
		}

		if(SavedData.saveData.hasLaserDefense1Talent && towers.Length > 0)
		{
			int numberOfTurrets = 0;
			if(SavedData.saveData.hasLaserDefense1Talent)
				numberOfTurrets++;
			if(SavedData.saveData.hasLaserDefense2Talent)
				numberOfTurrets++;
			if(SavedData.saveData.hasLaserDefense3Talent)
				numberOfTurrets++;

			float towerPlacementAngle = 360 / numberOfTurrets;

			for(int i = 0; i < numberOfTurrets; i++)
			{
				GameObject newTower = Instantiate(towers[0].prefab, transform.position, transform.rotation);
				Orbit towerOrbit = newTower.GetComponent<Orbit>();
				towerOrbit.principle = populatedBodies[0].gameObject;
				float orbitDistance = populatedBodies[0].GetComponent<SphereCollider>().radius + 1f;
				towerOrbit.axisVector = new Vector2(orbitDistance, orbitDistance);
				towerOrbit.followOrbit = false;
				newTower.transform.position = towerOrbit.GetPositionAt(towerPlacementAngle * i);
				towerOrbit.RestartOrbit();
			}
		}

		if(SavedData.saveData.hasIonEngineTalent)
		{
			travelTimeScalar *= 5f / 4f;
		}

		if(SavedData.saveData.hasOrionDriveTalent)
		{
			travelTimeScalar *= 2f;
		}

		if(SavedData.saveData.hasPopulatedPlanetTalent)
		{
			populatedBodies[0].Population++;
		}

		if(SavedData.saveData.hasMoonBaseTalent)
		{
			CelestialBody[] cbs = FindObjectsOfType<CelestialBody>();
			CelestialBody closestMoon = null;
			foreach(CelestialBody cb in cbs)
			{
				if(cb.info.type == BodyType.Moon && cb.GetComponent<Orbit>().principle == populatedBodies[0].gameObject)
				{
					if(closestMoon == null || Vector3.Distance(cb.transform.position, populatedBodies[0].transform.position) < Vector3.Distance(closestMoon.transform.position, populatedBodies[0].transform.position))
					{
						closestMoon = cb;
					}
				}
			}

			closestMoon.AddPopulation(1);
		}
	}

	private void SetOriginPlanet()
	{
		List<PopulatedBody> popPlanets = GetPopulatedBodies();

		if(SavedData.saveData.originIsEarth && popPlanets.Count == 0)
		{
			tabBodies[2].GetComponent<CelestialBody>().info.isOrigin = true;
			PopulatedBody pb = tabBodies[2].GetComponent<CelestialBody>().AddPopulation(3);
			pb.timeToPopulationGrowth = 4;
			Select(pb.gameObject);
		}
		else if(SavedData.saveData.hasTerraformingTalent && SavedData.saveData.originIsMars && popPlanets.Count == 0)
		{
			tabBodies[3].GetComponent<CelestialBody>().info.isOrigin = true;
			PopulatedBody pb = tabBodies[3].GetComponent<CelestialBody>().AddPopulation(3);
			pb.timeToPopulationGrowth = 4;
			Select(pb.gameObject);
		}
	}

	public void ToggleMenu()
	{
		GameObject m = menu.transform.GetChild(0).gameObject;
		m.SetActive(!m.activeSelf);
	}

	public void QuitGame()
	{
		Application.Quit();
	}
}
