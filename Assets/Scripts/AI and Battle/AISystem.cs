using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AISystem
{
    public interface IAI_SimpleMove
    {
        float Speed { get; set; } 
        float MaxSpeed { get; set; }
        float AngularSpeed { get; set; }
        float MaxAngularSpeed { get; set; }

        Vector3 Velocity { get; set; }
        Vector3 MaxVelocity { get; set; }
        Vector3 DeltaVelocity { get; set; }
        Vector3 MaxDeltaVelocity { get; set; }
        Vector3 PositionNextFrame { get; set; }

        bool UseRootMotion { get; set; }
        void GetRootMotionSpeed();
    }

    public interface IAI_CollisionAvoid
    {
        Transform NearestObstacle { get; set; }
        LayerMask CollisionLayer { get; set; }
        float ProbeLength { get; set; }
        float CollisionDetectRadius { get; set; }
        float DotLastFrame { get; set; }

        bool DetectCollision();
        void AvoidCollision();
    }

    public interface IAI_MonsterBehavior
    {
        Transform Target { get; set; }
        Vector3 TargetDirection { get; set; }
        float TargetDistance { get; set; }

        float IdleSpeed { get; set; }
        float ChaseRadius { get; set; }
        // ... 
    }
}
