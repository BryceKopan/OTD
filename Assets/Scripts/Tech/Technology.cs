using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Technology : MonoBehaviour
{
	public int researchCost;

	public abstract void AddTechnologyTo(Tower tower);
	public abstract void RemoveTechnologyFrom(Tower tower);
}
