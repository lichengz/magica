using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    public GameObject ChooseNaokoButton;
    public GameObject ChooseYuriButton;
    public GameObject HostButton;
    public GameObject ClientButton;
	public GameObject HostIP;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void SubmitButtonClicked()
    {
        GameObject.Find("UserName").SetActive(false);
        GameObject.Find("NameInstruction").SetActive(false);
        GameObject.Find("CharactorInstructions").GetComponent<Text>().enabled = true;
        //GameObject.Find("CharacterSelect").GetComponent<CharacterSelect>().userNameEntered = true;
        GameObject.Find("NameSubmit").SetActive(false);
        ChooseNaokoButton.SetActive(true);
        ChooseYuriButton.SetActive(true);
    }
    public void ChooseHostOrClient()
    {
        ChooseNaokoButton.SetActive(false);
        ChooseYuriButton.SetActive(false);
        GameObject.Find("CharactorInstructions").GetComponent<Text>().text = "Connect as a Host or a Client";
        HostButton.SetActive(true);
        ClientButton.SetActive(true);
		HostIP.SetActive(true);
    }
}
