using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
	public int originSeason = 0;
	public GameObject enemyPrefab;
	private GameObject targetCB;

	private GameController GC;
	private WaveController WC;

	void Start()
    {
		GC = FindObjectOfType<GameController>();
		WC = FindObjectOfType<WaveController>();
		targetCB = GetComponent<Orbit>().principle.gameObject;
		originSeason = GC.SC.Season;
	}

    void Update()
    {
		transform.rotation = Quaternion.LookRotation(targetCB.transform.position - transform.position, new Vector3(0, 1, 0));
	}

	public IEnumerator SpawnWave(int waveStrength, WavePattern wavePattern)
	{
		int spawnedStrength = 0;
		waveStrength = Mathf.RoundToInt((wavePattern.enemyStrengthWeight * waveStrength));

		if(wavePattern.patternCycle.Count == 0)
			wavePattern.patternCycle = new List<int>{ 0 };

		while(spawnedStrength < waveStrength)
		{
			foreach(int index in wavePattern.patternCycle)
			{
				if(spawnedStrength < waveStrength)
				{
					yield return new WaitForSeconds(wavePattern.enemySpawnInterval);
					SpawnPattern spawnPattern = wavePattern.spawnPatterns[index];

					if(spawnPattern.sizePerEnemy.Length != spawnPattern.enemiesPerSpawn)
					{
						spawnPattern.sizePerEnemy = new int[spawnPattern.enemiesPerSpawn];
						for(int i = 0; i < spawnPattern.enemiesPerSpawn; i++)
							spawnPattern.sizePerEnemy[i] = 1;
					}

					if(spawnPattern.shieldPerEnemy.Length != spawnPattern.enemiesPerSpawn)
					{
						spawnPattern.shieldPerEnemy = new int[spawnPattern.enemiesPerSpawn];
						for(int i = 0; i < spawnPattern.enemiesPerSpawn; i++)
							spawnPattern.shieldPerEnemy[i] = 0;
					}

					if(spawnPattern.spawnOnDeathPerEnemy.Length != spawnPattern.enemiesPerSpawn)
					{
						spawnPattern.spawnOnDeathPerEnemy = new bool[spawnPattern.enemiesPerSpawn];
						for(int i = 0; i < spawnPattern.enemiesPerSpawn; i++)
							spawnPattern.spawnOnDeathPerEnemy[i] = false;
					}

					if(spawnPattern.spawningEnemyPerEnemy.Length != spawnPattern.enemiesPerSpawn)
					{
						spawnPattern.spawningEnemyPerEnemy = new bool[spawnPattern.enemiesPerSpawn];
						for(int i = 0; i < spawnPattern.enemiesPerSpawn; i++)
							spawnPattern.spawningEnemyPerEnemy[i] = false;
					}

					for(int i = 0; i < spawnPattern.enemiesPerSpawn; i++)
					{
						Enemy enemy = Instantiate(enemyPrefab, transform.position, transform.rotation, targetCB.transform).GetComponent<Enemy>();
						enemy.speed = spawnPattern.enemyStartSpeed;
						enemy.acceleration = spawnPattern.enemyAcceleration;
						enemy.turnSpeed = spawnPattern.enemyTurnSpeed;
						enemy.turnAcceleration = spawnPattern.enemyTurnAcceleration;
						enemy.target = targetCB;
						enemy.size = spawnPattern.sizePerEnemy[i];
						enemy.shield.ShieldStrength = spawnPattern.shieldPerEnemy[i];
						if(spawnPattern.spawnOnDeathPerEnemy[i])
							enemy.modifiers.Add(EnemyModifier.Bursting);
						if(spawnPattern.spawningEnemyPerEnemy[i])
							enemy.modifiers.Add(EnemyModifier.Spawning);

						Vector3 startingDirection = targetCB.transform.position - transform.position;
						float startAngle = spawnPattern.enemyStartAngle - (((spawnPattern.enemiesPerSpawn - 1) / 2) * spawnPattern.angleBetweenEnemies) + spawnPattern.angleBetweenEnemies * i;
						startingDirection = Quaternion.Euler(0, startAngle, 0) * startingDirection;
						enemy.directionUnit = startingDirection;

						spawnedStrength += (int)enemy.GetStrengthValue();
					}
				}
			}
		}
	}
}
