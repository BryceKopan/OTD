using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallelFire : Technology
{
	public override void AddTechnologyTo(Tower tower)
	{
		tower.parallelFire = true;
		tower.projectilesPerVolley = tower.projectilesPerVolley * 2 / 1;
		tower.BaseCooldown = tower.BaseCooldown * (4f / 3f);
	}

	public override void RemoveTechnologyFrom(Tower tower)
	{
		tower.parallelFire = false;
		tower.projectilesPerVolley = tower.projectilesPerVolley * 1 / 2;
		tower.BaseCooldown = tower.BaseCooldown * (3f / 4f);
	}
}
