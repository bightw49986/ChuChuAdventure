using ResourcesManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public sealed class FireFSMGenerater : NPCFSMGenerater
{
    System.Random random = new System.Random();
    BossStats bossStats;
    string sReturnState;
    Dictionary<string, Transform> FxPoint = new Dictionary<string, Transform>();//先是取得我要的名稱的game objects，除了可指定特效初始位置，也可在狀態機的Do改變transform做出射出技能的效果
    [HideInInspector]public GameObject FXNumberOne , FXNumberTwo;//物件池access出來的物件容器s


    public override void InitState(StateSystem state)
    {
        base.InitState(state);
        state.bossStats = bossStats;
    }

    void AddParticleAttackBoxes()
    {
        bossStats.AddParticleAttackBoxes(BossFX_Fire.Fire2FireBall);
        bossStats.AddParticleAttackBoxes(BossFX_Fire.Fire1OnArm);
        bossStats.AddParticleAttackBoxes(BossFX_Fire.Fire3Onfloor);
        bossStats.AddParticleAttackBoxes(BossFX_Fire.Fire3Onhand);
        bossStats.AddParticleAttackBoxes(BossFX_Fire.FireJumpDown);
        bossStats.AddParticleAttackBoxes(BossFX_Fire.FireJumpUp);
        bossStats.AddParticleAttackBoxes(BossFX_Fire.FireTransformOver);
        bossStats.AddParticleAttackBoxes(BossFX_Fire.FireTransformStart);
    }

    //void CastSkillBox()
    //{
    //    bossStats.CastSkillBox(PoolKey.BossFX_Fire, BossFX_Fire.Fire2FireBall);
    //    bossStats.CastSkillBox(PoolKey.BossFX_Fire, BossFX_Fire.Fire1OnArm);
    //    bossStats.CastSkillBox(PoolKey.BossFX_Fire, BossFX_Fire.Fire3Onfloor);
    //    bossStats.CastSkillBox(PoolKey.BossFX_Fire, BossFX_Fire.Fire3Onhand);
    //    bossStats.CastSkillBox(PoolKey.BossFX_Fire, BossFX_Fire.FireJumpDown);
    //    bossStats.CastSkillBox(PoolKey.BossFX_Fire, BossFX_Fire.FireJumpUp);
    //    bossStats.CastSkillBox(PoolKey.BossFX_Fire, BossFX_Fire.FireTransformOver);
    //    bossStats.CastSkillBox(PoolKey.BossFX_Fire, BossFX_Fire.FireTransformStart);
    //}

    void AddFxChildren()//在Npc的Prefab下放的空物件，value那個空物件不動，將由各狀態Do()中，每frame改變FXNumberOne , FXNumberTwo的transform做成投射技能(位置與方向)，而技能的擊中特效由OnCollision產生(?)
    {
        FxPoint.Add("FxGroundCenter", transform.FindDeepChild("FxGroundCenter"));
        FxPoint.Add("FxFace", transform.FindDeepChild("FxFace"));
        FxPoint.Add("FxRightHand", transform.FindDeepChild("FxRightHand"));
    }

    void CastSkillBox(int order)//動畫事件傳入編號int觸發
    {
        switch (SCurrentState)
        {
            case "Transform":
                if (order == 1) FXNumberOne = ObjectPool.AccessGameObjectFromPool(PoolKey.BossFX_Fire, BossFX_Fire.FireTransformStart, FxPoint["FxGroundCenter"], FxPoint["FxGroundCenter"].position);
                else if (order == 2) FXNumberTwo = ObjectPool.AccessGameObjectFromPool(PoolKey.BossFX_Fire, BossFX_Fire.FireTransformOver, FxPoint["FxGroundCenter"], FxPoint["FxGroundCenter"].position);
                break;
            case "Jump":
                if (order == 1)
                {
                    bossStats.isJumping = true;
                    FXNumberOne = ObjectPool.AccessGameObjectFromPool(PoolKey.BossFX_Fire, BossFX_Fire.FireJumpUp, FxPoint["FxGroundCenter"], FxPoint["FxGroundCenter"].position);
                }
                else if (order == 2)
                {
                    FXNumberTwo = ObjectPool.AccessGameObjectFromPool(PoolKey.BossFX_Fire, BossFX_Fire.FireJumpDown, FxPoint["FxGroundCenter"], FxPoint["FxGroundCenter"].position);
                }
                break;
            case "Attack1":
                FXNumberOne = ObjectPool.AccessGameObjectFromPool(PoolKey.BossFX_Fire, BossFX_Fire.Fire1OnArm, FxPoint["FxRightHand"], FxPoint["FxRightHand"].position);
                break;
            case "Attack2":
                FXNumberOne = ObjectPool.AccessGameObjectFromPool(PoolKey.BossFX_Fire, BossFX_Fire.Fire2FireBall, FxPoint["FxFace"], FxPoint["FxFace"].position);
                break;
            case "Attack3":
                FXNumberOne = ObjectPool.AccessGameObjectFromPool(PoolKey.BossFX_Fire, BossFX_Fire.Fire3Onhand, FxPoint["FxRightHand"], FxPoint["FxRightHand"].position);
                FXNumberTwo = ObjectPool.AccessGameObjectFromPool(PoolKey.BossFX_Fire, BossFX_Fire.Fire3Onfloor, FxPoint["FxGroundCenter"], FxPoint["FxGroundCenter"].position);
                break;
            default:
                FXNumberOne = FXNumberTwo = null;
                //print("找不到你現在的狀態R~");
                break;
        }
        if (FXNumberOne != null)
        {
            BattleSystem.AttackBox_Skill skillBoxOne = FXNumberOne.GetComponentInChildren<BattleSystem.AttackBox_Skill>();
            skillBoxOne.InitAttackBox(bossStats);
            StartCoroutine("ReturnFx");
        }
        if (FXNumberTwo != null)
        {
            BattleSystem.AttackBox_Skill skillBoxTwo = FXNumberTwo.GetComponentInChildren<BattleSystem.AttackBox_Skill>();
            skillBoxTwo.InitAttackBox(bossStats);
        }       
    }

    IEnumerator ReturnFx()//先統一從第一個特效出來的時，過一定秒數收回所有特效
    {
        sReturnState = SCurrentState;
        yield return new WaitForSeconds(4.5f);
        ReturnGameObjectToPool();
    }
    

    void AimAttack2()
    {
        FXNumberOne.transform.rotation = Quaternion.LookRotation(bossStats.m_vDistanceToPlayer);
        FXNumberOne.transform.Translate(bossStats.m_vDistanceToPlayer.normalized * Time.deltaTime);//Coroutine?
    }

    void AimAttack3()
    {
        FXNumberTwo.transform.rotation = Quaternion.LookRotation(bossStats.m_vDistanceToPlayer);
    }

    void ReturnGameObjectToPool()//動畫事件或OnCollision觸發，只要動畫不loop就AllowTransit時一起收
    {
        switch (sReturnState)
        {
            case "Transform":
                ObjectPool.ReturnGameObjectToPool(FXNumberOne, PoolKey.BossFX_Fire, BossFX_Fire.FireTransformStart);
                ObjectPool.ReturnGameObjectToPool(FXNumberTwo, PoolKey.BossFX_Fire, BossFX_Fire.FireTransformOver);
                break;
            case "Jump":
                ObjectPool.ReturnGameObjectToPool(FXNumberOne, PoolKey.BossFX_Fire, BossFX_Fire.FireJumpUp);
                ObjectPool.ReturnGameObjectToPool(FXNumberTwo, PoolKey.BossFX_Fire, BossFX_Fire.FireJumpDown);
                break;
            case "Attack1":
                ObjectPool.ReturnGameObjectToPool(FXNumberOne, PoolKey.BossFX_Fire, BossFX_Fire.Fire1OnArm);
                break;
            case "Attack2":
                ObjectPool.ReturnGameObjectToPool(FXNumberOne, PoolKey.BossFX_Fire, BossFX_Fire.Fire2FireBall);
                break;
            case "Attack3":
                ObjectPool.ReturnGameObjectToPool(FXNumberOne, PoolKey.BossFX_Fire, BossFX_Fire.Fire3Onhand);
                ObjectPool.ReturnGameObjectToPool(FXNumberTwo, PoolKey.BossFX_Fire, BossFX_Fire.Fire3Onfloor);
                break;
            default:
                print("找不到你要回收的物件R~");
                break;
        }
    }

    public sealed override void Update()
    {
        base.Update();
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
