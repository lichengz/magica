using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GM_End : NetworkBehaviour
{
    public GameObject end;
    // Use this for initialization
    public bool gameOver = false;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    //GameOverCanvas
    public void StopPlayer()
    {
        try
        {
            GameObject.Find("YuriSword_Player(Clone)").GetComponent<YuriController>().movementDisable = true;
            GameObject.Find("NaokoStaff_Player(Clone)").GetComponent<NaokoController>().movementDisable = true;
        }catch{}
        GameObject go = (GameObject)Instantiate(end, new Vector3(0, 0, 0), Quaternion.identity);
        //gameObject.GetComponent<GM_Random_Drops>().
        gameOver = true;
        NetworkServer.Spawn(go);
    }
}
