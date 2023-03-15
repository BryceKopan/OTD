using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum ToolTipType
{
	Tower,
	TowerPreview,
	Enemy,
	Research,
	Talent
}

public class ToolTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public ToolTipType type;
	ToolTipWindow toolTipWindow;
	GameController GC;

	public string generalToolTipText;

	//Temp Tower Preview Setting
	public string towerPreviewName;
	public int towerPreviewCost;

	public bool isMouseOver = false;

	private void Start()
	{
		GC = FindObjectOfType<GameController>();
		toolTipWindow = GC.tooltip.GetComponent<ToolTipWindow>();
	}

	private void OnEnable()
	{
		GC = FindObjectOfType<GameController>();
		toolTipWindow = GC.tooltip.GetComponent<ToolTipWindow>(); ;
	}

	public void ActivateTooltip()
	{
		if(!toolTipWindow.gameObject.activeSelf)
			toolTipWindow.SetupWindowAtMousePosition(this);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		isMouseOver = true;
		ActivateTooltip();
	}


	public void OnPointerExit(PointerEventData eventData)
	{
		isMouseOver = false;
	}
}
