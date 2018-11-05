using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStats : MonoBehaviour
{
    System.Random random = new System.Random();//隨機機
    public float fHP = 100f;//血量
    public float fToughness = 10f;//強韌度
    public bool BCanTransform { get; private set; }//Boss能不能變形了
    private float fStoneTime = 15f;//只有石頭的時間
    public readonly float fIceOrFireTime = 25f;//有屬性的時間
    public bool bCanMove = true; //Boss可否移動
    public bool bCanAttack = true;//Boss能否攻擊
    public bool bCanJump = true;//Boss能否跳躍 
    [SerializeField]protected internal GameObject StoneGolem;
    [SerializeField]protected internal GameObject FireGolem;
    [SerializeField]protected internal GameObject IceGolem;

    // 叫石頭出來
    void Start () {
        StoneGolem.SetActive(true);
    }
    //變身術
    public void SwitchBoss()
    {
        if (StoneGolem.activeInHierarchy == true)
        {
            StartCoroutine(OnfireOrIce());
        }
        else if (FireGolem.activeInHierarchy == true || IceGolem.activeInHierarchy == true)
        {
            StartCoroutine(ExtinguishfireOrIce());
        }
    }
     //有一半機率變火或冰
    IEnumerator OnfireOrIce()
    {
        BCanTransform = false;
        StoneGolem.SetActive(false);
        if (random.NextDouble() > 0.5f)
        {
            FireGolem.SetActive(true);
        }
        else
        {
            IceGolem.SetActive(true);
        }        
        yield return new WaitForSeconds(fIceOrFireTime);
        BCanTransform = true;
    }
    //時間到就變回石頭
    IEnumerator ExtinguishfireOrIce()
    {
        BCanTransform = false;
        StoneGolem.SetActive(true);
        FireGolem.SetActive(false);
        IceGolem.SetActive(false);
        yield return new WaitForSeconds(fStoneTime);
        BCanTransform = true;
    }
}
