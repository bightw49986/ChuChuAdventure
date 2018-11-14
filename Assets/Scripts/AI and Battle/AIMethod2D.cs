using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AISystem
{
    public static class AIMethod2D
    {
        public static bool CheckinSightFan(Transform origin,Vector3 vTargetPos, float fRange, float fAngles)
        {
            Vector3 vFaceDir = Vector3.ProjectOnPlane(origin.forward, Vector3.up);
            Vector3 vTaDir = Vector3.ProjectOnPlane(vTargetPos - origin.position, Vector3.up);
            float fTaDis = Vector3.SqrMagnitude(vTaDir);
            if (fTaDis > fRange * fRange) return false;
            Vector3 vLeft = Quaternion.AngleAxis(-fAngles * 0.5f, Vector3.up) * vFaceDir;
            Vector3 vRight = Quaternion.AngleAxis(fAngles * 0.5f, Vector3.up) * vFaceDir;
            if (Vector3.Dot(Vector3.Cross(vTaDir, vRight).normalized, Vector3.Cross(vTaDir, vLeft).normalized) != -1) return false;
            if (Vector3.Dot(vTaDir, vFaceDir) < 0) return false;
            return true;
        }

        public static Vector3 SeekTarget(Vector3 originPos, Vector3 target, Vector3 velocity, float fMaxSpeed)
        {
            Vector3 steering = Vector3.zero;
            Vector3 oriPos = originPos;
            oriPos.y = 0;
            Vector3 tarPos = target;
            tarPos.y = 0;
            Vector3 tarDir = tarPos - oriPos;
            if (Vector3.SqrMagnitude(tarDir)< velocity.sqrMagnitude)
            {
                return Vector3.zero;
            }
            Vector3 desireVel = tarDir.normalized * fMaxSpeed;
            steering = desireVel - velocity;
            steering = Vector3.Min(steering, steering.normalized * fMaxSpeed);
            return steering;
        }

        public static Vector3 SeekTarget(Vector3 origin, Vector3 target, float fMass, Vector3 velocity, float fMaxSpeed)
        {
            return SeekTarget(origin, target, velocity, fMaxSpeed) / fMass;
        }

        public static Vector3 SeekTarget(Vector3 origin, Vector3 target, Vector3 velocity, float fMaxSpeed, float fArriveRange)
        {
            Vector3 steering = Vector3.zero;
            Vector3 oriPos = origin;
            oriPos.y = 0;
            Vector3 tarPos = target;
            tarPos.y = 0;
            Vector3 tarDir = tarPos - oriPos;
            float fDist = Vector3.Magnitude(tarDir);
            if (fDist < velocity.magnitude)
            {
                return Vector3.zero;
            }
            Vector3 desireVel;
            if(fDist<= fArriveRange)
            {
                float fRampedSpeed = Mathf.Min(fMaxSpeed * (fDist / fArriveRange), fMaxSpeed);
                desireVel = fRampedSpeed / fDist * tarDir;
            }
            else
            {
                desireVel = Vector3.Normalize(tarDir) * fMaxSpeed;
            }
            steering = desireVel - velocity;
            steering = Vector3.Min(steering, steering.normalized * fMaxSpeed);
            return steering;
        }

        public static Vector3 SeekTarget(Vector3 origin, Vector3 target, float fMass, Vector3 velocity, float fMaxSpeed, float fArriveRange)
        {
            return SeekTarget(origin, target, velocity, fMaxSpeed, fArriveRange)  / fMass ;
        }

        public static Vector3 FleeTarget(Vector3 origin, Vector3 target, Vector3 velocity, float fMaxSpeed)
        {
            Vector3 steering = Vector3.zero;
            Vector3 oriPos = origin;
            oriPos.y = 0;
            Vector3 tarPos = target;
            tarPos.y = 0;
            Vector3 tarDir = -(tarPos - oriPos);
            if (Vector3.SqrMagnitude(tarDir) < velocity.sqrMagnitude)
            {
                return Vector3.zero;
            }
            Vector3 desireVel = tarDir.normalized * fMaxSpeed;
            steering = desireVel - velocity;
            steering = Vector3.Min(steering, steering.normalized * fMaxSpeed);
            return steering;
        }
    }
}


