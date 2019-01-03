using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    [CreateAssetMenu(menuName = "MyCharacter/Create Skill")]
    public class Skills : ScriptableObject
    {
        public string Description;
        public Sprite Icon;
        public int levelNeeded;
        public int SkillPointNeeded;
        public List<PlayerAttributes> AffectedAttributes = new List<PlayerAttributes>();
    }
}