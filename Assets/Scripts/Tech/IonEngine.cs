using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IonEngine : Talent
{
	protected override void GetSavedData()
	{
		isUnlocked = SavedData.saveData.hasIonEngineTalent;
	}

	public override void UnlockTalent()
	{
		if(SavedData.saveData.unspentTalentPoints > 0 || SavedData.IS_DEBUGGING)
		{
			base.UnlockTalent();
			SavedData.saveData.unspentTalentPoints--;
			SavedData.saveData.hasIonEngineTalent = true;
		}
	}
}
