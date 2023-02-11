using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class BillboardSprite : MonoBehaviour
{
	void LateUpdate()
	{
		transform.LookAt(Camera.main.transform.position, -Vector3.up);
	}
}
