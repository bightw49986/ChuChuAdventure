using UnityEngine;


namespace AISystem
{
    public partial class AIData : MonoBehaviour
    {
        public bool PlayerShowedUp()
        {
            return AIMethod.CheckPointInFan(transform, m_vPlayerPos, fFaceCautionRange, FOV) || PlayerInCautionRange();
        }

        public bool PlayerInCautionRange()
        {
            return fSqrPlayerDis <= fSqrBackCautionRange;
        }

        public bool PlayerInChaseRange()
        {
            return fSqrPlayerDis <= fSqrChaseRange;
        }

        public bool PlayerInJumpAtkRange()
        {
            return fSqrPlayerDis <= fSqrJumpAtkRange;
        }

        public bool PlayerInAtkRange()
        {
            return fSqrPlayerDis <= fSqrAtkRange;
        }

        public bool PlayerStillInAtkRange()
        {
            return AIMethod.CheckPointInFan(transform, m_vPlayerPos, fAtkRange + fAtkOffset, 180f);
        }

        public bool PlayerOnRightSide()
        {
            return AIMethod.FindAngle(transform.position, m_vPlayerPos, transform.up) > 0;
        }
    }
}