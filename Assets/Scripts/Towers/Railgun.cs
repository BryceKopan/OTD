using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Railgun : LaserTower
{
	public float rangeGrowthPercent;

	public override void SetRankStats()
	{
		base.SetRankStats();
		Range = BaseRange + (Rank * (BaseRange * rangeGrowthPercent / 100));
	}
}
