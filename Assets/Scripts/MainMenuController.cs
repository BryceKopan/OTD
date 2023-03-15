using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
	public Vector3 mainMenuPosition, preGamePosition;
	public GameObject mainMenuText, mainMenuButtons, preGameLeft, preGameRight, talentPanel, talentText;

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

		preGameLeft.transform.GetChild(1).GetChild(1).GetChild(2).GetComponent<Toggle>().SetIsOnWithoutNotify(SavedData.saveData.isHardMode);
		preGameLeft.transform.GetChild(1).GetChild(1).GetChild(3).GetComponent<Toggle>().SetIsOnWithoutNotify(SavedData.saveData.isQuickStartMode);
		preGameLeft.transform.GetChild(1).GetChild(1).GetChild(4).GetComponent<Toggle>().SetIsOnWithoutNotify(SavedData.saveData.isWildPatternsMode);
		preGameLeft.transform.GetChild(1).GetChild(1).GetChild(8).GetComponent<Toggle>().SetIsOnWithoutNotify(SavedData.saveData.isShieldWallMode);
		preGameLeft.transform.GetChild(1).GetChild(1).GetChild(10).GetComponent<Toggle>().SetIsOnWithoutNotify(SavedData.saveData.isBigThreatMode);
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
		Camera.main.transform.localPosition = mainMenuPosition;
	}

	public void StartGame()
	{
		SavedData.SaveFile();
		SceneManager.LoadScene(1);
	}

	public void ToggleTalents()
	{
		if(talentPanel.activeSelf)
			talentPanel.SetActive(false);
		else
			ShowTalents();
	}

	public void ToggleHardMode()
	{
		SavedData.saveData.isHardMode = !SavedData.saveData.isHardMode;
		preGameLeft.transform.GetChild(1).GetChild(1).GetChild(2).GetComponent<Toggle>().SetIsOnWithoutNotify(SavedData.saveData.isHardMode);
	}

	public void ToggleQuickstart()
	{
		SavedData.saveData.isQuickStartMode = !SavedData.saveData.isQuickStartMode;
		preGameLeft.transform.GetChild(1).GetChild(1).GetChild(3).GetComponent<Toggle>().SetIsOnWithoutNotify(SavedData.saveData.isQuickStartMode);
	}

	public void ToggleWildPatterns()
	{
		SavedData.saveData.isWildPatternsMode = !SavedData.saveData.isWildPatternsMode;
		preGameLeft.transform.GetChild(1).GetChild(1).GetChild(4).GetComponent<Toggle>().SetIsOnWithoutNotify(SavedData.saveData.isWildPatternsMode);
	}

	public void ToggleShieldWall()
	{
		SavedData.saveData.isShieldWallMode = !SavedData.saveData.isShieldWallMode;
		preGameLeft.transform.GetChild(1).GetChild(1).GetChild(8).GetComponent<Toggle>().SetIsOnWithoutNotify(SavedData.saveData.isShieldWallMode);
	}

	public void ToggleBigThreat()
	{
		SavedData.saveData.isBigThreatMode = !SavedData.saveData.isBigThreatMode;
		preGameLeft.transform.GetChild(1).GetChild(1).GetChild(10).GetComponent<Toggle>().SetIsOnWithoutNotify(SavedData.saveData.isBigThreatMode);
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
}
