using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class RD_Hp : NetworkBehaviour
{
    Color tmpColor;
    public GameObject body;
    bool dying = false;
    float tmp = 0f;
    // Use this for initialization
    void Start()
    {
        tmpColor = body.GetComponent<Renderer>().material.color;
        Invoke("SelfDestroy", 10);
        Invoke("StartDying", 7);
    }

    // Update is called once per frame
    void Update()
    {
        if (dying)
        {
            Invoke("BlinkColor", tmp);
            Invoke("OriginalColor", tmp + 0.2f);
            tmp += 0.4f;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerHP>())
        {
            //other.GetComponent<PlayerHP>().isHPadd = true;
            if (other.GetComponent<PlayerHP>().hp < 10)
            {
                if (isServer)
                {
                    RpcReverse(other.name);
                }
                other.GetComponent<PlayerHP>().isHPadd = true;
                other.GetComponent<PlayerHP>().AddHp();
            }
            SelfDestroy();
        }
    }

    void SelfDestroy()
    {
        Destroy(gameObject);
    }

    void BlinkColor()
    {
        body.GetComponent<Renderer>().material.color = new Color(3, 3, 3);
    }

    void OriginalColor()
    {
        body.GetComponent<Renderer>().material.color = tmpColor;
    }

    void StartDying()
    {
        dying = true;
    }

    [ClientRpc]
    void RpcReverse(string name)
    {
        GameObject.Find(name).GetComponent<PlayerHP>().isHPadd = true;
    }
}
