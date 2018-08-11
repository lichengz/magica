using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GM_Random_Drops : NetworkBehaviour
{
    public bool g = false;
    bool gameOver;
    public GameObject hpSupply;
    // Use this for initialization

    // HP supply
    //[SyncVar] bool isHpDroped = false;
    float refreshRate;
    float nextRefresh;
    public override void OnStartServer()
    {
        //Spawner();
    }

    void Start()
    {
        refreshRate = Random.Range(0, 20);
        gameOver = GameObject.Find("GameMaster").GetComponent<GM_End>().gameOver;
    }
    void Update()
    {
        if (!g)
        {
            CanWeStart();
        }
        Spawner();

    }
    void Spawner()
    {
        if (!isServer)
        {
            return;
        }
        if (g && !gameOver)
        {
            SpawnHpSupply();
        }
    }

    void SpawnHpSupply()
    {
        refreshRate = Random.Range(0, 20);
        if (Time.time > nextRefresh)
        {
            nextRefresh = Time.time + refreshRate;
            GameObject go = (GameObject)Instantiate(hpSupply, new Vector3(Random.Range(-40f, 40f), 1f, Random.Range(-40f, 40f)), Quaternion.identity);
            //ClientScene.RegisterPrefab(go);
            NetworkServer.Spawn(go);
        }
    }

    void CanWeStart()
    {
        if (GameObject.Find("YuriSword_Player" + "(Clone)") != null
        && GameObject.Find("NaokoStaff_Player" + "(Clone)") != null)
        {
            g = true;
        }
    }
}
