using UnityEngine;
using System.Collections.Generic;

//this class is for demo purposes.
public class TeamSwapper : MonoBehaviour
{
    public void IncrementTeamForLocalPlayer()
    {
        PlayerController localPlayer = new List<PlayerController>(GameObject.FindObjectsOfType<PlayerController>()).Find(player => player.isLocalPlayer);
        if (localPlayer.teamIndex >= 3)
        {
            localPlayer.teamIndex = 1;
        }
        else
        {
            localPlayer.teamIndex++;
        }
    }
}
