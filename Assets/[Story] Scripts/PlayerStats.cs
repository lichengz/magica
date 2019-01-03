using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class PlayerStats : MonoBehaviour
    {
        [Header("Player Stats")]
        public int HP;
        [Header("Player Attributes")]
        public List<PlayerAttributes> attributes = new List<PlayerAttributes>();
        [Header("Player Attributes")]
        public List<Skills> skills = new List<Skills>();
    }
}