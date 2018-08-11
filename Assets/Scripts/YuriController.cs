using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class YuriController : NetworkBehaviour
{
    Animator animator;
    float speed;
    float sideSpeed;
    CharacterController controller;
    [SyncVar] public bool movementDisable = false;
    public GameObject UIprefab;
    public GameObject BattleCameraPrefab;

    //Chat
    private ChatSystem chatSystem;
    private ChatIdentifier chatIdentifier;
    private TeamIdentifier teamIdentifier;

    [SerializeField]
    public uint teamIndex = 2;

    // Battle
    Transform magicSpawnPosition;
    public float magicPower = 30f;
    float fireRate = 1.2f;
    float nextFire;
    bool isLocked = false;
    public GameObject excalibur;
    public float K_CD = 2;
    float nextK;

    // K
    Collider[] hitColliders;
    public LayerMask hittableLayer;
    YuriPushBack yuriPushBackScript;

    public Sprite YuriSkill_Sprite_J;
    public Sprite YuriSkill_Sprite_K;

    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        magicSpawnPosition = GameObject.Find("ExcaliburSawn").GetComponent<Transform>();
        //if(GameObject.Find("NaokoStaff_Player")){
        //	Debug.Log("find naoko");
        yuriPushBackScript = GameObject.Find("BattleManager").GetComponent<YuriPushBack>();
        //}
        // Chat
        chatSystem = GameObject.FindObjectOfType<ChatSystem>();
        chatIdentifier = GameObject.FindObjectOfType<ChatIdentifier>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
            return;
        if (!movementDisable)
            MoveControl();
        //Battle
        if (!isLocked)
        {
            if (Input.GetButtonDown("Fire1") && Time.time > nextFire)
            {
                animator.Play("great_sword_high_spin_attack");
                CmdAttackJ();
                //Cool Down
                GameObject.Find("UI(Clone)").transform.Find("Canvas").transform.Find("Skill_J").GetComponent<SkillBar>().isCD = true;
            }
            if (Input.GetButtonDown("Fire2") && Time.time > nextK)
            {
                animator.Play("great_sword_kick");
                CmdPushK();
                //Cool Down
                GameObject.Find("UI(Clone)").transform.Find("Canvas").transform.Find("Skill_K").GetComponent<SkillBar>().isCD = true;
            }
        }
        // K
        //if(GameObject.Find("NaokoStaff_Player" + "(Clone)")){
        //	yuriPushBackScript = GameObject.Find("NaokoStaff_Player" + "(Clone)").GetComponent<YuriPushBack>();
        //}

        // Chat
        if (Input.GetKeyUp(KeyCode.Y) && (!ChatSystemIsOpen() || chatIdentifier.InputField.text == ""))
        {
            chatSystem.OpenChat(true, 0);
            //movementDisable = true;
        }
        else if (Input.GetKeyUp(KeyCode.U) && (!ChatSystemIsOpen() || chatIdentifier.InputField.text == ""))
        {
            chatSystem.OpenChat(true, teamIndex);
            //movementDisable = true;
        }
        else if (Input.GetKeyUp(KeyCode.Escape))
        {
            chatSystem.ForceCloseChat();
            movementDisable = false;
        }
    }

    void LateUpdate()
    {
        LockMovementDuringAttack();
    }

    void MoveControl()
    {
        speed = -Input.GetAxis("Vertical");
        sideSpeed = Input.GetAxis("Horizontal");
        if (speed != 0 || sideSpeed != 0)
        {
            animator.SetFloat("speed", 1);
            //CmdWalkAnimation();
        }
        else
        {
            animator.SetFloat("speed", 0);
            //CmdIdleAnimation();
        }
        if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D))
            transform.rotation = Quaternion.Euler(0, -45, 0);
        if (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A))
            transform.rotation = Quaternion.Euler(0, 45, 0);
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D))
            transform.rotation = Quaternion.Euler(0, -135, 0);
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A))
            transform.rotation = Quaternion.Euler(0, 135, 0);

        if (Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
            transform.rotation = Quaternion.Euler(0, 0, 0);
        if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
            transform.rotation = Quaternion.Euler(0, 180, 0);
        if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))
            transform.rotation = Quaternion.Euler(0, -90, 0);
        if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))
            transform.rotation = Quaternion.Euler(0, 90, 0);
        Vector3 moveDirection = new Vector3(-sideSpeed * 10, transform.position.y, speed * 10);
        controller.SimpleMove(moveDirection);
        //GetComponent<Rigidbody>().velocity = moveDirection;
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        GameObject UI = Instantiate(UIprefab, Vector3.zero, Quaternion.identity);
        UI.GetComponent<HPbar>().playerHPScript = GetComponent<PlayerHP>();
        UI.GetComponent<UpdateNetworth>().playerNetWorthScript = GetComponent<PlayerNetWorth>();

        GameObject camera = Instantiate(BattleCameraPrefab, BattleCameraPrefab.transform.position, BattleCameraPrefab.transform.rotation);
        camera.GetComponent<CameraFollow>().cameraTarget = transform;
        GameObject.Find("Camera").SetActive(false);
        // Sync animation
        //GetComponent<NetworkAnimator>().SetParameterAutoSend(0, true);
        //Cool Down
        GameObject.Find("UI(Clone)").transform.Find("Canvas").transform.Find("Skill_J").GetComponent<SkillBar>().coolDown = fireRate;
        GameObject.Find("UI(Clone)").transform.Find("Canvas").transform.Find("Skill_K").GetComponent<SkillBar>().coolDown = K_CD;
        GameObject.Find("UI(Clone)").transform.Find("Canvas").transform.Find("Skill_J").GetComponent<Image>().sprite = YuriSkill_Sprite_J;
        GameObject.Find("UI(Clone)").transform.Find("Canvas").transform.Find("Skill_K").GetComponent<Image>().sprite = YuriSkill_Sprite_K;
    }

    public override void PreStartClient()
    {
        GetComponent<NetworkAnimator>().SetParameterAutoSend(0, true);
    }

    //[Command]
    public void CmdIdleAnimation()
    {
        animator.SetFloat("speed", 0);
    }
    //[Command]
    public void CmdWalkAnimation()
    {
        animator.SetFloat("speed", 1);
    }

    // Battle
    [Command]
    public void CmdAttackJ()
    {
        //if(Input.GetButtonDown("Fire1") && Time.time > nextFire)
        //{
        isLocked = true;
        movementDisable = true;

        GameObject go = (GameObject)Instantiate(excalibur, magicSpawnPosition.position, magicSpawnPosition.rotation);
        go.GetComponent<Rigidbody>().AddForce(magicSpawnPosition.forward * magicPower, ForceMode.Impulse);
        //Spawn magic on server
        ClientScene.RegisterPrefab(go);
        NetworkServer.Spawn(go);
        Destroy(go, 2f);
        RpcUpdateTime();
        //}
    }

    [ClientRpc]
    public void RpcUpdateTime()
    {
        nextFire = Time.time + fireRate;
    }

    [Command]
    public void CmdPushK()
    {
        //if(Input.GetButtonDown("Fire2") && Time.time > nextK)
        //{
        isLocked = true;
        movementDisable = true;
        nextK = Time.time + K_CD;
        // push back enemeis
        hitColliders = Physics.OverlapSphere(transform.position, 5, hittableLayer);
        foreach (Collider col in hitColliders)
        {
            if (col.GetComponent<Rigidbody>() != null && col.name != "YuriSword_Player")
            {
                Vector3 pushDir = col.transform.position - transform.position;
                if (yuriPushBackScript)
                    yuriPushBackScript.AddImpact(pushDir);
                if (col.CompareTag("Enemy"))
                {
                    //push back enemies
                }
            }
        }
        //}
    }

    void LockMovementDuringAttack()
    {
        if ((animator.GetCurrentAnimatorStateInfo(0).IsName("great_sword_high_spin_attack")
        || animator.GetCurrentAnimatorStateInfo(0).IsName("great_sword_kick"))
        && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            movementDisable = false;
            isLocked = false;
        }
    }



    // Chat helper
    private bool ChatSystemIsOpen()
    {
        return chatSystem.GetComponent<CanvasGroup>().alpha > 0.01f;
    }
}