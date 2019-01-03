using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    [CreateAssetMenu(menuName = "MyCharacter/Create Attribute")]
    public class Attributes : ScriptableObject
    {
        public string Description;
        public Sprite Icon;
    }
}

