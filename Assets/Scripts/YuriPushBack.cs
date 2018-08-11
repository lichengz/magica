using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class YuriPushBack : NetworkBehaviour {

	   //float mass = 3.0F; // defines the character mass
     [SyncVar] Vector3 impact = Vector3.zero;
     private CharacterController character;
     // Use this for initialization
     void Start () {
          //character = GetComponent<CharacterController>();
     }
     
     // Update is called once per frame
     void Update () {
       if(GameObject.Find("NaokoStaff_Player" + "(Clone)"))
          character = GameObject.Find("NaokoStaff_Player" + "(Clone)").GetComponent<CharacterController>();
      // apply the impact force:
       CmdPush();
     }
     // call this function to add an impact force:
     public void AddImpact(Vector3 dir){
       dir.Normalize();
       if (dir.y < 0) dir.y = -dir.y; // reflect down force on the ground
       CmdUpdateImpact(dir);
     }

     //[Command]
     public void CmdUpdateImpact(Vector3 dir){
        impact += dir.normalized * 200 / 3f;
     }
     
     //[Command]
     public void CmdPush(){
        if (impact.magnitude > 0.2F) 
        {
          RpcMove();
        }
           // consumes the impact energy each cycle:
        impact = Vector3.Lerp(impact, Vector3.zero, 5*Time.deltaTime);
     }

     //[ClientRpc]
     public void RpcMove(){
        character.Move(impact * Time.deltaTime);
     }
}
