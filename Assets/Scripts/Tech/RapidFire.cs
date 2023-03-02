using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RapidFire : Technology
{
	public override void AddTechnologyTo(Tower tower)
	{
		tower.BaseCooldown = tower.BaseCooldown * (3f/4f);
	}

	public override void RemoveTechnologyFrom(Tower tower)
	{
		tower.BaseCooldown = tower.BaseCooldown * (4f/3f);
	}
}
