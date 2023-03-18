using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResearchPlanets : Talent
{
	protected override void GetSavedData()
	{
		isUnlocked = SavedData.saveData.hasResearchPlanetsTalent;
	}

	public override void UnlockTalent()
	{
		if(SavedData.saveData.unspentTalentPoints > 0 || SavedData.IS_DEBUGGING)
		{
			SavedData.saveData.unspentTalentPoints--;
			SavedData.saveData.hasResearchPlanetsTalent = true;
			base.UnlockTalent();
		}
	}
}
