using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tower : MonoBehaviour
{
	public GameObject travelShipPrefab;
	private GameObject travelShip;
	private bool isTraveling = false;
	private GameObject origin;
	public float travelSpeed = 2.5f;

	public string displayName = "Tower";
	public float resourceCost = 1f;

	public int maxHealth = 3;
	private int health;
	public int Health
	{
		get { return health; }
		set
		{
			health = value;
			if(health <= 0)
				Destroy(gameObject);
		}
	}

	[SerializeField]
	private float baseCooldown;
	public float BaseCooldown
	{
		get { return baseCooldown; }
		set { baseCooldown = value; SetRankStats(); }
	}
	[HideInInspector]
	public float cooldown;

	protected bool readyToFire = true;

	private float baseRange;
	public float BaseRange
	{
		get { return baseRange; }
		set { baseRange = value; SetRankStats(); }
	}
	private float range;
	public float Range
	{
		get { return range; }
		set { targetingCollider.radius = range = value;}
	}

	public float rankProgress = 0, xpMultiplier = 1;
	private int rank = 0;
	public int Rank
	{
		get { return rank; }
		set { rank = value; SetRankStats(); }
	}

	public string targetTag = "Enemy";

	protected List<GameObject> targetsInRange = new List<GameObject>();
	protected Orbit orbit;

	SphereCollider targetingCollider;

	public bool parallelFire = false, burstFire = false;
	public int projectilesPerVolley = 1;
	[SerializeField]
	protected float distanceBetweenProjectiles = .5f;
	protected GameController GC;

	protected void Start()
	{
		GC = FindObjectOfType<GameController>();
		orbit = GetComponent<Orbit>();
		targetingCollider = GetComponent<SphereCollider>();
		Range = BaseRange = targetingCollider.radius;
		cooldown = BaseCooldown;
		Health = maxHealth;
		travelSpeed *= GC.travelTimeScalar;
	}

	private void FixedUpdate()
	{
		for(int i = 0; i < targetsInRange.Count; i++)
		{
			if(targetsInRange[i] == null)
			{
				targetsInRange.RemoveAt(i);
				i--;
			}
		}

		if(!isTraveling)
			FireAtFarthestTarget();
		else
			Travel();
	}

	protected virtual void FireAtFarthestTarget()
	{
		if(readyToFire && targetsInRange.Count > 0)
		{
			GameObject farthestTarget = targetsInRange[0];
			Vector3 planetPosition = GetComponent<Orbit>().principle.transform.position;

			for(int i = 0; i < targetsInRange.Count; i++)
			{
				if(Vector3.Distance(targetsInRange[i].transform.position, planetPosition) < Vector3.Distance(farthestTarget.transform.position, planetPosition))
				{
					farthestTarget = targetsInRange[i];
				}
			}

			readyToFire = false;
			StartCoroutine(FireAt(farthestTarget));
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.transform.gameObject.tag == targetTag)
		{
			targetsInRange.Add(other.transform.gameObject);		
		}
	}
	private void OnTriggerExit(Collider other)
	{
		if(other.transform.gameObject.tag == targetTag)
		{
			targetsInRange.Remove(other.transform.gameObject);
		}
	}

	public abstract IEnumerator FireAt(GameObject target);

	protected IEnumerator FireCooldown()
	{
		readyToFire = false;
		yield return new WaitForSeconds(cooldown);
		readyToFire = true;
	}

	public virtual void GetXP()
	{
		rankProgress += (1f / (10f + 10f * Rank)) * xpMultiplier;

		if(rankProgress >= 1 && Rank < 10)
		{
			Rank ++;
			rankProgress = 0;
		}
	}

	public virtual void SetRankStats()
	{
		cooldown = BaseCooldown;
		Range = BaseRange;
	}

	protected Vector3 GetParallellFireVector(GameObject target)
	{
		Vector3 targetVector = Vector3.Normalize(target.transform.position - transform.position);
		Vector2 perp = Vector2.Perpendicular(new Vector2(targetVector.x, targetVector.y));
		Vector3 adjustment = Vector3.Normalize(new Vector3(perp.x, 0, perp.y));

		return adjustment;
	}

	private void OnCollisionEnter(Collision collision)
	{
		if(collision.transform.gameObject.tag == "Enemy" && !isTraveling)
		{
			Health--;
			collision.gameObject.GetComponent<Enemy>().Die();
		}
	}

	public void StartTravel(GameObject origin)
	{
		this.origin = origin;
		transform.GetChild(0).gameObject.SetActive(false);
		transform.GetChild(2).gameObject.SetActive(true);
		travelShip = Instantiate(travelShipPrefab, origin.transform.position, Quaternion.identity, transform);
		isTraveling = true;
	}
	
	public void FinishTravel()
	{
		transform.GetChild(0).gameObject.SetActive(true);
		transform.GetChild(2).gameObject.SetActive(false);
		Destroy(travelShip);
		isTraveling = false;
	}

	private void Travel()
	{
		travelShip.transform.position = Vector3.MoveTowards(travelShip.transform.position, transform.position, Time.deltaTime * travelSpeed);
		travelShip.transform.rotation = Quaternion.LookRotation(transform.position - travelShip.transform.position, new Vector3(0, 1, 0));

		if(travelShip.transform.position == transform.position)
		{
			FinishTravel();
		}
	}
}
