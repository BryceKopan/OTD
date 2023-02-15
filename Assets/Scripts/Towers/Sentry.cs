using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sentry : LaserTower
{
	public GameObject sentryTower;

	public override void GetXP()
	{
		sentryTower.GetComponent<SentryTower>().GetXP();
	}
}
