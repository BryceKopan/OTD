using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyModifier
{
	Shielded,
	Bursting,
	Spawning
}

public class Enemy : MonoBehaviour
{
	public GameObject target;
	public Vector3 directionUnit;
	public float speed;
	public float acceleration;
	public float turnSpeed;
	public float turnAcceleration;

	[HideInInspector]
	public int maxHealth;
	private int health = 1;
	public int Health
	{
		get { return health; }
		set
		{
			health = value;
			if(health <= 0)
				Die();
		}
	}

	public Shield shield;

	public int size = 1;

	public List<EnemyModifier> modifiers = new List<EnemyModifier>();
	private int enemyCount = 0;

	public float onDeathMultipleir = 1f, spawningTime = 1f;
	public Color shieldedColor, burstingColor, spawningColor;
	public float angleBetweenEnemies = 15f;
	public float spaceBetween = .5f;
	private WaveController WC;

	private float strengthValue = 0;
	public float shieldStrengthValueScalar = 1, spawnOnDeathStrengthValueScalar = 1, spawningStrengthValueScalar = 1;

	private GameController GC;

	private void Start()
	{
		if(strengthValue == 0)
		{
			SetupEnemy();
		}

		GC = FindObjectOfType<GameController>();
		WC = FindObjectOfType<WaveController>();
	}

	private void Update()
	{
		Vector3 deltaPosition = directionUnit * Time.deltaTime * speed;
		transform.position = transform.position + deltaPosition;
		transform.rotation = Quaternion.LookRotation(directionUnit, new Vector3(0, 1, 0));

		directionUnit = Vector3.RotateTowards(directionUnit, (target.transform.position - transform.position), turnSpeed * Time.deltaTime, 0).normalized;
		speed += Time.deltaTime * acceleration;
		turnSpeed += Time.deltaTime * turnAcceleration;
	}

	public void Die()
	{
		Destroy(gameObject);

		if(modifiers.Contains(EnemyModifier.Bursting))
		{
			for(int i = 0; i < enemyCount; i ++)
			{

				Quaternion targetRotation = transform.rotation;
				Vector3 relativePosition = new Vector3(0, 0, 0);

				relativePosition.x = -spaceBetween + (i % 3 * spaceBetween);
				relativePosition.z = -( i / 3) * spaceBetween;

				SpawnEnemy(relativePosition, targetRotation, directionUnit);
			}
		}

		GC.SC.EnemiesKilled++;
		GC.SC.EnemiesKilledScore += strengthValue;
	}

	public IEnumerator SpawnEnemiesAtInterval()
	{
		do
		{
			yield return new WaitForSeconds(spawningTime);


			for(int i = 0; i<size; i++)
			{
				Quaternion targetRotation = transform.rotation;
				targetRotation *= Quaternion.Euler(0, 180, 0);
				Vector3 startingDirection = -transform.forward;
				float startAngle = (((size - 1f) / 2f) * angleBetweenEnemies) - angleBetweenEnemies * i;
				startingDirection = Quaternion.Euler(0, startAngle - transform.rotation.y, 0) * startingDirection;
				Vector3 relativePosition = new Vector3((-spaceBetween * (size-1f) / 2f) + spaceBetween * i, 0, -.5f);
				SpawnEnemy(relativePosition, targetRotation, startingDirection);
			}
		} while(Health > 0);
	}

	private void SpawnEnemy(Vector3 relativePosition, Quaternion targetRotation, Vector3 startingDirection)
	{
		Vector3 targetPosition = transform.TransformPoint(relativePosition);
		Enemy enemy = Instantiate(WC.enemyPrefab, targetPosition, targetRotation, target.transform).GetComponent<Enemy>();
		enemy.speed = speed;
		enemy.acceleration = acceleration;
		enemy.turnSpeed = turnSpeed;
		enemy.turnAcceleration = turnAcceleration;
		enemy.target = target;
		enemy.shield.ShieldStrength = 0;
		enemy.size = 1;

		enemy.directionUnit = startingDirection;
	}

	public float GetStrengthValue()
	{
		if(strengthValue == 0)
		{
			SetupEnemy();
		}
		return strengthValue;
	}

	private void SetupEnemy()
	{
		if(SavedData.saveData.isShieldWallMode && shield.ShieldStrength == 0)
			shield.ShieldStrength = 1;

		if(SavedData.saveData.isBigThreatMode && size == 1)
			size = 2;

		switch(size)
		{
			case 1:
				gameObject.transform.localScale = new Vector3(.5f, 1f, .5f);
				Health = 1;
				if(modifiers.Contains(EnemyModifier.Bursting))
					modifiers.Remove(EnemyModifier.Bursting);
				if(modifiers.Contains(EnemyModifier.Spawning))
					modifiers.Remove(EnemyModifier.Spawning);
				break;
			case 2:
				gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
				Health = 5;
				enemyCount = 3;
				break;
			case 3:
				gameObject.transform.localScale = new Vector3(1.5f, 1f, 1.5f);
				Health = 10;
				enemyCount = 6;
				break;
		}

		if(modifiers.Contains(EnemyModifier.Bursting))
			transform.GetChild(0).GetComponent<SpriteRenderer>().color = burstingColor;

		if(modifiers.Contains(EnemyModifier.Spawning))
		{
			transform.GetChild(0).GetComponent<SpriteRenderer>().color = spawningColor;
			StartCoroutine(SpawnEnemiesAtInterval());
		}

		strengthValue = Health + (shield.ShieldStrength * shieldStrengthValueScalar);
		if(modifiers.Contains(EnemyModifier.Bursting))
			strengthValue *= spawnOnDeathStrengthValueScalar;
		if(modifiers.Contains(EnemyModifier.Spawning))
			strengthValue += spawningStrengthValueScalar;

		maxHealth = Health;

		if(shield.ShieldStrength > 0)
			modifiers.Add(EnemyModifier.Shielded);
	}
}
