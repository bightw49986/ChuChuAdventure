using ResourcesManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public sealed class IceFSMGenerater : NPCFSMGenerater
{
    System.Random random = new System.Random();
    BossStats bossStats;
    public bool bJumpSwitch = false;
    Dictionary<string, Transform> FxPoint = new Dictionary<string, Transform>();//先是取得我要的名稱的game objects，除了可指定特效初始位置，也可在狀態機的Do改變transform做出射出技能的效果
    [HideInInspector]public GameObject FXNumberOne , FXNumberTwo;//物件池access出來的物件容器s


    public override void InitState(StateSystem state)
    {
        base.InitState(state);
        state.bossStats = bossStats;
    }

    void AddParticleAttackBoxes()
    {
        bossStats.AddParticleAttackBoxes(BossFX_Ice.Ice1Fan);
        bossStats.AddParticleAttackBoxes(BossFX_Ice.Ice1OnHand);
        bossStats.AddParticleAttackBoxes(BossFX_Ice.Ice2IceLance);
        bossStats.AddParticleAttackBoxes(BossFX_Ice.Ice3FistsExplosion);
        bossStats.AddParticleAttackBoxes(BossFX_Ice.IceJumpDown);
        bossStats.AddParticleAttackBoxes(BossFX_Ice.IceJumpUp);
        bossStats.AddParticleAttackBoxes(BossFX_Ice.IceTransformOver);
        bossStats.AddParticleAttackBoxes(BossFX_Ice.IceTransformStart);
    }

    void AddFxChildren()//在Npc的Prefab下放的空物件，value那個空物件不動，將由各狀態Do()中，每frame改變FXNumberOne , FXNumberTwo的transform做成投射技能(位置與方向)，而技能的擊中特效由OnCollision產生(?)
    {
        FxPoint.Add("FxGroundCenter", transform.FindDeepChild("FxGroundCenter"));
        FxPoint.Add("FxLeftHand", transform.FindDeepChild("FxLeftHand"));
        FxPoint.Add("FxRightHand", transform.FindDeepChild("FxRightHand"));
    }

    void CastSkillBox(int order)//動畫事件傳入編號int觸發
    {
        switch (SCurrentState)
        {
            case "Transform":
                if (order == 1) FXNumberOne = ObjectPool.AccessGameObjectFromPool(PoolKey.BossFX_Ice, BossFX_Ice.IceTransformStart, FxPoint["FxGroundCenter"], FxPoint["FxGroundCenter"].position);
                else if (order == 2) FXNumberTwo = ObjectPool.AccessGameObjectFromPool(PoolKey.BossFX_Ice, BossFX_Ice.IceTransformOver, FxPoint["FxGroundCenter"], FxPoint["FxGroundCenter"].position);
                break;
            case "Jump":
                if (order == 1)
                {
                    bJumpSwitch = true;
                    FXNumberOne = ObjectPool.AccessGameObjectFromPool(PoolKey.BossFX_Ice, BossFX_Ice.IceJumpUp, FxPoint["FxGroundCenter"], FxPoint["FxGroundCenter"].position);
                }
                else if (order == 2)
                {
                    bJumpSwitch = false;
                    FXNumberTwo = ObjectPool.AccessGameObjectFromPool(PoolKey.BossFX_Ice, BossFX_Ice.IceJumpDown, FxPoint["FxGroundCenter"], FxPoint["FxGroundCenter"].position);
                }
                break;
            case "Attack1":
                if (order == 1) FXNumberOne = ObjectPool.AccessGameObjectFromPool(PoolKey.BossFX_Ice, BossFX_Ice.Ice1OnHand, FxPoint["FxLeftHand"], FxPoint["FxLeftHand"].position);
                else if (order == 2) FXNumberTwo = ObjectPool.AccessGameObjectFromPool(PoolKey.BossFX_Ice, BossFX_Ice.Ice1Fan, FxPoint["FxGroundCenter"], FxPoint["FxGroundCenter"].position);
                break;
            case "Attack2":
                FXNumberOne = ObjectPool.AccessGameObjectFromPool(PoolKey.BossFX_Ice, BossFX_Ice.Ice2IceLance, FxPoint["FxLeftHand"], FxPoint["FxLeftHand"].position);
                FXNumberTwo = ObjectPool.AccessGameObjectFromPool(PoolKey.BossFX_Ice, BossFX_Ice.Ice2IceLance, FxPoint["FxRightHand"], FxPoint["FxRightHand"].position);
                break;
            case "Attack3"://Ice3FistsExplosion為擊中特效
                FXNumberOne = ObjectPool.AccessGameObjectFromPool(PoolKey.BossFX_Ice, BossFX_Ice.Ice1OnHand, FxPoint["FxLeftHand"], FxPoint["FxLeftHand"].position);
                FXNumberTwo = ObjectPool.AccessGameObjectFromPool(PoolKey.BossFX_Ice, BossFX_Ice.Ice1OnHand, FxPoint["FxRightHand"], FxPoint["FxRightHand"].position);
                break;
            default:
                FXNumberOne = FXNumberTwo = null;
                print("找不到你現在的狀態R~");
                break;
        }
        if (FXNumberOne != null)
        {
            BattleSystem.AttackBox_Skill skillBoxOne = FXNumberOne.GetComponentInChildren<BattleSystem.AttackBox_Skill>();
            skillBoxOne.InitAttackBox(bossStats);
        }
        if (FXNumberTwo != null)
        {
            BattleSystem.AttackBox_Skill skillBoxTwo = FXNumberTwo.GetComponentInChildren<BattleSystem.AttackBox_Skill>();
            skillBoxTwo.InitAttackBox(bossStats);
        }

    }

    void AimAttack2()
    {
        FXNumberOne.transform.rotation = Quaternion.LookRotation(bossStats.m_vDistanceToPlayer);
        FXNumberOne.transform.Translate(bossStats.m_vDistanceToPlayer.normalized * Time.deltaTime);//Coroutine?
    }


    void ReturnGameObjectToPool()//動畫事件或OnCollision觸發，特效用coroutine收
    {
        switch (SCurrentState)
        {
            case "Transform":
                ObjectPool.ReturnGameObjectToPool(FXNumberOne, PoolKey.BossFX_Ice, BossFX_Ice.IceTransformStart);
                ObjectPool.ReturnGameObjectToPool(FXNumberTwo, PoolKey.BossFX_Ice, BossFX_Ice.IceTransformOver);
                break;
            case "Jump":
                ObjectPool.ReturnGameObjectToPool(FXNumberOne, PoolKey.BossFX_Ice, BossFX_Ice.IceJumpUp);
                ObjectPool.ReturnGameObjectToPool(FXNumberTwo, PoolKey.BossFX_Ice, BossFX_Ice.IceJumpDown);
                break;
            case "Attack1":
                ObjectPool.ReturnGameObjectToPool(FXNumberOne, PoolKey.BossFX_Ice, BossFX_Ice.Ice1OnHand);
                ObjectPool.ReturnGameObjectToPool(FXNumberTwo, PoolKey.BossFX_Ice, BossFX_Ice.Ice1Fan);
                break;
            case "Attack2":
                ObjectPool.ReturnGameObjectToPool(FXNumberOne, PoolKey.BossFX_Ice, BossFX_Ice.Ice2IceLance);
                ObjectPool.ReturnGameObjectToPool(FXNumberTwo, PoolKey.BossFX_Ice, BossFX_Ice.Ice2IceLance);
                break;
            case "Attack3":
                ObjectPool.ReturnGameObjectToPool(FXNumberOne, PoolKey.BossFX_Ice, BossFX_Ice.Ice1OnHand);
                ObjectPool.ReturnGameObjectToPool(FXNumberTwo, PoolKey.BossFX_Ice, BossFX_Ice.Ice1OnHand);
                break;
            default:
                print("找不到你要回收的物件R~");
                break;
        }
    }

    public sealed override void Update()
    {
        base.Update();
        //招式飛行
        if (bJumpSwitch)bossStats.Jump();
    }

    public override void Awake()
    {
        base.Awake();
        AddFxChildren();
        bossStats = GetComponentInParent<BossStats>();
        if (bossStats == null) print("boss stats GG");
    }

    public sealed override void Start()
    {
        base.Start();
        AddParticleAttackBoxes();
    }


    public sealed override bool AnyState()
    {
        if (bossStats.Hp <= 0f)
        {
            SNextState = "Death";
        }
        else if (bossStats.Endurance < 5f)
        {
            if (random.NextDouble() > 0.5f)
            {
                SNextState = "GetHit1";
            }
            else
            {
                SNextState = "GetHit2";
            }  
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
