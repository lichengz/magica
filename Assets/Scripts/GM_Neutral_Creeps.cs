using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GM_Neutral_Creeps : NetworkBehaviour
{
    public GameObject white_front_creep, black_front_creep;
    float creepSpawnRate = 15f;
    float nextSpawnTime = 0;
    public Transform whiteBase, blackBase;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(!isServer) return;
        if (Time.time > nextSpawnTime)
        {
            nextSpawnTime += creepSpawnRate;
            StartCoroutine(SpawnAWaveOfCreeps(white_front_creep, whiteBase));
            StartCoroutine(SpawnAWaveOfCreeps(black_front_creep, blackBase));
        }
    }

    public override void OnStartServer()
    {
        // GameObject go = (GameObject)Instantiate(creep, whiteBase.position + new Vector3(-1, 2, -1), Quaternion.identity);
        // //ClientScene.RegisterPrefab(go);
        // NetworkServer.Spawn(go);
    }
    IEnumerator SpawnAWaveOfCreeps(GameObject creep, Transform spawnPoint)
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject go = (GameObject)Instantiate(creep, spawnPoint.position + new Vector3(0, 2, 0), Quaternion.identity);
            NetworkServer.Spawn(go);
            if (spawnPoint == whiteBase)
            {
                go.tag = "White Creep";
            }
            if (spawnPoint == blackBase)
            {
                go.tag = "Black Creep";
            }
            //go.tag = "White Creep";
            yield return new WaitForSeconds(1);
        }
    }
}
