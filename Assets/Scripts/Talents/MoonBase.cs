using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonBase : Talent
{
	protected override void GetSavedData()
	{
		isUnlocked = SavedData.saveData.hasMoonBaseTalent;
	}

	public override void UnlockTalent()
	{
		if(SavedData.saveData.unspentTalentPoints > 0 || SavedData.IS_DEBUGGING)
		{
			SavedData.saveData.unspentTalentPoints--;
			SavedData.saveData.hasMoonBaseTalent = true;
			base.UnlockTalent();
		}
	}
}
