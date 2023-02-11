using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Enemy : MonoBehaviour
{
	public float resourceValue = .5f;

	//Orbit orbit;
	Vector3 startPosition;
	GameObject target;
	float lerpT = 0;
	public float speed = 1;
	public float deltaSpeed = .1f;
	public float ang;

	private void Start()
	{
		//orbit = gameObject.GetComponent<Orbit>();
		startPosition = transform.position;
		target = FindObjectOfType<Planet>().gameObject;
	}

	private void Update()
	{
		lerpT += Time.deltaTime * speed;
		speed += Time.deltaTime * deltaSpeed;
		transform.position = Vector3.Lerp(startPosition, target.transform.position, lerpT);

		transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position, new Vector3(0, 1, 0));
	}
}
