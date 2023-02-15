using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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

public class GameController : MonoBehaviour
{
	public GameObject selectedObject;

	public WaveSettings wavesSettings;

	public float resources = 1f;

	public bool pause = true;
	public bool showAllRanges = false;
	private float timeScale;

	public GameObject waveCounter, resourceCounter, pauseSymbol, playSymbol, tower1Button, tower2Button, tower3Button, endGameUI;

	public GameObject gatePrefab, tower1Prefab, tower2Prefab, tower3Prefab;
	GameObject gate, unbuiltTower;
	float unbuiltTowerCost;
	Orbit unbuiltTowerOrbit;

	public GameObject Sun, TargetPlanet;

	RaycastHit mouseHit;

	public float orbitIncrement, angleIncrement;

	public GameObject window, towerWindow, towerName, towerRange, towerSpeed, towerPrestige, towerProgress;

	// Start is called before the first frame update
	void Start()
    {
		if(pause)
		{
			pause = false;
			Pause();
		}

		resourceCounter.GetComponent<UnityEngine.UI.Text>().text = "Resources: " + resources;

		SpawnGate();
	}

    // Update is called once per frame
    void Update()
    {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if(Physics.Raycast(ray, out mouseHit, Mathf.Infinity, -1, QueryTriggerInteraction.Ignore))
		{
			RangeToolTip toolTip = mouseHit.transform.GetComponent<RangeToolTip>();
			if(toolTip)
			{
				toolTip.indicatorIsActive = true;
			}
		}

		if(unbuiltTower != null)
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
		if(resources >= 1)
			tower1Button.GetComponent<UnityEngine.UI.Button>().interactable = true;
		if(resources >= 2)
			tower2Button.GetComponent<UnityEngine.UI.Button>().interactable = true;
		if(resources >= 3)
			tower3Button.GetComponent<UnityEngine.UI.Button>().interactable = true;
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

	public void LeftClick(InputAction.CallbackContext context)
	{
		if(context.started)
		{
			if(unbuiltTower != null)
			{
				unbuiltTowerOrbit.RestartOrbit();
				unbuiltTower = null;
				AddResources(-unbuiltTowerCost);
				if(resources < 1)
					tower1Button.GetComponent<UnityEngine.UI.Button>().interactable = false;
				if(resources < 2)
					tower2Button.GetComponent<UnityEngine.UI.Button>().interactable = false;
				if(resources < 3)
					tower3Button.GetComponent<UnityEngine.UI.Button>().interactable = false;
			}
			else if(mouseHit.transform)
			{
				GameObject obj = mouseHit.transform.gameObject;
				if(obj)
				{
					Select(obj);
				}
			}
		}
	}

	public void RightClick(InputAction.CallbackContext context)
	{
		if(context.started)
		{
			Destroy(unbuiltTower);
			unbuiltTower = null;
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

	void SpawnGate()
	{
		float angle = Random.Range(0.0f, Mathf.PI * 2);
		Vector3 V = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));
		V *= wavesSettings.gateDistance;
		Vector3 gateLocation = V + TargetPlanet.transform.position;

		gate = Instantiate(gatePrefab, gateLocation, transform.rotation);
		Orbit gateOrbit = gate.GetComponent<Orbit>();
		gateOrbit.principle = TargetPlanet.GetComponent<CelestialBody>();
		gateOrbit.axisVector = new Vector2(wavesSettings.gateDistance, wavesSettings.gateDistance);
		gateOrbit.RestartOrbit();
	}

	public void WaveComplete()
	{
		if(wavesSettings.wave % 2 == 0)
			wavesSettings.burstsPerWave++;
		else
			wavesSettings.enemiesPerBurst++;

		wavesSettings.wave++;
		waveCounter.GetComponent<UnityEngine.UI.Text>().text = "Wave: " + (wavesSettings.wave + 1);

		if(wavesSettings.wave % 3 == 0)
		{
			wavesSettings.gateDistance += wavesSettings.gateDistanceGrowth;
			Destroy(gate.gameObject);
			StartCoroutine(DelayedSpawn());
		}
	}

	IEnumerator DelayedSpawn()
	{
		yield return new WaitForSeconds(wavesSettings.waveCooldown);
		SpawnGate();
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
		selectedObject = obj;

		if(obj.GetComponent<Tower>())
		{
			Tower tower = obj.GetComponent<Tower>();
			window.SetActive(true);
			towerWindow.SetActive(true);
			towerName.GetComponent<UnityEngine.UI.Text>().text = tower.displayName;
		}
		else if(obj.GetComponent<MiningTower>())
		{
			MiningTower tower = obj.GetComponent<MiningTower>();
			window.SetActive(true);
			towerWindow.SetActive(true);
			towerName.GetComponent<UnityEngine.UI.Text>().text = tower.displayName;
		}
		else
		{
			window.SetActive(false);
		}
	}

	private void UpdateSelectedUI()
	{
		if(selectedObject.GetComponent<Tower>())
		{
			Tower tower = selectedObject.GetComponent<Tower>();
			towerRange.GetComponent<UnityEngine.UI.Text>().text = "Range: " + tower.GetComponent<SphereCollider>().radius;
			towerSpeed.GetComponent<UnityEngine.UI.Text>().text = "Speed: " + (1 / tower.cooldown).ToString("n2") + "/s";
			towerPrestige.GetComponent<UnityEngine.UI.Text>().text = "Prestige: " + tower.prestige;
			towerProgress.GetComponent<UnityEngine.UI.Text>().text = "Progress: " + tower.prestigeProgress.ToString("n2");
		}
		else if(selectedObject.GetComponent<MiningTower>())
		{
			MiningTower tower = selectedObject.GetComponent<MiningTower>();
			towerRange.GetComponent<UnityEngine.UI.Text>().text = "Range: " + tower.GetComponent<SphereCollider>().radius;
			towerSpeed.GetComponent<UnityEngine.UI.Text>().text = "Speed: " + (1 / tower.cooldown).ToString("n2") + "/s";
			towerPrestige.GetComponent<UnityEngine.UI.Text>().text = "Prestige: " + tower.prestige;
			towerProgress.GetComponent<UnityEngine.UI.Text>().text = "Progress: " + tower.prestigeProgress.ToString("n2");
		}
	}

	public void DeleteSelectedTower()
	{
		Tower tower = selectedObject.GetComponent<Tower>();
		if(tower)
			Destroy(tower.gameObject);
		Select(null);
	}
}
