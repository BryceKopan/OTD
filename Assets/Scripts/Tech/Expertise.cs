using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Expertise : Technology
{
	public override void AddTechnologyTo(Tower tower)
	{
		tower.xpMultiplier = tower.xpMultiplier * 2/1;
	}

	public override void RemoveTechnologyFrom(Tower tower)
	{
		tower.xpMultiplier = tower.xpMultiplier * 1/2;
	}
}
