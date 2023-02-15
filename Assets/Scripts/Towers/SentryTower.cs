using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentryTower : LaserTower
{
	public GameObject sentryPrefab;
	public float maxSentries = 2;
	public float buildCooldown = 4;

	private List<GameObject> sentries = new List<GameObject>();

	private void Start()
	{
		StartCoroutine(BuildSentry());
	}

	IEnumerator BuildSentry()
	{
		yield return new WaitForSeconds(buildCooldown);

		if(sentries.Count < maxSentries)
		{
			GameObject newSentry = Instantiate(sentryPrefab, transform.position, transform.rotation);
			sentries.Add(newSentry);
			newSentry.GetComponent<Sentry>().sentryTower = gameObject;
			Orbit sentryOrbit = newSentry.GetComponent<Orbit>();
			Orbit towerOrbit = GetComponent<Orbit>();
			sentryOrbit.principle = towerOrbit.principle;
			sentryOrbit.axisVector = towerOrbit.axisVector;

			float sentryPlacementAngle = 360 / (sentries.Count + 1);

			for(int i = 0; i < sentries.Count; i++)
			{
				Orbit orbit = sentries[i].GetComponent<Orbit>();
				orbit.followOrbit = false;
				sentries[i].transform.position = towerOrbit.orbitPath.GetPositionOnEllipse((sentryPlacementAngle * (i + 1)) + towerOrbit.GetCurrentAngle());
				orbit.RestartOrbit();
			}
		}

		StartCoroutine(BuildSentry());
	}

	public override void GetXP()
	{
		prestigeProgress += 1f / (10f + 10f * prestige);

		if(prestigeProgress >= 1 && prestige < 10)
		{
			prestige++;
			prestigeProgress = 0;

			cooldown -= cooldownGrowth;
			GetComponent<SphereCollider>().radius += rangeGrowth;
			if(prestige % 2 == 0)
				maxSentries++;
		}
	}
}