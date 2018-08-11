using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YuriBodyDamage : MonoBehaviour {
	PlayerHP playerHPScript;
	public GameObject body;
	Color tmpColor;
	// Use this for initialization
	void Start () {
		playerHPScript = GameObject.Find("YuriSword_Player" + "(Clone)").GetComponent<PlayerHP>();
		tmpColor = body.GetComponent<Renderer>().material.color;
	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnTriggerEnter(Collider other)
	{
		if(other.name == "Naoko_MagicBall(Clone)"){
			//playerHPScript.hp -= 1;
			body.GetComponent<Renderer>().material.color = new Color(3, 3, 3);
		}
		Invoke("ChangeColorBack", 0.1f);
	}

	void ChangeColorBack(){
		body.GetComponent<Renderer>().material.color = tmpColor;
	}

}
