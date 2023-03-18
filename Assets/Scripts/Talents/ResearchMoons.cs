using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResearchMoons : Talent
{
	protected override void GetSavedData()
	{
		isUnlocked = SavedData.saveData.hasResearchMoonsTalent;
	}

	public override void UnlockTalent()
	{
		if(SavedData.saveData.unspentTalentPoints > 0 || SavedData.IS_DEBUGGING)
		{
			SavedData.saveData.unspentTalentPoints--;
			SavedData.saveData.hasResearchMoonsTalent = true;
			base.UnlockTalent();
		}
	}
}
