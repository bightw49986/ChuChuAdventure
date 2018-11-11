using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ObjectPool
{
    public class ObjectPool : MonoBehaviour ,ISingleton
    {
        Dictionary<PoolingObject.PoolKey, List<PoolingObject>> m_pool;

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
            m_pool = new Dictionary<PoolingObject.PoolKey, List<PoolingObject>>();
        }

        public void InstantiatePoolObjects(int iCount, PoolingObject.PoolKey pKey, Object obj)
        {
            List<PoolingObject> pList;

            if (obj == null) return;

            if (!m_pool.ContainsKey(pKey))
            {
                pList = new List<PoolingObject>();
                m_pool.Add(pKey, pList);
            }
            else
            {
                pList = m_pool[pKey];
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

        public void InstantiatePoolObjects(int iCount, PoolingObject.PoolKey pKey, Object obj, Transform parent)
        {
            List<PoolingObject> pList;

            if (obj == null) return;

            if (!m_pool.ContainsKey(pKey))
            {
                pList = new List<PoolingObject>();
                m_pool.Add(pKey, pList);
            }
            else
            {
                pList = m_pool[pKey];
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
        /// <param name="pKey">該物件在池裡的Key</param>
        public GameObject AccessGameObjectFromPool(PoolingObject.PoolKey pKey)
        {
            List<PoolingObject> pList;
            if (m_pool.ContainsKey(pKey) == false)
            {
                Debug.LogError("物件不在池子裡卻呼叫了AccessGameObjectFromPool(須先呼叫InstantiatePoolObjects)");
                return null;
            }
            pList = m_pool[pKey];
            if(pList.Count == 0)
            {
                Debug.LogError("物件不在池子裡卻呼叫了AccessGameObjectFromPool(須先呼叫InstantiatePoolObjects)");
                return null;
            }
            Object obj = pList[0].m_gameObject;
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
                    InstantiatePoolObjects(1, pKey, obj);
                    Debug.LogWarning("池子物件: " + pKey + "不夠用，多生成一份");
                }
                else break;
            }
            return goToReturn;
        }

        public void ReturnGameObjectToPool(GameObject go, PoolingObject.PoolKey key)
        {
            List<PoolingObject> pList;
            if (!m_pool.ContainsKey(key))
            {
                Debug.LogError("池裡沒有此種物件！");
                return;
            }
            pList = m_pool[key];
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
        public enum PoolKey {HitFX =0, Goblin = 1};

    }
}
