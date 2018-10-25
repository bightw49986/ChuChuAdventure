using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class AIData  {
    public float m_fRadius;
    public float m_fProbeLength;   
    public float m_Speed;
    public float m_fMaxSpeed;
    public float m_fRot;
    public float m_fMaxRot;
    public GameObject m_Go;


    public float m_fSight;
    public float m_fAttackRange;

    [HideInInspector]
    public float m_fAttackTime;
    public float m_fHp;
    [HideInInspector]
    public GameObject m_TargetObject;

    [HideInInspector]
    public Vector3 m_vTarget;
    [HideInInspector]
    public float m_fTempTurnForce;
    [HideInInspector]
    public float m_fMoveForce;
    [HideInInspector]
    public bool m_bMove;

    [HideInInspector]
    public bool m_bCol;

    [HideInInspector]
    public FSMSystem m_FSMSystem;

}


public class AIFunction
{
    public static GameObject CheckEnemyInSight(AIData data, ref bool bAttack)
    {
        GameObject go = Main.m_Instance.GetPlayer();
        Vector3 v = go.transform.position - data.m_Go.transform.position;
        float fDist = v.magnitude;
        if (fDist < data.m_fAttackRange)
        {
            bAttack = true;
            return go;
        }
        else if (fDist < data.m_fSight)
        {
            bAttack = false;
            return go;
        }
        return null;
    }

    public static bool CheckTargetEnemyInSight(AIData data, GameObject target, ref bool bAttack)
    {
        GameObject go = target;
        Vector3 v = go.transform.position - data.m_Go.transform.position;
        float fDist = v.magnitude;
        if (fDist < data.m_fAttackRange)
        {
            bAttack = true;
            return true;
        }
        else if (fDist < data.m_fSight)
        {
            bAttack = false;
            return true;
        }
        return false;
    }
}