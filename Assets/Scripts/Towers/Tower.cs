using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tower : MonoBehaviour
{
	public string displayName = "Tower";

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

	public float rankProgress = 0, xpMultiplier = 1, rangeGrowth, cooldownGrowth;
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

	private void Start()
	{
		orbit = GetComponent<Orbit>();
		targetingCollider = GetComponent<SphereCollider>();
		Range = BaseRange = targetingCollider.radius;
		cooldown = BaseCooldown;
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

		FireAtFarthestTarget();
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
		cooldown = BaseCooldown - (cooldownGrowth * rank);
		Range = BaseRange + (rangeGrowth * rank);
	}

	protected Vector3 GetParallellFireVector(GameObject target)
	{
		Vector3 targetVector = Vector3.Normalize(target.transform.position - transform.position);
		Vector2 perp = Vector2.Perpendicular(new Vector2(targetVector.x, targetVector.y));
		Vector3 adjustment = Vector3.Normalize(new Vector3(perp.x, 0, perp.y));

		return adjustment;
	}
}
