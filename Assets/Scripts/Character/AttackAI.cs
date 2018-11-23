using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAI : MonoBehaviour
{

    HashSet<GameObject> m_NearEnemyList;
    void Awake()
    {
        m_NearEnemyList = GetComponentInParent<PlayerFSMGenerater>().NearEnemyList;
        if (m_NearEnemyList == null) print("No m_PlayerFSMGenerater");
    }


    /// <summary>
    /// 進入攻擊範圍後記錄名單
    /// </summary>
    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Enemy" && m_NearEnemyList.Contains(collider.gameObject) == false)
        {

            m_NearEnemyList.Add(collider.gameObject);
            //print(m_NearEnemyList.Count);
        }
    }
    /// <summary>
    /// 離開攻擊範圍後刪除名單
    /// </summary>
    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Enemy" && m_NearEnemyList.Contains(collider.gameObject))
        {

            m_NearEnemyList.Remove(collider.gameObject);
            //print(m_NearEnemyList.Count);
        }
    }
}

