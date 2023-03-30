using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct GameMode
{
	public Toggle toggle;
	public string tooltipMessage;
	public int unlockLevel;
}

public class MainMenuController : MonoBehaviour
{
	public Vector3 mainMenuPosition, preGamePosition;
	public GameObject mainMenuText, mainMenuButtons, preGameLeft, preGameRight, talentPanel, talentText;
	public Toggle originEarthToggle, originMarsToggle;

	public List<GameMode> gameModes;

	private bool isInPreGame = false;

    public void NewGame()
	{
		TransitionToPreGameScreen();
	}

	public void TransitionToPreGameScreen()
	{
		isInPreGame = true;
		mainMenuText.SetActive(false);
		mainMenuButtons.SetActive(false);
		preGameLeft.SetActive(true);
		preGameRight.SetActive(true);
		preGameRight.transform.GetChild(4).GetComponent<Text>().text = "Human " + SavedData.saveData.popLevel;
		Camera.main.transform.localPosition = new Vector3(0, Camera.main.transform.position.y, 0);

		gameModes[0].toggle.SetIsOnWithoutNotify(SavedData.saveData.isTutorialMode);
		gameModes[2].toggle.SetIsOnWithoutNotify(SavedData.saveData.isHardMode);
		gameModes[4].toggle.SetIsOnWithoutNotify(SavedData.saveData.isQuickStartMode);
		gameModes[1].toggle.SetIsOnWithoutNotify(SavedData.saveData.isWildPatternsMode);
		gameModes[6].toggle.SetIsOnWithoutNotify(SavedData.saveData.isShieldWallMode);
		gameModes[8].toggle.SetIsOnWithoutNotify(SavedData.saveData.isBigThreatMode);

		originEarthToggle.SetIsOnWithoutNotify(SavedData.saveData.originIsEarth);
		originMarsToggle.SetIsOnWithoutNotify(SavedData.saveData.originIsMars);

		UpdateTalentEffectedUI();
		UpdateGameModes();
	}

	public void Quit()
	{
		Application.Quit();
	}
	
	public void BackToMainMenu()
	{
		isInPreGame = false;
		mainMenuText.SetActive(true);
		mainMenuButtons.SetActive(true);
		preGameLeft.SetActive(false);
		preGameRight.SetActive(false);
		Camera.main.transform.localPosition = mainMenuPosition;
	}

	public void StartGame()
	{
		SavedData.SaveFile();
		SceneManager.LoadScene(1);
	}

	private void ResetOrigin()
	{
		SavedData.saveData.originIsEarth = false;
		originEarthToggle.SetIsOnWithoutNotify(false);
		SavedData.saveData.originIsMars = false;
		originMarsToggle.SetIsOnWithoutNotify(false);
	}

	public void SetOriginToEarth()
	{
		ResetOrigin();
		SavedData.saveData.originIsEarth = true;
		originEarthToggle.SetIsOnWithoutNotify(true);
	}

	public void SetOriginToMArs()
	{
		ResetOrigin();
		SavedData.saveData.originIsMars = true;
		originMarsToggle.SetIsOnWithoutNotify(true);
	}

	public void ToggleTalents()
	{
		if(talentPanel.activeSelf)
		{
			UpdateTalentEffectedUI();
			talentPanel.SetActive(false);
		}
		else
			ShowTalents();
	}

	public void UpdateTalentEffectedUI()
	{
		if(SavedData.saveData.hasTerraformingTalent)
			originMarsToggle.interactable = true;
		else
			originMarsToggle.interactable = false;
	}

	public void ToggleHardMode()
	{
		SavedData.saveData.isHardMode = !SavedData.saveData.isHardMode;
		gameModes[2].toggle.SetIsOnWithoutNotify(SavedData.saveData.isHardMode);
	}

	public void ToggleQuickstart()
	{
		SavedData.saveData.isQuickStartMode = !SavedData.saveData.isQuickStartMode;
		gameModes[4].toggle.SetIsOnWithoutNotify(SavedData.saveData.isQuickStartMode);
	}

	public void ToggleWildPatterns()
	{
		SavedData.saveData.isWildPatternsMode = !SavedData.saveData.isWildPatternsMode;
		gameModes[1].toggle.SetIsOnWithoutNotify(SavedData.saveData.isWildPatternsMode);
	}

	public void ToggleShieldWall()
	{
		SavedData.saveData.isShieldWallMode = !SavedData.saveData.isShieldWallMode;
		gameModes[6].toggle.SetIsOnWithoutNotify(SavedData.saveData.isShieldWallMode);
	}

	public void ToggleBigThreat()
	{
		SavedData.saveData.isBigThreatMode = !SavedData.saveData.isBigThreatMode;
		gameModes[8].toggle.SetIsOnWithoutNotify(SavedData.saveData.isBigThreatMode);
	}

	public void ToggleTutorial()
	{
		SavedData.saveData.isTutorialMode = !SavedData.saveData.isTutorialMode;
		gameModes[0].toggle.SetIsOnWithoutNotify(SavedData.saveData.isTutorialMode);
	}

	public void ResetSaveData()
	{
		SavedData.saveData = new DataHolder(false);
		SavedData.SaveFile();
	}

	public void ShowTalents()
	{
		talentPanel.SetActive(true);

		talentText.transform.GetChild(1).GetComponent<Text>().text = "Pop Lvl: " + SavedData.saveData.popLevel + "\n Unspent Points: " + SavedData.saveData.unspentTalentPoints;
	}

	public void UpdateGameModes()
	{
		foreach(GameMode gm in gameModes)
		{
			if(SavedData.saveData.popLevel >= gm.unlockLevel || SavedData.IS_DEBUGGING)
			{
				gm.toggle.interactable = true;
				gm.toggle.GetComponent<ToolTip>().generalToolTipText = gm.tooltipMessage;
			}
			else
			{
				gm.toggle.interactable = false;
				gm.toggle.GetComponent<ToolTip>().generalToolTipText = "Unlock by reaching pop lvl " + gm.unlockLevel;
			}
		}
	}
}
