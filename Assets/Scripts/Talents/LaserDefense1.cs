using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserDefense1 : Talent
{
	protected override void GetSavedData()
	{
		isUnlocked = SavedData.saveData.hasLaserDefense1Talent;
	}

	public override void UnlockTalent()
	{
		if(SavedData.saveData.unspentTalentPoints > 0 || SavedData.IS_DEBUGGING)
		{
			base.UnlockTalent();
			SavedData.saveData.unspentTalentPoints--;
			SavedData.saveData.hasLaserDefense1Talent = true;
		}
	}
}

