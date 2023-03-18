using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiningMoons : Talent
{
	protected override void GetSavedData()
	{
		isUnlocked = SavedData.saveData.hasMiningMoonsTalent;
	}

	public override void UnlockTalent()
	{
		if(SavedData.saveData.unspentTalentPoints > 0 || SavedData.IS_DEBUGGING)
		{
			SavedData.saveData.unspentTalentPoints--;
			SavedData.saveData.hasMiningMoonsTalent = true;
			base.UnlockTalent();
		}
	}
}
