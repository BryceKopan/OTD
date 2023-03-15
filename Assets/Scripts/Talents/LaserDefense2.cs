using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserDefense2 : Talent
{
	protected override void GetSavedData()
	{
		isUnlocked = SavedData.saveData.hasLaserDefense2Talent;
	}

	public override void UnlockTalent()
	{
		if(SavedData.saveData.unspentTalentPoints > 0 || SavedData.IS_DEBUGGING)
		{
			base.UnlockTalent();
			SavedData.saveData.unspentTalentPoints--;
			SavedData.saveData.hasLaserDefense2Talent = true;
		}
	}
}

