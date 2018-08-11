using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
 
public class GM_ChooseHero : MonoBehaviour {
 
    public GameObject characterSelect;
 
    public void PickHero(int hero)
    {
        NetworkManager.singleton.GetComponent<NetworkManagerCustom>().chosenCharacter = hero;
        characterSelect.SetActive(false);
        //GameObject.Find("WelcomePanel").SetActive(false);
    }
}