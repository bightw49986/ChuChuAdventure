using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public sealed class BossFSMGenerater : NPCFSMGenerater
{
    BossStats bossStats;
    public override void InitState(StateSystem state)
    {
        base.InitState(state);
        state.bossStats = bossStats;
    }

    public override void Awake()
    {
        base.Awake();
        bossStats = GetComponentInParent<BossStats>();
    }
    public sealed override bool AnyState()
    {
        if (bossStats.fHP <= 0f)
        {
            SNextState = "Death";
        }
        else if (bossStats.fToughness < 5f)
        {
            SNextState = "GetHit1";
            SNextState = "GetHit2";
        }
        return false;
    }

    public sealed override void AddState()
    {
        SubscribeStateLibrary.Add("Idle", new BossIdle());
        InitState(SubscribeStateLibrary["Idle"]);
        SubscribeStateLibrary.Add("Walk", new BossWalk());
        InitState(SubscribeStateLibrary["Walk"]);
        SubscribeStateLibrary.Add("Jump", new BossJump());
        InitState(SubscribeStateLibrary["Jump"]);
        SubscribeStateLibrary.Add("Attack1", new BossAttack1());
        InitState(SubscribeStateLibrary["Attack1"]);
        SubscribeStateLibrary.Add("Attack2", new BossAttack2());
        InitState(SubscribeStateLibrary["Attack2"]);
        SubscribeStateLibrary.Add("Attack3", new BossAttack3());
        InitState(SubscribeStateLibrary["Attack3"]);
        SubscribeStateLibrary.Add("Transform", new BossTransform());
        InitState(SubscribeStateLibrary["Transform"]);
        SubscribeStateLibrary.Add("GetHit1", new BossGetHit1());
        InitState(SubscribeStateLibrary["GetHit1"]);
        SubscribeStateLibrary.Add("GetHit2", new BossGetHit2());
        InitState(SubscribeStateLibrary["GetHit2"]);
        SubscribeStateLibrary.Add("Death", new BossDeath());
        InitState(SubscribeStateLibrary["Death"]);
        SubscribeStateLibrary.Add("Watch", new BossWatch());
        InitState(SubscribeStateLibrary["Watch"]);
    }
}
