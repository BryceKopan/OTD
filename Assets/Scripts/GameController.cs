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

	private WaveController WC;
	private TechController TC;
	public int season = 0;

	public float resources = 1f;

	public bool pause = true, skipToNextSeason = false;
	public bool showAllRanges = false;
	private float timeScale;

	public GameObject seasonCounter, resourceCounter, pauseSymbol, playSymbol, endGameUI;

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
	public List<CelestialBody> populatedBodies = new List<CelestialBody>(); 

	public GameObject totalHealth, planetDetail, planetDetailName, planetDetailPopulation, planetDetailResourcers, planetDetailResearchers, planetDetailTTPG, planetDetailTransferMessage;
	public GameObject planetDetailArkButton, planetDetailPopulated, planetDetailUnpopulated, planetDetailUnlocks;
	public GameObject researchPanel;
	public OrbitalMenu orbitalMenu;
	public Canvas canvas;

	private bool isTransferingPopulation = false, isSelectingTower = false;

	// Start is called before the first frame update
	void Start()
    {
		resourceCounter.GetComponent<UnityEngine.UI.Text>().text = "Resources: " + resources;
		WC = FindObjectOfType<WaveController>();
		TC = FindObjectOfType<TechController>();
		populatedBodies = GetPopulatedBodies();

		Camera.main.transform.parent = populatedBodies[0].transform;

		for(int i = 0; i < tabs.Count; i++)
		{
			if(!tabBodies[i].GetComponent<CelestialBody>().isPopulated)
			{
				//tabBodies[i].GetComponent<CelestialBody>().IsExplored = false;
				//SetBodyTab(tabs[i], null);
			}
		}

		if(pause)
		{
			pause = false;
			Pause();
		}
	}

    // Update is called once per frame
    void Update()
    {
		if(skipToNextSeason)
			Time.timeScale += .1f;

		UpdateUI();

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if(Physics.Raycast(ray, out mouseHit, Mathf.Infinity, -1, QueryTriggerInteraction.Ignore))
		{
			RangeToolTip rToolTip = mouseHit.transform.GetComponentInChildren<RangeToolTip>();
			if(rToolTip)
				rToolTip.indicatorIsActive = true;

			ToolTip toolTip = mouseHit.transform.GetComponentInChildren<ToolTip>();
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
			unbuiltTowerOrbit.principle = GetClosestCelestialBody(unbuiltTower);
			if(unbuiltTowerOrbit.orbitPath)
			{
				float angle = unbuiltTowerOrbit.GetCurrentAngle();
				float angleOffset = angle % angleIncrement;

				if(angleOffset >= angleIncrement / 2)
					angle += angleIncrement - angleOffset;
				else
					angle -= angleOffset;

				unbuiltTower.transform.position = unbuiltTowerOrbit.orbitPath.GetPositionOnEllipse(angle);
			}

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

			if(isPlacementValid)
				unbuiltTower.GetComponentInChildren<SpriteRenderer>().color = originalColor;
			else
				unbuiltTower.GetComponentInChildren<SpriteRenderer>().color = Color.red;
		}
		else if(unbuiltTower != null)
		{
			UnityEngine.UI.Image image = TC.researchTowerImage.GetComponent<UnityEngine.UI.Image>();
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
		unbuiltTowerOrbit.principle = GetClosestCelestialBody(unbuiltTower);
		originalColor = unbuiltTower.GetComponentInChildren<SpriteRenderer>().color;

		ToolTip tt = unbuiltTower.GetComponent<ToolTip>();
		if(tt)
			tt.enabled = false;

		orbitalMenu.IsSelectingTower = false;
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
				}

				unbuiltTowerOrbit.RestartOrbit();
				unbuiltTower.GetComponent<ToolTip>().enabled = true;
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

			if(newDistance <  currentDistance && newDistance < body.gravityRadius)
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

				lastSelectedCelestialBody.TransferPopulationTo(obj.GetComponent<CelestialBody>());
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
			Time.timeScale = 1;
					skipToNextSeason = false;
		}

		season++;
		seasonCounter.GetComponent<UnityEngine.UI.Text>().text = "Season: " + season;

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
			nameText.text = lastSelectedCelestialBody.displayName;
			nameText.color = lastSelectedCelestialBody.transform.GetChild(0).GetComponent<SpriteRenderer>().color;

			if(lastSelectedCelestialBody.isOrigin)
			{
				planetDetailArkButton.SetActive(true);
			}
			else
			{
				planetDetailArkButton.SetActive(false);
			}

			if(lastSelectedCelestialBody.isPopulated)
			{
				planetDetailPopulated.SetActive(true);
				planetDetailUnpopulated.SetActive(false);
			}
			else
			{
				planetDetailPopulated.SetActive(false);
				planetDetailUnpopulated.SetActive(true);
				planetDetailUnpopulated.transform.GetChild(0).GetComponent<Text>().text = "Max pop: " + lastSelectedCelestialBody.maxPopulation; 
			}
		}
		else
		{
			planetDetail.SetActive(false);
		}
	}

	public List<CelestialBody> GetPopulatedBodies()
	{
		CelestialBody[] cBodies = FindObjectsOfType<CelestialBody>();
		List<CelestialBody> populatedCBs = new List<CelestialBody>();

		foreach(CelestialBody body in cBodies)
		{
			if(body.isPopulated)
				populatedCBs.Add(body);
		}

		return populatedCBs;
	}

	public void UpdateUI()
	{
		int health = 0;

		foreach(CelestialBody cb in populatedBodies)
		{
			health += cb.Population;
		}

		totalHealth.GetComponent<UnityEngine.UI.Text>().text = "Health: " + health;
		planetDetailPopulation.GetComponent<UnityEngine.UI.Text>().text = "Population: " + lastSelectedCelestialBody.Population + " / " + lastSelectedCelestialBody.maxPopulation;
		planetDetailTTPG.GetComponent<UnityEngine.UI.Text>().text = "Time to population growth: " + lastSelectedCelestialBody.timeToPopulationGrowth;
		planetDetailResourcers.GetComponent<UnityEngine.UI.Text>().text = "Resource Gatherers: " + lastSelectedCelestialBody.resourcers;
		planetDetailResearchers.GetComponent<UnityEngine.UI.Text>().text = "Researchers: " + lastSelectedCelestialBody.researchers;
		if(lastSelectedCelestialBody.unlockedTowerPrefab)
		{
			planetDetailUnlocks.SetActive(true);
			planetDetailUnlocks.transform.GetChild(1).GetComponent<Text>().text = lastSelectedCelestialBody.unlockedTowerPrefab.GetComponent<Tower>().displayName;
		}
			
		else
			planetDetailUnlocks.SetActive(false);

		arkButton.GetComponent<UnityEngine.UI.Button>().interactable = resources >= 100;

		//Tech UI
		if(TC.SelectedTechnology)
			TC.selectedTechUI.transform.GetChild(1).GetComponent<UnityEngine.UI.Text>().text = TC.SelectedTechnology.ResearchProgress + "/" + TC.SelectedTechnology.researchCost;
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
			celestialBody.GetComponent<CelestialBody>().population1 = bodyTab.transform.GetChild(3).gameObject;
		}
	}

	public void IncreaseSelectedCBResourcers()
	{
		lastSelectedCelestialBody.IncreaseResourcers();
	}

	public void IncreaseSelectedCBResearchers()
	{
		lastSelectedCelestialBody.IncreaseResearchers();
	}

	public void TransferPopulation()
	{
		planetDetailTransferMessage.SetActive(true);
		isTransferingPopulation = true;
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
		skipToNextSeason = true;
	}

	public void EndGame()
	{
		if(!IS_DEBUGGING)
			endGameUI.SetActive(true);
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
}
