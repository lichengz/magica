using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelect : MonoBehaviour
{
    GM_ChooseHero gmChooseHeroScript;
    StartMenu startMenu;
    // public bool userNameEntered = false;
    // // Use this for initialization
    void Start()
    {
        gmChooseHeroScript = GameObject.Find("CharacterSelect").GetComponent<GM_ChooseHero>();
        startMenu = GameObject.Find("WelcomePanel").GetComponent<StartMenu>();
    }

    // // Update is called once per frame
    // void Update () {
    // 	if(Input.GetKeyDown(KeyCode.Alpha1) && userNameEntered){
    // 		gmChooseHeroScript.PickHero(0);
    // 	}
    // 	if(Input.GetKeyDown(KeyCode.Alpha2) && userNameEntered){
    // 		gmChooseHeroScript.PickHero(1);
    // 	}
    // }
    public void PickNaoko()
    {
        gmChooseHeroScript.PickHero(0);
        startMenu.ChooseHostOrClient();
    }
    public void PickYuri()
    {
        gmChooseHeroScript.PickHero(1);
        startMenu.ChooseHostOrClient();
    }
}
