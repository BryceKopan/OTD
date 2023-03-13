using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
	public Color shield1Color, shield2Color, shield3Color;

	SpriteRenderer shieldSprite;

	private int shieldStrength = 0;
	public int ShieldStrength
	{
		get { return shieldStrength; }
		set
		{
			shieldStrength = value;

			switch(shieldStrength)
			{
				case 0:
					gameObject.SetActive(false);
					break;
				case 1:
					gameObject.SetActive(true);
					shieldSprite.color = shield1Color;
					break;
				case 2:
					gameObject.SetActive(true);
					shieldSprite.color = shield2Color;
					break;
				case 3:
					gameObject.SetActive(true);
					shieldSprite.color = shield3Color;
					break;
			}
		}
	}

	private void OnEnable()
	{
		shieldSprite = GetComponentInChildren<SpriteRenderer>();
	}
}
