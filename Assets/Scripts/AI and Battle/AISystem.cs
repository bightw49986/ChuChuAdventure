using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AISystem
{
    public interface IAI_SimpleMove
    {
        float fCurrentSpeed { get; set; } 
        float fMaxSpeed { get; set; }
        Vector3 vVelocity { get; set; }
        Vector3 vMaxAcceleration { get; set; }
        Vector3 vMaxVelocity { get; set; }
        Vector3 vPositionNextFrame { get; set; }
        bool bAnimated { get; set; }
        bool bUseRootMotion { get; set; }
        void GetRootMotionSpeed();
    }

    public interface IAI_CollisionAvoid
    {
        Transform tNearestObstacle { set; get; }
        LayerMask CollisionLayer { set; get; }
        float fProbeLength { set; get; }
        float fDetectionRadius { set; get; }
        float fDotLastFrame { set; get; }

        bool DetectCollision();

        void AvoidCollision();
    }
}
