using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolTip : MonoBehaviour
{
	ToolTipWindow toolTipWindow;
	GameController GC;

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
		if(!toolTipWindow.isActiveAndEnabled)
			toolTipWindow.SetupWindowAtMousePosition(this);
	}
}
