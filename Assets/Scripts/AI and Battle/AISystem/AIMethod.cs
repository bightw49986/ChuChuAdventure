using UnityEngine;

namespace AISystem
{
    public static class AIMethod
    {
        public static bool CheckPointInFan(Transform origin,Vector3 vTargetPos, float fRange, float fAngles)
        {
            Vector3 vFaceDir = origin.forward;
            Vector3 vTaDir = vTargetPos - origin.position;
            float fTaDis = Vector3.SqrMagnitude(vTaDir);
            if (fTaDis > fRange * fRange) return false;
            vFaceDir.y = 0;
            vTaDir.y = 0;
            vFaceDir.Normalize();
            vTaDir.Normalize();
            Vector3 vLeft = Quaternion.AngleAxis(-fAngles * 0.5f, Vector3.up) * vFaceDir;
            Vector3 vRight = Quaternion.AngleAxis(fAngles * 0.5f, Vector3.up) * vFaceDir;
            return Vector3.Dot(Vector3.Cross(vTaDir, vRight).normalized, Vector3.Cross(vTaDir, vLeft).normalized) != -1 || Vector3.Dot(vTaDir, vFaceDir) >= 0;
        }

        public static float FindAngle(Vector3 vFrom , Vector3 vTo, Vector3 vUp)
        {
            if (vTo == Vector3.zero) return 0f;
            float fAngle = Vector3.Angle(vFrom, vTo);
            Vector3 vNormal = Vector3.Cross(vFrom, vTo);
            fAngle *= Mathf.Sign(Vector3.Dot(vNormal, vUp));
            return fAngle;
        }

        public static Vector3 Seek(Vector3 originPos, Vector3 target, Vector3 velocity, float fMaxSpeed)
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

        public static Vector3 Seek(Vector3 origin, Vector3 target, float fMass, Vector3 velocity, float fMaxSpeed)
        {
            return Seek(origin, target, velocity, fMaxSpeed) / fMass;
        }

        public static Vector3 Seek(Vector3 origin, Vector3 target, Vector3 velocity, float fMaxSpeed, float fArriveRange)
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

        public static Vector3 Seek(Vector3 origin, Vector3 target, float fMass, Vector3 velocity, float fMaxSpeed, float fArriveRange)
        {
            return Seek(origin, target, velocity, fMaxSpeed, fArriveRange)  / fMass ;
        }

        public static Vector3 Flee(Vector3 origin, Vector3 target, Vector3 velocity, float fMaxSpeed)
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

        public static bool Random(int percentage)
        {
            System.Random random = new System.Random();
            int pick = random.Next(0, 100);
            return percentage < pick;
        }
    }
}


