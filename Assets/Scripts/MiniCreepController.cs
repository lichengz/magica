using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;


public class MiniCreepController : NetworkBehaviour
{
    private Transform myTransform;
    private NavMeshAgent myNav;
    private Collider[] hitColliders;
    private float checkRate;
    private float nextCheck;
    private float detectionRadius = 10;
    public LayerMask White_BattleLayer, Black_BattleLayer;
    //Choose Attack Target
    public GameObject playerTarget = null;
    GameObject whiteBase, blackBase;
    public bool findTarget;

    // Attack
    float attackRate = 2;
    float nextAttack;
    float minDistance = 4;
    float currentDistance;
    // [SerializeField] private Material white;
    // [SerializeField] private Material red;
    //(hook = "OnChangeHealth")
    [SyncVar] public int creepHp = 3;
    public RectTransform healthBar;

    // Drop Coins
    public GameObject coin;

    // Use this for initialization
    void Start()
    {
        SetInitRef();
        if (isServer)
        {
            StartCoroutine(Attack());
        }
        if (gameObject.tag == "White Creep")
        {
            playerTarget = blackBase;
        }
        if (gameObject.tag == "Black Creep")
        {
            playerTarget = whiteBase;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.tag == "White Creep")
        {
            DetectAttackTarget("Black Creep", Black_BattleLayer);
        }
        if (gameObject.tag == "Black Creep")
        {
            DetectAttackTarget("White Creep", White_BattleLayer);
        }
        if (creepHp == 0)
        {
            SpawnCoins();
            Destroy(gameObject);
        }
        OnChangeHealth(creepHp);
    }

    void SetInitRef()
    {
        myTransform = transform;
        myNav = GetComponent<NavMeshAgent>();
        //checkRate = Random.Range(0.8f, 1.2f);
        checkRate = 0.5f;
        whiteBase = GameObject.Find("WhiteBase");
        blackBase = GameObject.Find("BlackBase");
    }
    void FindNewTarget()
    {

    }
    void GoThrone()
    {
        if (gameObject.tag == "White Creep")
        {
            playerTarget = blackBase;
            if (isServer)
                CmdScrPlayerSetDestination(blackBase.transform.position);
        }
        if (gameObject.tag == "Black Creep")
        {
            playerTarget = whiteBase;
            if (isServer)
                CmdScrPlayerSetDestination(whiteBase.transform.position);
        }
    }
    void DetectAttackTarget(string creepTag, LayerMask detectionLayer)
    {
        if(playerTarget == null) findTarget = false;
        if (Time.time > nextCheck && myNav.enabled == true && !findTarget)
        {
            nextCheck = Time.time + checkRate;
            hitColliders = Physics.OverlapSphere(myTransform.position, detectionRadius, detectionLayer);
            if (hitColliders.Length > 0)
            {
                float minDistance = 100f;
                GameObject target = null;
                foreach (Collider col in hitColliders)
                {
                    if (col.tag == creepTag)
                    {
                        //find the closest target
                        if (Vector3.Distance(transform.position, col.transform.position) < minDistance)
                        {
                            minDistance = Vector3.Distance(transform.position, col.transform.position);
                            target = col.gameObject;
                        }
                    }
                }
                playerTarget = target;
                if (isServer)
                    CmdScrPlayerSetDestination(target.transform.position);
                findTarget = true;
            }
            else
            {
                findTarget = false;
                GoThrone();
            }
        }
        if (!findTarget)
        {
            GoThrone();
        }

        // if (Time.time > nextCheck && myNav.enabled == true)
        // {
        //     nextCheck = Time.time + checkRate;
        //     hitColliders = Physics.OverlapSphere(myTransform.position, detectionRadius, detectionLayer);
        //     if (hitColliders.Length > 0)
        //     {
        //         foreach (Collider col in hitColliders)
        //         {
        //             if (GameObject.Find(col.name).tag.Contains("Player"))
        //             {
        //                 playerTarget = col.gameObject;
        //                 if (isServer)
        //                     CmdScrPlayerSetDestination(col.transform.position);
        //             }
        //         }
        //     }
        // }
    }

    public void OnAttackedByPlayer(Vector3 playerPos)
    {
        CmdScrPlayerSetDestination(playerPos);
    }

    [Command]
    public void CmdScrPlayerSetDestination(Vector3 argPosition)
    {//Step B, I do simple work, I not verifi a valid position in server, I only send to all clients
        RpcScrPlayerSetDestination(argPosition);
    }

    [ClientRpc]
    public void RpcScrPlayerSetDestination(Vector3 argPosition)
    {//Step C, only the clients move
        myNav.SetDestination(argPosition);
    }

    // Attack
    IEnumerator Attack()
    {
        for (; ; )
        {
            yield return new WaitForSeconds(0.2f);
            CheckIfInRange();
        }
    }

    void CheckIfInRange()
    {
        if (playerTarget != null)
        {
            currentDistance = Vector3.Distance(playerTarget.transform.position, transform.position);
            if (currentDistance < minDistance && Time.time > nextAttack)
            {
                nextAttack = Time.time + attackRate;
                if (playerTarget.GetComponent<PlayerHP>() != null) playerTarget.GetComponent<PlayerHP>().TakeDamage();
                if (playerTarget.GetComponent<MiniCreepController>() != null) playerTarget.GetComponent<MiniCreepController>().CreepTakeDamage();
                // StartCoroutine(ChangeCreepColor());
                // RpcChangeCreepColor();
            }
        }
    }

    // IEnumerator ChangeCreepColor()
    // {
    //     GetComponent<Renderer>().material = red;
    //     yield return new WaitForSeconds(1);
    //     GetComponent<Renderer>().material = white;

    // }

    // [ClientRpc]
    // void RpcChangeCreepColor()
    // {
    //     StartCoroutine(ChangeCreepColor());
    // }

    void OnChangeHealth(int health)
    {
        healthBar.sizeDelta = new Vector2(health * 17, healthBar.sizeDelta.y);
    }

    void SpawnCoins()
    {
        GameObject go = (GameObject)Instantiate(coin, transform.position, Quaternion.identity);
        NetworkServer.Spawn(go);
    }

    void CreepTakeDamage()
    {
        if (!isServer)
            return;
        // if (creepHp > 0)
        //     creepHp -= 1;
        RpcCreepTakeDamage();
    }

    [ClientRpc]
    public void RpcCreepTakeDamage()
    {
        if (creepHp > 0)
            creepHp -= 1;
    }

    // [ClientRpc]
    // public void RpcCreepTakeDamage()
    // {
    //     if (creepHp > 0)
    //         creepHp -= 1;
    // }

}