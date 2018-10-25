using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 咱們可愛的Player
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    public Rigidbody m_Rig; //玩家的剛體
    public bool m_bMoveAble = true; //玩家當前可否移動
    public float m_fMoveSpeed; //玩家的移動速度
    public bool m_bAttackAble;
    public bool m_bHarmless;
    public bool m_bCanJump;
    public string m_currentState = "Idle";

    public Dictionary<string, float> stats = new Dictionary<string, float>()
    {
          { "Toughness",10 },
          { "HP",100 },
    };

    void Awake()
    {
        m_Rig = GetComponent<Rigidbody>();
    }

}
