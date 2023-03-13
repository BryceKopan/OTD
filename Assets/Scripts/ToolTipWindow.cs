using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToolTipWindow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public ToolTip currentToolTip;

	public GameObject towerName, towerRank, towerRankProgress, towerRange, towerAttackSpeed;
	public GameObject sentryStats, missleStats, mineStats, towerHealth;

	bool isMouseOver = false;
	Canvas canvas;

	private void Start()
	{
		canvas = FindObjectOfType<Canvas>();
	}

	private void OnEnable()
	{
		canvas = FindObjectOfType<Canvas>();
	}

	//Is this needed?
	private void Update()
	{
		if(!isMouseOver)
			gameObject.SetActive(false);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		isMouseOver = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		isMouseOver = false;
		gameObject.SetActive(false);
	}

	public void SetupWindowAtMousePosition(ToolTip newToolTip)
	{
		gameObject.SetActive(true);
		missleStats.SetActive(false);
		mineStats.SetActive(false);
		sentryStats.SetActive(false);
		currentToolTip = newToolTip;

		Vector2 pos;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out pos);
		transform.position = canvas.transform.TransformPoint(pos);

		Tower tower = currentToolTip.GetComponent<Tower>();
		if(tower)
		{
			towerName.GetComponent<Text>().text = tower.displayName;
			towerRank.GetComponent<Text>().text = "Rank: " + tower.Rank;
			towerRankProgress.GetComponent<Text>().text = "Rank Progress: " + tower.rankProgress * 100 + "%";
			towerRange.GetComponent<Text>().text = "Range: " + tower.Range;
			towerAttackSpeed.GetComponent<Text>().text = "Speed: " + 1 / tower.cooldown + "/s";
			towerHealth.GetComponent<Text>().text = tower.Health + "/" + tower.maxHealth;
		}

		if(currentToolTip.GetComponent<MissleBattery>())
		{
			missleStats.SetActive(true);
			MissleBattery mb = currentToolTip.GetComponent<MissleBattery>();
			missleStats.transform.GetChild(0).GetComponent<Text>().text = "Missles: " + mb.baseMissles + "<color=#FFBD00> + " + (mb.Rank * mb.missleGrowth) + "</color>";
		}
		else if(currentToolTip.GetComponent<MineLayer>())
		{
			mineStats.SetActive(true);
			MineLayer ml = currentToolTip.GetComponent<MineLayer>();
			mineStats.transform.GetChild(0).GetComponent<Text>().text = "Density Limit: " + ml.baseMaxMineDensity + "<color=#FFBD00> + " + (ml.Rank / 3) + "</color>";
		}
		else if(currentToolTip.GetComponent<Railgun>())
		{
			Railgun rl = currentToolTip.GetComponent<Railgun>();
			towerRange.GetComponent<Text>().text = "Range: " + tower.BaseRange + "<color=#FFBD00> + " + (rl.Rank * rl.rangeGrowthPercent / 100) + "</color>";
		}
		else if(currentToolTip.GetComponent<SentryTower>())
		{
			sentryStats.SetActive(true);
			SentryTower st = currentToolTip.GetComponent<SentryTower>();
			sentryStats.transform.GetChild(0).GetComponent<Text>().text = "Sentry Limit: " + 2 + "<color=#FFBD00> + " + (st.Rank / 2) + "</color>";
		}
		else if(currentToolTip.GetComponent<MiningTower>())
		{
			
		}
		else if(currentToolTip.GetComponent<LaserTower>())
		{
			LaserTower lt = currentToolTip.GetComponent<LaserTower>();
			towerAttackSpeed.GetComponent<Text>().text = "<color=#FFBD00>Speed: " + 1 / lt.cooldown + "/s</color>";
		}
	}
}
