using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
	public float minResourceCount, maxResourceCount;
	public float resourceCount;

	private void Start()
	{
		resourceCount = Random.Range(minResourceCount, maxResourceCount);
	}

	public void GatherResources(float f)
	{
		resourceCount -= f;
		if(resourceCount <= 0)
			Destroy(gameObject);
	}
}
