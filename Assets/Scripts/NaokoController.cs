using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NaokoController : NetworkBehaviour
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
    public uint teamIndex = 1;

    // Battle
    public GameObject magicBall;
    Transform magicSpawnPosition;
    public float magicPower = 30f;
    float fireRate = 0.5f;
    float nextFire;
    bool isLocked = false;
    bool willBlink = false;
    public float K_CD = 5;
    float nextK;
    public Sprite NaokoSkill_Sprite_J;
    public Sprite NaokoSkill_Sprite_K;

    // Use this for initialization
    void Start()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        // Battle
        magicSpawnPosition = GameObject.Find("MagicSpawn").GetComponent<Transform>();
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
        // Battle
        if (!isLocked)
        {
            if (Input.GetButtonDown("Fire1") && Time.time > nextFire)
            {
                animator.Play("Standing_1H_Magic_Attack_01");
                CmdAttackJ();
                //Cool Down
                GameObject.Find("UI(Clone)").transform.Find("Canvas").transform.Find("Skill_J").GetComponent<SkillBar>().isCD = true;
            }
            if (Input.GetButtonDown("Fire2") && Time.time > nextK)
            {
                animator.Play("standing_1H_cast_spell_01");
                CmdBlinkK();
                //Cool Down
                GameObject.Find("UI(Clone)").transform.Find("Canvas").transform.Find("Skill_K").GetComponent<SkillBar>().isCD = true;
            }
        }

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
        if (willBlink && animator.GetCurrentAnimatorStateInfo(0).IsName("standing_1H_cast_spell_01") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.3f)
        {
            RpcBlink();
        }
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
        GameObject.Find("UI(Clone)").transform.Find("Canvas").transform.Find("Skill_J").GetComponent<Image>().sprite = NaokoSkill_Sprite_J;
        GameObject.Find("UI(Clone)").transform.Find("Canvas").transform.Find("Skill_K").GetComponent<Image>().sprite = NaokoSkill_Sprite_K;
    }

    public override void PreStartClient()
    {
        GetComponent<NetworkAnimator>().SetParameterAutoSend(0, true);
    }

    [Command]
    public void CmdIdleAnimation()
    {
        //animator.SetFloat("speed", 0);
        animator.Play("Standing_1H_Magic_Attack_01");
    }
    [Command]
    public void CmdWalkAnimation()
    {
        //animator.SetFloat("speed", 1);
        animator.Play("standing_1H_cast_spell_01");
    }

    // Battle
    [Command]
    public void CmdAttackJ()
    {
        isLocked = true;
        movementDisable = true;

        GameObject go = (GameObject)Instantiate(magicBall, magicSpawnPosition.position, magicSpawnPosition.rotation);
        go.GetComponent<Rigidbody>().AddForce(magicSpawnPosition.forward * magicPower, ForceMode.Impulse);
        //Spawn magic on server
        ClientScene.RegisterPrefab(go);
        NetworkServer.Spawn(go);
        Destroy(go, 2f);
        RpcUpdateTimeJ();
    }

    [ClientRpc]
    public void RpcUpdateTimeJ()
    {
        nextFire = Time.time + fireRate;
    }

    [Command]
    public void CmdBlinkK()
    {
        if (Input.GetButtonDown("Fire2") && Time.time > nextK)
            //{
            isLocked = true;
        movementDisable = true;
        willBlink = true;
        RpcUpdateTimeK();
        //}
    }

    [ClientRpc]
    public void RpcBlink()
    {
        transform.position += transform.forward * 10;
        willBlink = false;
    }

    [ClientRpc]
    public void RpcUpdateTimeK()
    {
        nextK = Time.time + K_CD;
    }

    void LockMovementDuringAttack()
    {
        if ((animator.GetCurrentAnimatorStateInfo(0).IsName("Standing_1H_Magic_Attack_01")
        || animator.GetCurrentAnimatorStateInfo(0).IsName("standing_1H_cast_spell_01"))
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