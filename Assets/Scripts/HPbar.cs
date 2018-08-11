using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPbar : MonoBehaviour {
	Transform hpSpawnPosition;
	public PlayerHP playerHPScript;
	public GameObject fullHeart;
	public GameObject halfHeart;
	public GameObject emptyHeart;
	int tmpHp;
	int fullHeartNum;
	int emptyHeartNum;
	bool isHalfExist = false;
	GameObject[] hearts;

	// Use this for initialization
	void Start () {
		/*if(GameObject.FindGameObjectWithTag("Player1")){
			playerHPScript = GameObject.FindGameObjectWithTag("Player1").GetComponent<PlayerHP>();
		}else if(GameObject.FindGameObjectWithTag("Player2")){
			playerHPScript = GameObject.FindGameObjectWithTag("Player2").GetComponent<PlayerHP>();	
		}*/
		hpSpawnPosition = GameObject.Find("HpSpawn").GetComponent<Transform>();
		DisplayHeart();
	}

	// Update is called once per frame
	void Update () {
		//tmpHp = playerControllerScript.hp;
		if(tmpHp != playerHPScript.hp)
		{
			//Debug.Log(playerControllerScript.hp);
			hearts = GameObject.FindGameObjectsWithTag("HP");
			foreach(GameObject go in hearts){
				Destroy(go);
			}
			DisplayHeart();		
		}
	}

	void DisplayHeart(){
		tmpHp = playerHPScript.hp;
		if( ( tmpHp % 2) == 1){
			isHalfExist = true;
		}else{
			isHalfExist = false;
		}
		fullHeartNum = tmpHp / 2;
		for(int i = 0; i < fullHeartNum; i++ ){
			Instantiate(fullHeart, hpSpawnPosition.position - new Vector3(2 * i, 0, 0), hpSpawnPosition.rotation);
		}
		if(isHalfExist){
			emptyHeartNum = 4 - fullHeartNum; 
			Instantiate(halfHeart, hpSpawnPosition.position - new Vector3(2 * fullHeartNum, 0, 0), hpSpawnPosition.rotation);
		}else{
			emptyHeartNum = 5 - fullHeartNum; 
		}
		for(int i = 0; i < emptyHeartNum; i++){
			Instantiate(emptyHeart, hpSpawnPosition.position + new Vector3(2 * (i - 4), 0, 0), hpSpawnPosition.rotation);
		}
	}
}
