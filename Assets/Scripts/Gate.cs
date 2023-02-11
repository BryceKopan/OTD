using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
	public GameObject enemyPrefab;
	//Path path;
	private GameObject target;

	private GameController GC;

	// Start is called before the first frame update
	void Start()
    {
		GC = FindObjectOfType<GameController>();
		target = FindObjectOfType<Planet>().gameObject;
		StartCoroutine(SpawnEnemy());
		//path = gameObject.GetComponentInChildren<Path>();
		//path.target = target;
	}

    // Update is called once per frame
    void Update()
    {
		transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position, new Vector3(0, 1, 0));
		//path.DrawPath();
	}

	IEnumerator SpawnEnemy()
	{
		WaveSettings wave = GC.wavesSettings;
		yield return new WaitForSeconds(wave.waveCooldown);

		while(true)
		{
			wave = GC.wavesSettings;
			for(int i = 0; i < wave.burstsPerWave; i++)
			{
				for(int j = 0; j < wave.enemiesPerBurst; j++)
				{
					Enemy enemy = Instantiate(enemyPrefab, transform.position, transform.rotation).GetComponent<Enemy>();
					yield return new WaitForSeconds(wave.enemiesCooldown);
				}	
				yield return new WaitForSeconds(wave.burstsCooldown);
			}

			GC.WaveComplete();
			yield return new WaitForSeconds(wave.waveCooldown);
		}
	}
}
