using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
	public List<GameObject> tutorialWindows;
	List<bool> isWindowUsed;
	GameController GC;
	public bool isTechResearched = false;

	private void Start()
	{
		GC = FindObjectOfType<GameController>();
		isWindowUsed = new List<bool>(new bool[tutorialWindows.Count]);
		isWindowUsed[0] = true;
	}

	private void Update()
	{
		if(!tutorialWindows[0].activeSelf && isWindowUsed[0] && !isWindowUsed[1])
		{
			tutorialWindows[1].SetActive(true);
			isWindowUsed[1] = true;
		}

		if(!tutorialWindows[1].activeSelf && isWindowUsed[1] && !isWindowUsed[2])
		{
			tutorialWindows[2].SetActive(true);
			isWindowUsed[2] = true;
		}

		if(Time.timeScale > 0 && !isWindowUsed[3])
		{
			tutorialWindows[3].SetActive(true);
			isWindowUsed[3] = true;
		}

		if(!tutorialWindows[3].activeSelf && isWindowUsed[3] && !isWindowUsed[4])
		{
			tutorialWindows[4].SetActive(true);
			isWindowUsed[4] = true;
		}

		if(GC.planetDetail.activeSelf && !isWindowUsed[5])
		{
			tutorialWindows[5].SetActive(true);
			isWindowUsed[5] = true;
		}

		if(!tutorialWindows[5].activeSelf && isWindowUsed[5] && !isWindowUsed[6])
		{
			tutorialWindows[6].SetActive(true);
			isWindowUsed[6] = true;
		}

		if(isTechResearched && !isWindowUsed[7])
		{
			tutorialWindows[7].SetActive(true);
			isWindowUsed[7] = true;
		}

		if(!tutorialWindows[7].activeSelf && isWindowUsed[7] && !isWindowUsed[8])
		{
			tutorialWindows[8].SetActive(true);
			isWindowUsed[8] = true;
		}

		if(!tutorialWindows[8].activeSelf && isWindowUsed[8] && !isWindowUsed[9])
		{
			tutorialWindows[9].SetActive(true);
			isWindowUsed[9] = true;
			SavedData.saveData.isTutorialMode = false;
		}
	}
}
