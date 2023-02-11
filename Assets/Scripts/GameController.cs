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
	private float timeScale;

	public GameObject waveCounter, resourceCounter, pauseSymbol, playSymbol, tower1Button, endGameUI;

	public GameObject gatePrefab, tower1Prefab;
	GameObject gate, unbuiltTower;
	Orbit unbuiltTowerOrbit;

	public GameObject Sun;

	RaycastHit mouseHit;

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
			unbuiltTowerOrbit.axisVector = new Vector2(orbitSize, orbitSize);
			unbuiltTowerOrbit.principle = GetClosestCelestialBody(unbuiltTower);
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
		if(resources >= 1)
			tower1Button.GetComponent<UnityEngine.UI.Button>().interactable = true;
	}

	public void BuildTower()
	{
		if(!pause)
			Pause();

		if(unbuiltTower)
			Destroy(unbuiltTower);

		unbuiltTower = Instantiate(tower1Prefab, transform.position, transform.rotation);
		unbuiltTowerOrbit = unbuiltTower.GetComponent<Orbit>();
		unbuiltTowerOrbit.followOrbit = false;
		unbuiltTowerOrbit.principle = GetClosestCelestialBody(unbuiltTower);
	}

	public void LeftClick(InputAction.CallbackContext context)
	{
		if(context.started)
		{
			if(unbuiltTower != null)
			{
				unbuiltTowerOrbit.RestartOrbit();
				unbuiltTower = null;
				AddResources(-1);
				if(resources < 1)
					tower1Button.GetComponent<UnityEngine.UI.Button>().interactable = false;
			}
			else if(mouseHit.transform)
			{
				GameObject obj = mouseHit.transform.gameObject;
				if(obj)
				{
					selectedObject = obj;
				}
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

	void SpawnGate()
	{
		float angle = Random.Range(0.0f, Mathf.PI * 2);
		Vector3 V = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));
		V *= wavesSettings.gateDistance;
		Vector3 gateLocation = V + Sun.transform.position;

		gate = Instantiate(gatePrefab, gateLocation, transform.rotation);
		Orbit gateOrbit = gate.GetComponent<Orbit>();
		gateOrbit.principle = Sun.GetComponent<CelestialBody>();
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
}
