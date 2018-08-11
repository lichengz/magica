using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateNetworth : MonoBehaviour {

	public PlayerNetWorth playerNetWorthScript;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		GameObject.Find("NetWorth").GetComponent<Text>().text = "Networth: " + playerNetWorthScript.netWorh.ToString();
	}

}
