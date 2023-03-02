using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongRange : Technology
{
	public override void AddTechnologyTo(Tower tower)
	{
		tower.BaseRange = tower.BaseRange * (5f / 4f);
	}

	public override void RemoveTechnologyFrom(Tower tower)
	{
		tower.BaseRange = tower.BaseRange * (4f / 5f);
	}
}
