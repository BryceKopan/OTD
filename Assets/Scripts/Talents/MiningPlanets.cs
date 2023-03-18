using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiningPlanets : Talent
{
	protected override void GetSavedData()
	{
		isUnlocked = SavedData.saveData.hasMiningPlanetsTalent;
	}

	public override void UnlockTalent()
	{
		if(SavedData.saveData.unspentTalentPoints > 0 || SavedData.IS_DEBUGGING)
		{
			SavedData.saveData.unspentTalentPoints--;
			SavedData.saveData.hasMiningPlanetsTalent = true;
			base.UnlockTalent();
		}
	}
}
