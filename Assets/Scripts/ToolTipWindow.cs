using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ToolTipWindow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	public ToolTip currentToolTip;

	public GameObject towerWindow, towerName, towerRank, towerRankProgress, towerRange, towerAttackSpeed, sentryStats, missleStats, mineStats, towerHealth;

	public GameObject enemyWindow;
	public Text enemyHealth, enemyModifiers;
	public Text[] modifierSlots;

	public GameObject towerPreviewWindow;
	public Text towerPreviewName, towerPreviewCost;

	public GameObject generalWindow;
	public Text generalText;

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

	private void Update()
	{
		if(!isMouseOver && currentToolTip && !currentToolTip.isMouseOver)
		{
			gameObject.SetActive(false);

			OrbitalMenu om = FindObjectOfType<OrbitalMenu>();
			if(currentToolTip.type == ToolTipType.TowerPreview && !om.isMouseOver)
			{
				om.IsSelectingTower = false;
			}
		}

		if(currentToolTip == null)
			gameObject.SetActive(false);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		isMouseOver = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		isMouseOver = false;
	}

	public void SetupWindowAtMousePosition(ToolTip newToolTip)
	{
		gameObject.SetActive(true);
		towerWindow.SetActive(false);
		missleStats.SetActive(false);
		mineStats.SetActive(false);
		sentryStats.SetActive(false);

		enemyWindow.SetActive(false);
		enemyModifiers.gameObject.SetActive(false);
		foreach(Text modifierSlot in modifierSlots)
		{
			modifierSlot.gameObject.SetActive(false);
		}

		towerPreviewWindow.SetActive(false);
		generalWindow.SetActive(false);

		currentToolTip = newToolTip;

		Vector2 pos;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out pos);
		transform.position = canvas.transform.TransformPoint(pos);

		switch(currentToolTip.type)
		{
			case ToolTipType.Tower:
				SetupTowerToolTip();
				break;
			case ToolTipType.Enemy:
				SetupEnemyToolTip();
				break;
			case ToolTipType.TowerPreview:
				SetupTowerPreviewToolTip();
				break;
			case ToolTipType.General:
				SetupGeneralToolTip();
				break;
		}
	}
	
	void SetupTowerToolTip()
	{
		towerWindow.SetActive(true);

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
			Debug.Log("Not IMplemented, will mining stay in the game?");
		}
		else if(currentToolTip.GetComponent<LaserTower>())
		{
			LaserTower lt = currentToolTip.GetComponent<LaserTower>();
			towerAttackSpeed.GetComponent<Text>().text = "<color=#FFBD00>Speed: " + 1 / lt.cooldown + "/s</color>";
		}
	}

	void SetupEnemyToolTip()
	{
		enemyWindow.SetActive(true);

		Enemy enemy= currentToolTip.GetComponent<Enemy>();
		if(enemy)
		{
			enemyHealth.text = enemy.Health + "/" + enemy.maxHealth;

			//Has Modifier
			if(enemy.modifiers.Count > 0)
			{
				enemyModifiers.gameObject.SetActive(true);
				int modifierSlotIndex = 0;
				if(enemy.modifiers.Contains(EnemyModifier.Shielded))
				{
					modifierSlots[modifierSlotIndex].gameObject.SetActive(true);
					modifierSlots[modifierSlotIndex].color = enemy.shieldedColor;
					modifierSlots[modifierSlotIndex].text = "Shielded " + enemy.shield.ShieldStrength + " / 3";
					modifierSlotIndex++;
				}
				if(enemy.modifiers.Contains(EnemyModifier.Bursting))
				{
					modifierSlots[modifierSlotIndex].gameObject.SetActive(true);
					modifierSlots[modifierSlotIndex].color = enemy.burstingColor;
					modifierSlots[modifierSlotIndex].text = "Bursting";
					modifierSlotIndex++;
				}
				if(enemy.modifiers.Contains(EnemyModifier.Spawning))
				{
					modifierSlots[modifierSlotIndex].gameObject.SetActive(true);
					modifierSlots[modifierSlotIndex].color = enemy.spawningColor;
					modifierSlots[modifierSlotIndex].text = "Spawning";
					modifierSlotIndex++;
				}
			}
		}
	}

	public void SetupTowerPreviewToolTip()
	{
		towerPreviewWindow.SetActive(true);

		towerPreviewName.text = currentToolTip.towerPreviewName;
		towerPreviewCost.text = "Cost: " + currentToolTip.towerPreviewCost;
	}

	public void SetupGeneralToolTip()
	{
		generalWindow.SetActive(true);
		generalText.text = currentToolTip.generalToolTipText;
	}
}
