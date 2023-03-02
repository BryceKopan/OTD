using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointDefense : Technology
{
	public override void AddTechnologyTo(Tower tower)
	{
		tower.BaseCooldown = tower.BaseCooldown * (1f / 4f);
		tower.BaseRange = tower.BaseRange * (3f / 4f);
	}

	public override void RemoveTechnologyFrom(Tower tower)
	{
		tower.BaseCooldown = tower.BaseCooldown * (4f / 1f);
		tower.BaseRange = tower.BaseRange * (4f / 3f);
	}
}
