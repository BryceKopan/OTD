using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserDefense3 : Talent
{
	protected override void GetSavedData()
	{
		isUnlocked = SavedData.saveData.hasLaserDefense3Talent;
	}

	public override void UnlockTalent()
	{
		if(SavedData.saveData.unspentTalentPoints > 0 || SavedData.IS_DEBUGGING)
		{
			SavedData.saveData.unspentTalentPoints--;
			SavedData.saveData.hasLaserDefense3Talent = true;
			base.UnlockTalent();
		}
	}
}

