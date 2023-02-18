using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
	private int originSeason = 0;
	public GameObject enemyPrefab;
	private GameObject target;

	private GameController GC;
	private WaveController WC;

	void Start()
    {
		GC = FindObjectOfType<GameController>();
		WC = FindObjectOfType<WaveController>();
		target = FindObjectOfType<Planet>().gameObject;
		StartCoroutine(SpawnEnemy());
		originSeason = GC.season;
	}

    void Update()
    {
		transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position, new Vector3(0, 1, 0));
	}

	IEnumerator SpawnEnemy()
	{
		WaveSettings wave = WC.wavesSettings;
		yield return new WaitForSeconds(wave.waveCooldown);

		while(true)
		{
			wave = WC.wavesSettings;
			for(int i = 0; i < wave.burstsPerWave; i++)
			{
				for(int j = 0; j < wave.enemiesPerBurst; j++)
				{
					Enemy enemy = Instantiate(enemyPrefab, transform.position, transform.rotation, target.transform).GetComponent<Enemy>();
					yield return new WaitForSeconds(wave.enemiesCooldown);
				}	
				yield return new WaitForSeconds(wave.burstsCooldown);
			}

			yield return new WaitForSeconds(wave.waveCooldown);
		}
	}
}
