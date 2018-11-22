using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BattleSystem;

namespace BattleSystem
{
    public enum EDamagePart { Middle = 0 , Left = 1 , Right = 2 }

    public class DefendBox_Player : DefendBox
    {
        public EDamagePart DamagePart = EDamagePart.Middle;
    }

}
