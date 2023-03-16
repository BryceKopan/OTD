using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlannedWorld : Talent
{
	protected override void GetSavedData()
	{
		isUnlocked = SavedData.saveData.hasPlannedWorldTalent;
	}

	public override void UnlockTalent()
	{
		if(SavedData.saveData.unspentTalentPoints > 0 || SavedData.IS_DEBUGGING)
		{
			SavedData.saveData.unspentTalentPoints--;
			SavedData.saveData.hasPlannedWorldTalent = true;
			base.UnlockTalent();
		}
	}
}
