using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class MyCharacter : MonoBehaviour
    {
        CharacterController controller;
        float speed;
        float sideSpeed;
        // Start is called before the first frame update
        void Start()
        {
            controller = GetComponent<CharacterController>();
        }

        // Update is called once per frame
        void Update()
        {
            MoveControl();
        }

        void MoveControl()
        {
            speed = -Input.GetAxis("Vertical");
            sideSpeed = Input.GetAxis("Horizontal");
            if (speed != 0 || sideSpeed != 0)
            {
                //animator.SetFloat("speed", 1);
                //CmdWalkAnimation();
            }
            else
            {
                //animator.SetFloat("speed", 0);
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
    }
}