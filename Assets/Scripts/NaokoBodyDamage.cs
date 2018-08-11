using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NaokoBodyDamage : MonoBehaviour {
	//[SyncVar]
	PlayerHP playerHPScript;
	public GameObject body;
	Color tmpColor;

	// Use this for initialization
	void Start () {
		playerHPScript = GameObject.Find("NaokoStaff_Player" + "(Clone)").GetComponent<PlayerHP>();
		tmpColor = body.GetComponent<Renderer>().material.color;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.name == "Excalibur"){
			//playerHPScript.hp -= 1;
			//Debug.Log(playerHPScript.hp);
			body.GetComponent<Renderer>().material.color = new Color(3, 3, 3);
		}
		Invoke("ChangeColorBack", 0.1f);
	}

	void ChangeColorBack(){
		body.GetComponent<Renderer>().material.color = tmpColor;
	}

}
