using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowthVats : Talent
{
	protected override void GetSavedData()
	{
		isUnlocked = SavedData.saveData.hasGrowthVatsTalent;
	}

	public override void UnlockTalent()
	{
		if(SavedData.saveData.unspentTalentPoints > 0 || SavedData.IS_DEBUGGING)
		{
			SavedData.saveData.unspentTalentPoints--;
			SavedData.saveData.hasGrowthVatsTalent = true;
			base.UnlockTalent();
		}
	}
}
