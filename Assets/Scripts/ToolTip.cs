using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum ToolTipType
{
	Tower,
	TowerPreview,
	Enemy,
	General,
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

	private void Update()
	{
		if(isMouseOver)
			ActivateTooltip();
	}

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
		toolTipWindow.SetupWindowAtMousePosition(this);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		isMouseOver = true;

	}

	public void OnPointerExit(PointerEventData eventData)
	{
		isMouseOver = false;
	}

	private void OnDisable()
	{
		isMouseOver = false;
	}
}
