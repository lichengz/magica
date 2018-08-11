using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
	public Transform cameraTarget;

	// Use this for initialization
	void Start () {
		//cameraTarget = GameObject.FindGameObjectWithTag ("Player1").GetComponent<Transform> ();
		//cameraTarget = null;
	}

	// Update is called once per frame
	void Update () {
	
	}

	void LateUpdate()
	{
		if(transform != null)
			transform.position = cameraTarget.position + new Vector3(0, 60f, 35);
	}

}
