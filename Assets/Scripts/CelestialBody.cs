using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CelestialBody : MonoBehaviour
{
	public float gravityRadius;

	private void OnCollisionEnter(Collision collision)
	{
		if(collision.transform.gameObject.tag == "Enemy" || collision.transform.gameObject.tag == "Bullet" || collision.transform.gameObject.tag == "Mine")
		{
			Destroy(collision.transform.gameObject);
		}
	}
}
