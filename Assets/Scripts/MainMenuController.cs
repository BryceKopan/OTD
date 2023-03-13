using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
	public Vector3 mainMenuPosition, preGamePosition;
	public GameObject mainMenuText, mainMenuButtons, preGameLeft, preGameRight;

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
		Camera.main.transform.localPosition = new Vector3(0, Camera.main.transform.position.y, 0); ;
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
}
