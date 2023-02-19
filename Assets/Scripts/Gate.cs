using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
	public int originSeason = 0;
	public GameObject enemyPrefab;
	private GameObject target;

	private GameController GC;
	private WaveController WC;

	void Start()
    {
		GC = FindObjectOfType<GameController>();
		WC = FindObjectOfType<WaveController>();
		target = FindObjectOfType<Planet>().gameObject;
		originSeason = GC.season;
	}

    void Update()
    {
		transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position, new Vector3(0, 1, 0));
	}

	public IEnumerator SpawnWave(int enemyQuantity, WavePattern pattern)
	{
		int enemiesSpawned = 0;
		enemyQuantity = Mathf.RoundToInt((pattern.enemyQuantityWeight * enemyQuantity));

		while(enemiesSpawned < enemyQuantity)
		{
			yield return new WaitForSeconds(pattern.enemySpawnInterval);
			for(int i = 0; i < pattern.enemiesPerSpawn; i ++)
			{
				Enemy enemy = Instantiate(enemyPrefab, transform.position, transform.rotation, target.transform).GetComponent<Enemy>();
				enemy.speed = pattern.enemyStartSpeed;
				enemy.acceleration = pattern.enemyAcceleration;
				enemy.turnSpeed = pattern.enemyTurnSpeed;
				enemy.turnAcceleration = pattern.enemyTurnAcceleration;

				Vector3 startingDirection = target.transform.position - transform.position;
				float startAngle = pattern.enemyStartAngle - (((pattern.enemiesPerSpawn - 1) / 2) * pattern.angleBetweenEnemies) + pattern.angleBetweenEnemies * i;
				startingDirection = Quaternion.Euler(0, startAngle, 0) * startingDirection;

				enemy.directionUnit = startingDirection;
			}

			enemiesSpawned += pattern.enemiesPerSpawn;
		}
	}
}
