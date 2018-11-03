using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AISystem
{
    public interface IAI_SimpleMove //基本移動
    {

        //屬性
        float Speed { get;}                  //速率
        float MaxSpeed { get; set; }         //最大速率
        float AngularSpeed { get;}           //角速率
        float MaxAngularSpeed { get; set; }  //最大角速率

        Vector3 Velocity { get;}                 //速度
        Vector3 MaxVelocity { get; set; }        //最大速度
        Vector3 DeltaVelocity { get;}            //與上一幀的速度差
        Vector3 MaxDeltaVelocity { get; set; }   //允許最大的速度差

        Vector3 PositionLastFrame { get; set; }  //上一幀位置
        Vector3 Position { get; set; }           //這一幀位置
        Vector3 PositionNextFrame { get; set; }  //下一幀位置

        Vector3 EulerAnglesLastFrame { get; set; } //上一幀尤拉角
        Vector3 EulerAngles { get; set; }        //尤拉角

        bool UseRootMotion { get;}               //是否使用美術位移


        //方法
        void UpdateTransformInfo();                   //更新Transform資訊
        void MoveToNextPosition();               //移動到下個位置
    }

    public interface IAI_CollisionAvoid //碰撞迴避
    {
        //屬性
        Transform NearestObstacle { get; set; }    //最靠近的障礙物
        LayerMask CollisionLayer { get; set; }     //障礙物的Layer
        float ProbeLength { get; set; }            //探針長度
        float CollisionDetectRadius { get; set; }  //障礙物迴避的偵測範圍
        float DotLastFrame { get; set; }           //行進方向內積

        //方法
        bool DetectCollision();                    //障礙物偵測
        void AvoidCollision();                     //障礙物迴避
    }

    public interface IAI_MonsterBehavior //怪物行為
    {
        //屬性
        string TargetState { get; set; }      //目標狀態(玩家的)
        Transform Target { get; set; }        //目標位置
        Vector3 TargetDirection { get; set; } //目標方向
        float TargetDistance { get; set; }    //目標距離
        float TargetWaitTime { get; set; }    //等待目標的時間

        float IdleTime { get; set; }          //閒晃時間
        float ChaseRadius { get; set; }       //追逐範圍
        float ConRadius { get; set; }         //對峙範圍
        float AttRadius { get; set; }         //攻擊範圍

        // ... 
    }

    public interface IAI_GroupBehavior //群體行為
    {
        
    }
}
