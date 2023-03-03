using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
	public GameObject selectedObject;
	public CelestialBody lastSelectedCelestialBody;

	private WaveController WC;
	private TechController TC;
	public int season = 0;

	public float resources = 1f;

	public bool pause = true, skipToNextSeason = false;
	public bool showAllRanges = false;
	private float timeScale;

	public GameObject seasonCounter, resourceCounter, pauseSymbol, playSymbol, tower1Button, tower2Button, tower3Button, tower4Button, tower5Button, tower6Button, endGameUI;

	public GameObject tower1Prefab, tower2Prefab, tower3Prefab, tower4Prefab, tower5Prefab, tower6Prefab;
	GameObject unbuiltTower;
	bool isPlacementValid = true;
	Color originalColor;
	float unbuiltTowerCost;
	Orbit unbuiltTowerOrbit;

	public GameObject Sun, TargetPlanet;

	RaycastHit mouseHit;

	public float orbitIncrement, angleIncrement;

	public GameObject window, towerWindow, towerName, towerRange, towerSpeed, towerPrestige, towerProgress, sentryTower, maxSentries, buildSpeed;
	public GameObject ark, arkButton;

	public List<GameObject> tabBodies = new List<GameObject>();
	public List<GameObject> tabs = new List<GameObject>();
	public List<CelestialBody> populatedBodies = new List<CelestialBody>(); 

	public GameObject totalHealth, planetDetail, planetDetailName, planetDetailPopulation, planetDetailResourcers, planetDetailResearchers, planetDetailTTPG, planetDetailTransferMessage;
	public GameObject researchPanel;

	private bool isTransferingPopulation = false;

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
				tabBodies[i].GetComponent<CelestialBody>().IsExplored = false;
				SetBodyTab(tabs[i], null);
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

		populatedBodies = GetPopulatedBodies();
		UpdateUI();

		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if(Physics.Raycast(ray, out mouseHit, Mathf.Infinity, -1, QueryTriggerInteraction.Ignore))
		{
			RangeToolTip toolTip = mouseHit.transform.GetComponentInChildren<RangeToolTip>();
			if(toolTip)
			{
				toolTip.indicatorIsActive = true;
			}
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
			if(TC.selectedTower != null)
				Destroy(TC.selectedTower);
			TC.selectedTower = unbuiltTower;
			unbuiltTower.SetActive(false);
			unbuiltTower = null;
			TC.UpdateSelectedTechUI();
		}

		if(selectedObject)
			UpdateSelectedUI();
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
	}

	public void BuildTower()
	{
		unbuiltTowerCost = 1;
		if(resources >= unbuiltTowerCost)
			BuildObject(tower1Prefab);
	}

	public void BuildTower2()
	{
		unbuiltTowerCost = 2;
		if(resources >= unbuiltTowerCost)
			BuildObject(tower2Prefab);
	}

	public void BuildTower3()
	{
		unbuiltTowerCost = 3;
		if(resources >= unbuiltTowerCost)
			BuildObject(tower3Prefab);
	}

	public void BuildTower4()
	{
		unbuiltTowerCost = 5;
		if(resources >= unbuiltTowerCost)
			BuildObject(tower4Prefab);
	}

	public void BuildTower5()
	{
		unbuiltTowerCost = 5;
		if(resources >= unbuiltTowerCost)
			BuildObject(tower5Prefab);
	}

	public void BuildTower6()
	{
		unbuiltTowerCost = 5;
		if(resources >= unbuiltTowerCost)
			BuildObject(tower6Prefab);
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
				if(unbuiltTower.GetComponent<Tower>())
				{
					TC.AddTower(unbuiltTower.GetComponent<Tower>());
				}

				unbuiltTowerOrbit.RestartOrbit();
				unbuiltTower = null;
				AddResources(-unbuiltTowerCost);
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
			Destroy(unbuiltTower);
			unbuiltTower = null;

			if(isTransferingPopulation)
			{
				isTransferingPopulation = false;
				planetDetailTransferMessage.SetActive(false);
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
			window.SetActive(false);
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

			if(lastSelectedCelestialBody == obj.GetComponent<CelestialBody>() && lastSelectedCelestialBody.isPopulated)
			{
				ShowPlanetDetails();
			}
			else
			{
				planetDetail.SetActive(false);
			}

			lastSelectedCelestialBody = obj.GetComponent<CelestialBody>();
		}
		if(obj.GetComponent<Tower>())
		{
			sentryTower.SetActive(false);

			Tower tower = obj.GetComponent<Tower>();
			window.SetActive(true);
			towerWindow.SetActive(true);
			towerName.GetComponent<UnityEngine.UI.Text>().text = tower.displayName;
		}
		if(obj.GetComponent<SentryTower>())
		{
			sentryTower.SetActive(true);
		}

		selectedObject = obj;
	}

	private void UpdateSelectedUI()
	{
		if(selectedObject.GetComponent<Tower>())
		{
			Tower tower = selectedObject.GetComponent<Tower>();
			towerRange.GetComponent<UnityEngine.UI.Text>().text = "Range: " + tower.GetComponent<SphereCollider>().radius;
			towerSpeed.GetComponent<UnityEngine.UI.Text>().text = "Speed: " + (1 / tower.cooldown).ToString("n2") + "/s";
			towerPrestige.GetComponent<UnityEngine.UI.Text>().text = "Prestige: " + tower.Rank;
			towerProgress.GetComponent<UnityEngine.UI.Text>().text = "Progress: " + tower.rankProgress.ToString("n2");
		}
		if(selectedObject.GetComponent<SentryTower>())
		{
			SentryTower tower = selectedObject.GetComponent<SentryTower>();
			maxSentries.GetComponent<UnityEngine.UI.Text>().text = "Max Sentries: " + tower.maxSentries;
			buildSpeed.GetComponent<UnityEngine.UI.Text>().text = "Build Speed: " + (1 / tower.buildCooldown).ToString("n2") + "/s";
		}
	}

	public void DeleteSelectedTower()
	{
		Tower tower = selectedObject.GetComponent<Tower>();
		if(tower)
			Destroy(tower.gameObject);
		Select(null);
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

		foreach(CelestialBody body in populatedBodies)
		{
			body.IncrementSeason();
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

		//Buttons that grey out from cost
		tower1Button.GetComponent<UnityEngine.UI.Button>().interactable = resources >= 1;
		tower2Button.GetComponent<UnityEngine.UI.Button>().interactable = resources >= 2;
		tower3Button.GetComponent<UnityEngine.UI.Button>().interactable = resources >= 3;
		tower4Button.GetComponent<UnityEngine.UI.Button>().interactable = resources >= 5;
		tower5Button.GetComponent<UnityEngine.UI.Button>().interactable = resources >= 5;
		tower6Button.GetComponent<UnityEngine.UI.Button>().interactable = resources >= 5;
		arkButton.GetComponent<UnityEngine.UI.Button>().interactable = resources >= 100;
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
}
