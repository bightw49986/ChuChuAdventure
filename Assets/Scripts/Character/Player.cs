using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

/// <summary>
/// 咱們可愛的ChuChu
/// </summary>
public class Player : MonoBehaviour
{
    public bool bCanMove = true; //主角當前可否移動
    public bool bCanAttack = true;//主角現在狀態能否攻擊
    public bool bHarmless = false;//主角現在狀態是否無敵，未完成！
    public bool bCanJump = true;//主角現在狀態能否跳躍 
    public bool bCanDash = true;//主角現在狀態能否跳躍
    public bool bFuckTheGravity = false;//主角能否不屑萬有引力

    public float fMouseInputSpeed = 1.0f;//滑鼠靈敏度
    public float fAttackSpeedRate = 1f;//攻擊速度倍率
    public float fToughness = 10f;//強韌度
    public float fLoseRecoverySpeedRate = 1f;//回血丟失速度倍率
    public float fHP = 100f;//血量
    public float fPower = 0f;//無雙值

    public bool bBlockInput = false; //
    //public static float fBlockTime = 0; //
    public bool bPreEnter = false;//玩家是否記錄預輸入
    //public float fRecordTime = 0f;
}
