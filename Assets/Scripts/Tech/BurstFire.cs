using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurstFire : Technology
{
	public override void AddTechnologyTo(Tower tower)
	{
		tower.burstFire = true;
		tower.projectilesPerVolley = tower.projectilesPerVolley * 3 / 1;
		tower.BaseCooldown = tower.BaseCooldown * (4f / 3f);
	}

	public override void RemoveTechnologyFrom(Tower tower)
	{
		tower.burstFire = false;
		tower.projectilesPerVolley = tower.projectilesPerVolley * 1 / 3;
		tower.BaseCooldown = tower.BaseCooldown * (3f / 4f);
	}
}
