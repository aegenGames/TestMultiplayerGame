using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
	private Transform target;

	void Start()
	{
		if (!target)
			target = Camera.main.transform;
	}

	void Update()
	{
		Rotation();
	}

	private void Rotation()
	{
		this.transform.LookAt(target);
	}
}
