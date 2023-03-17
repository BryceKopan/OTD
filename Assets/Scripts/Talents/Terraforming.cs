using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terraforming : Talent
{
	protected override void GetSavedData()
	{
		isUnlocked = SavedData.saveData.hasTerraformingTalent;
	}

	public override void UnlockTalent()
	{
		if(SavedData.saveData.unspentTalentPoints > 0 || SavedData.IS_DEBUGGING)
		{
			SavedData.saveData.unspentTalentPoints--;
			SavedData.saveData.hasTerraformingTalent = true;
			base.UnlockTalent();
		}
	}
}
