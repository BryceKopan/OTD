using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Training : Technology
{
	public override void AddTechnologyTo(Tower tower)
	{
		if(tower.Rank < 1)
			tower.Rank = 1;
	}

	public override void RemoveTechnologyFrom(Tower tower)
	{
	}
}
