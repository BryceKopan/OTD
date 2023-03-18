using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulatedPlanet : Talent
{
	protected override void GetSavedData()
	{
		isUnlocked = SavedData.saveData.hasPopulatedPlanetTalent;
	}

	public override void UnlockTalent()
	{
		if(SavedData.saveData.unspentTalentPoints > 0 || SavedData.IS_DEBUGGING)
		{
			SavedData.saveData.unspentTalentPoints--;
			SavedData.saveData.hasPopulatedPlanetTalent = true;
			base.UnlockTalent();
		}
	}
}
