using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseDetector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	GameController GC;

	private void Start()
	{
		GC = FindObjectOfType<GameController>();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		GC.isMouseOverUI = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		GC.isMouseOverUI = false;
	}
}
