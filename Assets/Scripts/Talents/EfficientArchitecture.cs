using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EfficientArchitecture : Talent
{
	protected override void GetSavedData()
	{
		isUnlocked = SavedData.saveData.hasEfficientArchitecureTalent;
	}

	public override void UnlockTalent()
	{
		if(SavedData.saveData.unspentTalentPoints > 0 || SavedData.IS_DEBUGGING)
		{
			SavedData.saveData.unspentTalentPoints--;
			SavedData.saveData.hasEfficientArchitecureTalent = true;
			base.UnlockTalent();
		}
	}
}
