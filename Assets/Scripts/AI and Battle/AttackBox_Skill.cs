using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleSystem
{
    public class AttackBox_Skill : AttackBox
    {
        void OnParticleCollision(GameObject other)
        {
            PassDamage(other,Host.AttackerType);
        }

        protected override void StartDetection()
        {
            HitBoxes = new List<DefendBox>();
        }

        protected override void StopDetection()
        {
            if (HitBoxes!=null)
            HitBoxes.Clear();
        }
    }
}

