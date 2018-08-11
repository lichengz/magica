using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class  BossController: MonoBehaviour {
	public int bossHP = 50;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	void OnCollisionEnter(Collision other)
	{
		//GetComponent<MeshRenderer>().material.color = Color.white;
		//Invoke("SetBossBackToRed", 0.1f);
		if(other.gameObject.CompareTag("Attack"))
		{
			bossHP -= 1;
		}
	}
	
	void SetBossBackToRed(){
		GetComponent<MeshRenderer>().material.color = Color.red;
	}
}
