using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStats : MonoBehaviour
{
    public static float fHP = 100f;//血量
    public static float fToughness = 10f;//強韌度
    public static float fTransformCD = 40f;//變身的CD時間
    public static float fIceOrFireTime = 25f;//變身時間
    public static bool bCanMove = true; //主角當前可否移動
    public static bool bCanAttack = true;//主角現在狀態能否攻擊
    public static bool bCanJump = true;//主角現在狀態能否跳躍 
    [SerializeField]protected internal GameObject StoneGolem;
    [SerializeField]protected internal GameObject FireGolem;
    [SerializeField]protected internal GameObject IceGolem;

    // Use this for initialization
    void Start () {
        StoneGolem.SetActive(true);
    }

    void SwitchBoss()
    {
        

    }

    private static void Destroy(GameObject bosses)
    {
        throw new NotImplementedException();
    }

    // Update is called once per frame
    void Update () {
		
	}
}
