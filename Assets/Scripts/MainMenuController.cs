using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
	public Vector3 mainMenuPosition, preGamePosition;
	public GameObject mainMenuText, mainMenuButtons, preGameLeft, preGameRight, talentPanel;

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
		Camera.main.transform.localPosition = new Vector3(0, Camera.main.transform.position.y, 0);
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
		SceneManager.LoadScene(1);
	}

	public void ShowTalents()
	{
		if(talentPanel.activeSelf)
			talentPanel.SetActive(false);
		else
			talentPanel.SetActive(true);
	}

	public void ToggleHardMode()
	{
		SavedData.isHardMode = !SavedData.isHardMode;
		preGameLeft.transform.GetChild(1).GetChild(1).GetChild(2).GetComponent<Toggle>().isOn = SavedData.isHardMode;
	}

	public void ToggleQuickstart()
	{
		SavedData.isQuickStartMode = !SavedData.isQuickStartMode;
		preGameLeft.transform.GetChild(1).GetChild(1).GetChild(3).GetComponent<Toggle>().isOn = SavedData.isQuickStartMode;
	}

	public void ToggleWildPatterns()
	{
		SavedData.isWildPatternsMode = !SavedData.isWildPatternsMode;
		preGameLeft.transform.GetChild(1).GetChild(1).GetChild(4).GetComponent<Toggle>().isOn = SavedData.isWildPatternsMode;
	}

	public void ToggleShieldWall()
	{
		SavedData.isShieldWallMode = !SavedData.isShieldWallMode;
		preGameLeft.transform.GetChild(1).GetChild(1).GetChild(8).GetComponent<Toggle>().isOn = SavedData.isShieldWallMode;
	}

	public void ToggleBigThreat()
	{
		SavedData.isBigThreatMode = !SavedData.isBigThreatMode;
		preGameLeft.transform.GetChild(1).GetChild(1).GetChild(10).GetComponent<Toggle>().isOn = SavedData.isBigThreatMode;
	}
}
