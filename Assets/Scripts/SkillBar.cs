using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillBar : MonoBehaviour
{
    Image imageCD;
    public float coolDown;
    public bool isCD;
    bool flag = true;

    // Use this for initialization
    void Start()
    {
        imageCD = transform.Find("CD").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isCD)
        {
            if (flag)
            {
                imageCD.fillAmount = 1;
				flag = false;
            }
            imageCD.fillAmount -= 1 / coolDown * Time.deltaTime;
            if (imageCD.fillAmount <= 0)
            {
                imageCD.fillAmount = 0;
                isCD = false;
				flag = true;
            }
        }
    }
}
