using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Technology : MonoBehaviour
{
	public int researchCost;
	private int researchProgress;
	public int ResearchProgress
	{
		get { return researchProgress; }
		set
		{
			researchProgress = value;
			uiProgress.GetComponent<UnityEngine.UI.Text>().text = ResearchProgress + "/" + researchCost;
			if(researchProgress >= researchCost)
			{
				isResearched = true;
				FindObjectOfType<TechController>().SelectedTechnology = null;
			}
		}
	}

	public bool isResearched = false;
	public string techName;

	public GameObject uiProgress;

	public virtual void AddTechnologyTo(Tower tower) { }
	public virtual void RemoveTechnologyFrom(Tower tower) { }
}
