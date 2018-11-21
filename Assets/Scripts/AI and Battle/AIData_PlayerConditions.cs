using UnityEngine;


namespace AISystem
{
    public partial class AIData : MonoBehaviour
    {
        public bool PlayerShowedUp
        {
            get { return AIMethod.CheckPointInFan(transform, m_vPlayerPos, fFaceCautionRange, FOV) || PlayerInCautionRange; }
        }

        public bool PlayerInBattleRange
        {
            get { return fSqrPlayerDis <= fSqrFaceCautionRange; }
        }

        public bool PlayerInCautionRange
        {
            get { return fSqrPlayerDis <= fSqrBackCautionRange; }
        }

        public bool PlayerInChaseRange
        {
            get { return fSqrPlayerDis <= fSqrChaseRange; }
        }

        public bool PlayerInJumpAtkRange
        {
            get { return fSqrPlayerDis <= fSqrJumpAtkRange; }
        }

        public bool PlayerInAtkRange
        {
            get { return fSqrPlayerDis <= fSqrAtkRange; }
        }

        public bool PlayerStillInAtkRange
        {
            get { return AIMethod.CheckPointInFan(transform, m_vPlayerPos, fAtkRange + fAtkOffset, 180f); }
        }

        public bool PlayerOnRightSide
        {
            get { return AIMethod.FindAngle(transform.position, m_vPlayerPos, transform.up) > 0; }
        }
    }
}