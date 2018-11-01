using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public sealed class BossFSMGenerater : NPCFSMGenerater
{
    Random rnd = new Random();
    public sealed override bool AnyState()
    {
        if (BossStats.fHP <= 0f)
        {
            sNextState = "Death";
        }
        else if (BossStats.fToughness <5f)
        {
            sNextState = "GetHit1";
            sNextState = "GetHit2";
        }
        return false;
    }

    public sealed override void AddState()
    {
        SubscribeStateLibrary.Add("Idle", new BossIdle());
        SubscribeStateLibrary.Add("Walk", new BossWalk());
        SubscribeStateLibrary.Add("Jump", new BossJump());
        SubscribeStateLibrary.Add("Attack1", new BossAttack1());
        SubscribeStateLibrary.Add("Attack2", new BossAttack2());
        SubscribeStateLibrary.Add("Attack3", new BossAttack3());
        SubscribeStateLibrary.Add("Transform", new BossTransform());
        SubscribeStateLibrary.Add("GetHit1", new BossGetHit1());
        SubscribeStateLibrary.Add("GetHit2", new BossGetHit2());
        SubscribeStateLibrary.Add("Death", new BossDeath());
        SubscribeStateLibrary.Add("Watch", new BossWatch());
    }
}
