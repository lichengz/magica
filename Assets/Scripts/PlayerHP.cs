using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerHP : NetworkBehaviour
{
    bool isGameOver = false;
    [SyncVar(hook = "OnChangeHealth")] public int hp = 10;
    public GameObject body;
    Color tmpColor;
    // Health bar
    public RectTransform healthBar;

    [SyncVar]
    public bool isHPadd = false;

    // Use this for initialization
    void Start()
    {
        isGameOver = false;
        tmpColor = body.GetComponent<Renderer>().material.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (hp == 0 && !isGameOver)
        {
            GameObject.Find("GameMaster").GetComponent<GM_End>().StopPlayer();
            Time.timeScale = 0;
            isGameOver = true;
        }
    }
    public void TakeDamage()
    {
        if (!isServer)
            return;
        if (hp > 0)
            hp -= 1;
        CmdChangeColor();
    }

    public void AddHp()
    {
        if (!isServer)
            return;
        if (hp < 10)
            hp += 1;
        //isHPadd = false;
        //RpcReverseHPadd();
    }

    [Command]
    public void CmdChangeColor()
    {
        RpcChangeColorWhite();
        RpcTimeWait();
    }

    [ClientRpc]
    public void RpcChangeColorWhite()
    {
        body.GetComponent<Renderer>().material.color = new Color(3, 3, 3);
    }

    [ClientRpc]
    void RpcTimeWait()
    {
        Invoke("ChangeColorBack", 0.1f);
    }

    //[ClientRpc]
    public void ChangeColorBack()
    {
        body.GetComponent<Renderer>().material.color = tmpColor;
    }

    // Upadte Health bar
    void OnChangeHealth(int health)
    {
        if (isHPadd)
        {
            hp += 1;
            isHPadd = false;
            //CmdReverseHPadd();
        }
        else
        {
            hp -= 1;
        }
        healthBar.sizeDelta = new Vector2(health * 5, healthBar.sizeDelta.y);
    }

    // [Command]
    // void CmdReverseHPadd()
    // {
    //     isHPadd = false;
    // }
}
