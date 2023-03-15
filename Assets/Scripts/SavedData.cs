using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public static class SavedData
{
	//TODO add saving and loading funtionality

	public static int popLevel = 0, currentPopXP = 0;
	public static bool isQuickStartMode = false, isWildPatternsMode = false, isShieldWallMode = false, isBigThreatMode = false, isHardMode = false;
}
