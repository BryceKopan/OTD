using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sentry : LaserTower
{
	public GameObject sentryTower;

	private TechController TC;

	private new void Start()
	{
		base.Start();

		TC = FindObjectOfType<TechController>();
		TC.AddTower(GetComponent<Tower>());
	}

	public override void GetXP()
	{
		sentryTower.GetComponent<SentryTower>().GetXP();
	}
}
