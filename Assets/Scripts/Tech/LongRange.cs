using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongRange : Technology
{
	public override void AddTechnologyTo(Tower tower)
	{
		tower.cooldown = tower.cooldown * (3f / 4f);
	}

	public override void RemoveTechnologyFrom(Tower tower)
	{
		tower.cooldown = tower.cooldown * (4f / 3f);
	}
}
