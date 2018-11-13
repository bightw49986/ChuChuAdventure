using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResourcesManagement
{
    public enum PoolKey { Npc = 0, SkillFX = 1, HitFX = 2, Other = 3 }
    public enum NpcKey { Goblin = 0 }
    public enum SkillFXKey {}
    public enum HitFXKey {}
    public enum OtherKey {}

    public class ObjectPool : MonoBehaviour ,ISingleton
    {
        public Dictionary<PoolKey,Dictionary<Enum,GameObject>> References;
        Dictionary<PoolKey, Dictionary<Enum, List<PoolingObject>>> m_pool;
        public int DefaultInitCount = 10;

        void Awake()
        {
            Init();
        }


        void Start()
        {

        }

        void Init()
        {
            SingletonLized();
            m_pool = new Dictionary<PoolKey, Dictionary<Enum, List<PoolingObject>>>();
            LoadReferences();
        }

        void LoadReferences()
        {
            References = new Dictionary<PoolKey, Dictionary<Enum, GameObject>>
            {

            };
        }

        public void InstantiatePoolObjects(int iCount, PoolKey pool, Enum type, UnityEngine.Object obj)
        {
            List<PoolingObject> pList;
            Dictionary<Enum, List<PoolingObject>> subPool;
            if (obj == null) return;

            if (!m_pool.ContainsKey(pool))
            {
                subPool = new Dictionary<Enum, List<PoolingObject>>();
                pList = new List<PoolingObject>();
                subPool.Add(type,pList);
                m_pool.Add(pool, subPool);
            }
            else if (!m_pool[pool].ContainsKey(type))
            {
                pList = new List<PoolingObject>();
                m_pool[pool].Add(type,pList);
            }
            else
            {
                pList = m_pool[pool][type];
            }
            for (int i = 0; i < iCount; i++)
            {
                PoolingObject poolData = new PoolingObject();
                GameObject go = Instantiate(obj) as GameObject;
                go.SetActive(false);
                poolData.m_gameObject = go;
                poolData.m_isUsing = false;
                pList.Add(poolData);
            }
        }

        public void InstantiatePoolObjects(int iCount, PoolKey pool, Enum type, UnityEngine.Object obj, Transform parent)
        {
            List<PoolingObject> pList;
            Dictionary<Enum, List<PoolingObject>> subPool;

            if (obj == null) return;

            if (!m_pool.ContainsKey(pool))
            {
                subPool = new Dictionary<Enum, List<PoolingObject>>();
                pList = new List<PoolingObject>();
                subPool.Add(type, pList);
                m_pool.Add(pool, subPool);
            }
            else if (!m_pool[pool].ContainsKey(type))
            {
                pList = new List<PoolingObject>();
                m_pool[pool].Add(type, pList);
            }
            else
            {
                pList = m_pool[pool][type];
            }
            for (int i = 0; i < iCount; i++)
            {
                PoolingObject poolData = new PoolingObject();
                GameObject go = Instantiate(obj,parent) as GameObject;
                go.SetActive(false);
                poolData.m_gameObject = go;
                poolData.m_isUsing = false;
                pList.Add(poolData);
            }
        }

        /// <summary>
        /// 從物件池拿一份指定類型的GameObject
        /// </summary>
        /// <returns>從池裡拿到的物件</returns>
        /// <param name="pool">該物件所在的母池</param>
        /// <param name="type">該物件的類型</param> 
        public GameObject AccessGameObjectFromPool(PoolKey pool,Enum type)
        {
            List<PoolingObject> pList;
            if (m_pool.ContainsKey(pool) == false || m_pool[pool].ContainsKey(type)==false)
            {
                Debug.LogError("物件不在池子裡卻呼叫了AccessGameObjectFromPool(須先呼叫InstantiatePoolObjects)");
                return null;
            }
            pList = m_pool[pool][type];
            if(pList.Count == 0)
            {
                Debug.LogError("物件不在池子裡卻呼叫了AccessGameObjectFromPool(須先呼叫InstantiatePoolObjects)");
                return null;
            }
            UnityEngine.Object obj = pList[0].m_gameObject;
            GameObject goToReturn = null;
            while (goToReturn == null)
            {
                for (int i = 0; i < pList.Count; i++)
                {
                    if (pList[i].m_isUsing == false)
                    {
                        pList[i].m_isUsing = true;
                        goToReturn = pList[i].m_gameObject;
                        goToReturn.SetActive(true);
                        break;
                    }
                }
                if (goToReturn == null)
                {
                    InstantiatePoolObjects(1, pool,type,obj);
                    Debug.LogWarning("池子物件: " + type + "不夠用，多生成一份");
                }
                else break;
            }
            return goToReturn;
        }

        public void ReturnGameObjectToPool(GameObject go, PoolKey pool , Enum type)
        {
            List<PoolingObject> pList;
            if (m_pool.ContainsKey(pool) == false || m_pool[pool].ContainsKey(type) == false)
            {
                Debug.LogError("池裡沒有此種物件！");
                return;
            }
            pList = m_pool[pool][type];
            for (int i = 0; i < pList.Count; i++)
            {
                if (pList[i].m_isUsing == true && pList[i].m_gameObject == go)
                {
                    go.SetActive(false);
                    pList[i].m_isUsing = false;
                    break;
                }
            }
        }

        public void ClearPool()
        {
            m_pool.Clear();
            Resources.UnloadUnusedAssets();
        }

        public void SingletonLized()
        {
            if (FindObjectsOfType<ObjectPool>().Length > 1)
            {
                Destroy(gameObject);
            }
            else
            {
                DontDestroyOnLoad(gameObject);
            }
        }
    }
    public class PoolingObject
    {
        public GameObject m_gameObject;
        public bool m_isUsing;
    }
}
