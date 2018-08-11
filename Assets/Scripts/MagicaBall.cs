using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicaBall : MonoBehaviour
{

    void OnCollisionEnter(Collision collision)
    {
        GameObject hit = collision.gameObject;
        if (hit.GetComponent<PlayerHP>() && hit.name != "NaokoStaff_Player" + "(Clone)")
        {
            //hit.GetComponent<PlayerHP>().isHPadd = false;
            hit.GetComponent<PlayerHP>().TakeDamage();
        }
        if (hit.GetComponent<MiniCreepController>())
        {
            hit.GetComponent<MiniCreepController>().creepHp -= 1;
            hit.GetComponent<MiniCreepController>().OnAttackedByPlayer(GameObject.Find("NaokoStaff_Player" + "(Clone)").transform.position);
        }
        if (hit.name != "NaokoStaff_Playerr" + "(Clone)")
            Destroy(gameObject);
    }
}
