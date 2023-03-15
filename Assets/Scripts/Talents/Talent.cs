using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Talent : MonoBehaviour
{
	public bool isUnlocked;
	public Talent preReqTalent;
	public Button talentButton;

	private void Start()
	{
		GetSavedData();

		if(preReqTalent != null && !preReqTalent.isUnlocked)
			talentButton.interactable = false;
		else
			talentButton.interactable = true;

		if(isUnlocked)
		{
			talentButton.transform.GetChild(0).GetComponent<Text>().color = new Color(0, 207, 255);
			talentButton.interactable = false;
		}
	}

	protected virtual void GetSavedData() { }
	public virtual void UnlockTalent()
	{
		isUnlocked = true;
		talentButton.transform.GetChild(0).GetComponent<Text>().color = new Color(0, 207, 255);
		talentButton.interactable = false;

		FindObjectOfType<MainMenuController>().ShowTalents();
	}

	private void Update()
	{
		if(preReqTalent != null && preReqTalent.isUnlocked && !isUnlocked)
			talentButton.interactable = true;
	}
}
