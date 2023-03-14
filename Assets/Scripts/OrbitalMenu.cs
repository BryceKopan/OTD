using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OrbitalMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public GameObject[] orbitalButtons;

	private bool isSelectingTower = false;
	public bool IsSelectingTower
	{
		get { return isSelectingTower; }
		set
		{
			isSelectingTower = value;

			if(isSelectingTower)
				gameObject.SetActive(true);
			else
				gameObject.SetActive(false);
		}
	}

	public bool isMouseOver = false;
	public Canvas canvas;
	public GameController GC;

	private void Update()
	{
		if(IsSelectingTower)
			UpdateUI();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		isMouseOver = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		isMouseOver = false;
		if(GC.tooltip.GetComponent<ToolTipWindow>().currentToolTip.type != ToolTipType.TowerPreview || !GC.tooltip.activeSelf)
		{
			IsSelectingTower = false;
		}
	}

	public void OpenTowerMenu()
	{
		IsSelectingTower = true;

		Vector2 pos;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out pos);
		transform.position = canvas.transform.TransformPoint(pos);
	}

	public void UpdateUI()
	{
		//Buttons that grey out from cost
		orbitalButtons[0].transform.GetChild(0).GetComponent<Button>().interactable = 
			orbitalButtons[0].transform.GetChild(1).GetComponent<Button>().interactable = 
			(GC.resources >= GC.towers[0].prefab.GetComponent<Tower>().resourceCost) && GC.towers[0].isUnlocked;

		orbitalButtons[1].transform.GetChild(0).GetComponent<Button>().interactable = 
			orbitalButtons[1].transform.GetChild(1).GetComponent<Button>().interactable = 
			(GC.resources >= GC.towers[1].prefab.GetComponent<Tower>().resourceCost) && GC.towers[1].isUnlocked;

		orbitalButtons[2].transform.GetChild(0).GetComponent<Button>().interactable = 
			orbitalButtons[2].transform.GetChild(1).GetComponent<Button>().interactable = 
			(GC.resources >= GC.towers[2].prefab.GetComponent<Tower>().resourceCost) && GC.towers[2].isUnlocked;

		orbitalButtons[3].transform.GetChild(0).GetComponent<Button>().interactable = 
			orbitalButtons[3].transform.GetChild(1).GetComponent<Button>().interactable = 
			(GC.resources >= GC.towers[3].prefab.GetComponent<Tower>().resourceCost) && GC.towers[3].isUnlocked;

		orbitalButtons[4].transform.GetChild(0).GetComponent<Button>().interactable = 
			orbitalButtons[4].transform.GetChild(1).GetComponent<Button>().interactable = 
			(GC.resources >= GC.towers[4].prefab.GetComponent<Tower>().resourceCost) && GC.towers[4].isUnlocked;

		orbitalButtons[5].transform.GetChild(0).GetComponent<Button>().interactable = 
			orbitalButtons[5].transform.GetChild(1).GetComponent<Button>().interactable = 
			(GC.resources >= GC.towers[5].prefab.GetComponent<Tower>().resourceCost) && GC.towers[5].isUnlocked;
	}
}
