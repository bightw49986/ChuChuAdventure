using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

/// <summary>
/// 咱們可愛的ChuChu
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour {
    public static bool bCanMove = true; //主角當前可否移動
    public static bool bCanAttack = true;//主角現在狀態能否攻擊
    public static bool bHarmless = false;//主角現在狀態是否無敵，未完成！
    public static bool bCanJump = true;//主角現在狀態能否跳躍 //Maybe過時！
    public static bool bCanDash = true;//主角現在狀態能否跳躍
    public static bool bFuckTheGravity = false;//主角能否不屑萬有引力

    public static bool bBlockInput = false; //
    public static float fBlockTime = 0; //
    public static bool bPreEnter = false;//玩家是否記錄預輸入
    public static float fRecordTime=0; //

    public static float fMouseInputSpeed = 1.0f;//滑鼠靈敏度
    public static float fAttackSpeedRate = 1f;//攻擊速度倍率
    public static float fToughness = 10f;//強韌度
    public static float fLoseRecoverySpeedRate = 1f;//回血丟失速度倍率
    public static float fHP = 100f;//血量
    public static float fPower = 0f;//無雙值
}
