using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaDefense : Technology
{
	public override void AddTechnologyTo(Tower tower)
	{
		tower.BaseRange = tower.BaseRange * (7f / 4f);
		tower.BaseCooldown = tower.BaseCooldown * (5f / 4f);
	}

	public override void RemoveTechnologyFrom(Tower tower)
	{
		tower.BaseRange = tower.BaseRange * (4f / 7f);
		tower.BaseCooldown = tower.BaseCooldown * (4f / 5f);
	}
}
