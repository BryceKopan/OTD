using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public struct DataHolder
{
	public int popLevel, unspentTalentPoints, currentPopXP;
	public bool isQuickStartMode, isWildPatternsMode, isShieldWallMode, isBigThreatMode, isHardMode;
	public bool hasGrowthVatsTalent, hasEfficientArchitecureTalent, hasPlannedWorldTalent, hasLaserDefense1Talent, hasLaserDefense2Talent, hasLaserDefense3Talent, hasIonEngineTalent, hasTerraformingTalent;

	public DataHolder(bool stuff)
	{
		popLevel = 0;
		unspentTalentPoints = 0;
		currentPopXP = 0;
		isQuickStartMode = false;
		isWildPatternsMode = false;
		isShieldWallMode = false;
		isBigThreatMode = false;
		isHardMode = false;
		hasGrowthVatsTalent = false;
		hasEfficientArchitecureTalent = false;
		hasPlannedWorldTalent = false;
		hasLaserDefense1Talent = false;
		hasLaserDefense2Talent = false;
		hasLaserDefense3Talent = false;
		hasIonEngineTalent = false;
		hasTerraformingTalent = false;
	}
}


public static class SavedData
{
	public static DataHolder saveData;
	public static bool IS_DEBUGGING = false;

	public static void SaveFile()
	{
		string destination = Application.persistentDataPath + "/save.dat";
		FileStream file;

		if(File.Exists(destination)) file = File.OpenWrite(destination);
		else file = File.Create(destination);

		BinaryFormatter bf = new BinaryFormatter();
		bf.Serialize(file, saveData);
		file.Close();
	}

	public static void LoadFile()
	{
		string destination = Application.persistentDataPath + "/save.dat";
		FileStream file;

		if(File.Exists(destination)) file = File.OpenRead(destination);
		else
		{
			saveData = new DataHolder(true);
			return;
		}

		BinaryFormatter bf = new BinaryFormatter();
		saveData = (DataHolder)bf.Deserialize(file);
		file.Close();
	}
}
