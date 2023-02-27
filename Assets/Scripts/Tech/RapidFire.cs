using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RapidFire : Technology
{
	public override void AddTechnologyTo(Tower tower)
	{
		tower.cooldown = tower.cooldown * (3/4);
	}

	public override void RemoveTechnologyFrom(Tower tower)
	{
		tower.cooldown = tower.cooldown * (4/3);
	}
}
