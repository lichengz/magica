using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RD_Gold : MonoBehaviour
{
    Color tmpColor;
    public GameObject body;
    bool dying = false;
    float tmp = 0f;
    // Use this for initialization
    void Start()
    {
        tmpColor = body.GetComponent<Renderer>().material.color;
        Invoke("SelfDestroy", 5);
        Invoke("StartDying", 3);
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
        if (other.GetComponent<PlayerNetWorth>())
        {
            other.GetComponent<PlayerNetWorth>().netWorh += 1;
			Destroy(gameObject);
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
}
