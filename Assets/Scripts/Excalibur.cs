using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Excalibur : MonoBehaviour
{

    void OnCollisionEnter(Collision collision)
    {
        GameObject hit = collision.gameObject;
        if (hit.GetComponent<PlayerHP>() && hit.name != "YuriSword_Player" + "(Clone)")
        {
            //hit.GetComponent<PlayerHP>().isHPadd = false;
            hit.GetComponent<PlayerHP>().TakeDamage();
        }
        if (hit.GetComponent<MiniCreepController>())
        {
            hit.GetComponent<MiniCreepController>().creepHp -= 1;
            hit.GetComponent<MiniCreepController>().OnAttackedByPlayer(GameObject.Find("YuriSword_Player" + "(Clone)").transform.position);
        }
        if (hit.name != "YuriSword_Player" + "(Clone)")
            Destroy(gameObject);
    }
}
