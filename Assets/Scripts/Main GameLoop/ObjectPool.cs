using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResourcesManagement
{
    //給中榮：我enum目錄的部分打完，需要做的是到LoadRefereces那邊，把每一個都用Resources.Load讀一次，然後你外面的class要用，就找到objectpool(可以用tag找Main，然後Get Component)，然後在開始做事前用PreparePoolObject叫他預先生成，再用AccessGameObjectFromPool拿，用完再ReturnGameObjectToPool(可能要想一下怎麼知道用完的Callback)
    //目前AccessGameObjectFromPool 沒有做設定parent跟position rotation的多載，所以外面的class拿出去之後要記得設定，不然他會噴在0,0,0
    public enum PoolKey {None =-1,  Npc = 0, PlayerFX = 1, HitFX = 2, BossFX_Fire = 3, BossFX_Ice= 4, BossFX_Stone= 5, NpcFX = 6, Other = 7 }

    public enum Npc { None = -1, Goblin = 0 }
    public enum PlayerFX { None = -1, HeavySpearEffect = 0, LightSpearEffect  = 1, Madness = 2 }
    public enum HitFX { None = -1, Hit_Blue1 = 0, Hit_Blue2 = 1, Hit_Blue3 = 2, Hit_Red1 = 3, Hit_Red2 = 4, Hit_Red3 = 5, Hit_White1 = 6, Hit_White2 = 7, Hit_White3 = 8, Hit_White4 = 9,  }
    public enum BossFX_Fire { None = -1, Fire2FireBall = 0, Fire2HitEffect = 1, Fire3Onfloor = 2, Fire3Onhand = 3, FireJumpDown = 4, FireJumpUp = 5 , FireTransformOver = 6, FireTransformStart = 7  }
    public enum BossFX_Ice { None = -1, Ice1Fan = 0, Ice1OnHand = 1, Ice2IceLance = 2, Ice3FistsExplosion = 3, IceJumpDown = 4, IceJumpUp = 5, IceTransformOver = 6, IceTransformStart = 7,  }
    public enum BossFX_Stone{ None = -1, Stone1Clap = 0, Stone2Throw = 1, Stone3Floor = 2, StoneTransformOver = 3, StoneTransformStart = 4 }
    public enum NpcFX { None = -1, }
    public enum Other { None = -1, }

    public class ObjectPool : MonoBehaviour ,ISingleton
    {
        public Dictionary<PoolKey,Dictionary<Enum,UnityEngine.Object>> References;
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
            //LoadReferences();

        }

        /// <summary>undone
        /// 把所有會讀到的物件都先讀起來一份
        /// </summary>
        void LoadReferences() 
        {
            References = new Dictionary<PoolKey, Dictionary<Enum, UnityEngine.Object>>
            {
                //把上面每一個enum都做一次Resources.Load，注意檔案路徑跟括號逗號，我先讀兩行提供參考
                {PoolKey.PlayerFX, new Dictionary<Enum, UnityEngine.Object>{{PlayerFX.HeavySpearEffect, Resources.Load("PlayerFX/HeavySpearEffect") },{PlayerFX.LightSpearEffect, Resources.Load("PlayerFX/LightSpearEffect") },{PlayerFX.Madness, Resources.Load("PlayerFX/Madness") }}},
                {PoolKey.HitFX, new Dictionary<Enum, UnityEngine.Object>{{HitFX.Hit_Blue1, Resources.Load("HitFX/Blue/Hit_Blue1") },{HitFX.Hit_Blue2,Resources.Load("HitFX/Blue/Hit_Blue2") }}} //剩下以此類推
            };
        }

        /// <summary>
        /// 外部的Class用這個方法叫池子準備物件(從預讀的References裡面Instantiate)
        /// </summary>
        /// <param name="iNum">要預生成幾份</param>
        /// <param name="pool">哪個母池</param>
        /// <param name="type">哪種物件</param>
        public void PreparePoolObject(int iNum, PoolKey pool, Enum type)
        {
            InstantiatePoolObjects(iNum, pool, type, References[pool][type]);
        }

        /// <summary>
        /// 生一份物件到池裡
        /// </summary>
        /// <param name="iCount">I count.</param>
        /// <param name="pool">Pool.</param>
        /// <param name="type">Type.</param>
        /// <param name="obj">Object.</param>
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
                poolData.m_origin = go.transform;
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
                Debug.LogError("物件不在池子裡卻呼叫了AccessGameObjectFromPool(須先準備物件)");
                return null;
            }
            pList = m_pool[pool][type];
            if(pList.Count == 0)
            {
                Debug.LogError("物件不在池子裡卻呼叫了AccessGameObjectFromPool(須先準備物件)");
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

        /// <summary>
        /// 從池裡拿一份指定類型的Gameobject
        /// </summary>
        /// <returns>The game object from pool.</returns>
        /// <param name="pool">Pool.</param>
        /// <param name="type">Type.</param>
        /// <param name="parent">指定他的parent</param>
        public GameObject AccessGameObjectFromPool(PoolKey pool, Enum type, Transform parent)
        {
            List<PoolingObject> pList;
            if (m_pool.ContainsKey(pool) == false || m_pool[pool].ContainsKey(type) == false)
            {
                Debug.LogError("物件不在池子裡卻呼叫了AccessGameObjectFromPool(須先準備物件)");
                return null;
            }
            pList = m_pool[pool][type];
            if (pList.Count == 0)
            {
                Debug.LogError("物件不在池子裡卻呼叫了AccessGameObjectFromPool(須先準備物件)");
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
                        goToReturn.transform.parent = parent;
                        goToReturn.SetActive(true);
                        break;
                    }
                }
                if (goToReturn == null)
                {
                    InstantiatePoolObjects(1, pool, type, obj);
                    Debug.LogWarning("池子物件: " + type + "不夠用，多生成一份");
                }
                else break;
            }
            return goToReturn;
        }

        /// <summary>
        /// 從池裡拿一份指定類型的Gameobject
        /// </summary>
        /// <returns>The game object from pool.</returns>
        /// <param name="pool">Pool.</param>
        /// <param name="type">Type.</param>
        /// <param name="position">指定他的位置</param>
        public GameObject AccessGameObjectFromPool(PoolKey pool, Enum type, Vector3 position)
        {
            List<PoolingObject> pList;
            if (m_pool.ContainsKey(pool) == false || m_pool[pool].ContainsKey(type) == false)
            {
                Debug.LogError("物件不在池子裡卻呼叫了AccessGameObjectFromPool(須先準備物件)");
                return null;
            }
            pList = m_pool[pool][type];
            if (pList.Count == 0)
            {
                Debug.LogError("物件不在池子裡卻呼叫了AccessGameObjectFromPool(須先準備物件)");
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
                        goToReturn.transform.parent = null;
                        goToReturn.transform.position = position;
                        goToReturn.transform.rotation = Quaternion.identity;
                        goToReturn.SetActive(true);
                        break;
                    }
                }
                if (goToReturn == null)
                {
                    InstantiatePoolObjects(1, pool, type, obj);
                    Debug.LogWarning("池子物件: " + type + "不夠用，多生成一份");
                }
                else break;
            }
            return goToReturn;
        }

        /// <summary>
        /// 從池裡拿一份指定類型的Gameobject
        /// </summary>
        /// <returns>The game object from pool.</returns>
        /// <param name="pool">Pool.</param>
        /// <param name="type">Type.</param>
        /// <param name="parent">指定他的parent</param>
        /// /// <param name="position">指定他的位置</param>
        public GameObject AccessGameObjectFromPool(PoolKey pool, Enum type,Transform parent, Vector3 position)
        {
            List<PoolingObject> pList;
            if (m_pool.ContainsKey(pool) == false || m_pool[pool].ContainsKey(type) == false)
            {
                Debug.LogError("物件不在池子裡卻呼叫了AccessGameObjectFromPool(須先準備物件)");
                return null;
            }
            pList = m_pool[pool][type];
            if (pList.Count == 0)
            {
                Debug.LogError("物件不在池子裡卻呼叫了AccessGameObjectFromPool(須先準備物件)");
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
                        goToReturn.transform.parent = parent;
                        goToReturn.transform.position = position;
                        goToReturn.transform.rotation = Quaternion.identity;
                        goToReturn.SetActive(true);
                        break;
                    }
                }
                if (goToReturn == null)
                {
                    InstantiatePoolObjects(1, pool, type, obj);
                    Debug.LogWarning("池子物件: " + type + "不夠用，多生成一份");
                }
                else break;
            }
            return goToReturn;
        }

        /// <summary>
        /// 從池裡拿一份指定類型的Gameobject
        /// </summary>
        /// <returns>The game object from pool.</returns>
        /// <param name="pool">Pool.</param>
        /// <param name="type">Type.</param>
        /// <param name="position">指定他的位置</param>
        /// <param name="rotation">指定他的Rotation，沒要特別處理就給Quaternion.Identity</param>
        public GameObject AccessGameObjectFromPool(PoolKey pool, Enum type, Vector3 position , Quaternion rotation)
        {
            List<PoolingObject> pList;
            if (m_pool.ContainsKey(pool) == false || m_pool[pool].ContainsKey(type) == false)
            {
                Debug.LogError("物件不在池子裡卻呼叫了AccessGameObjectFromPool(須先準備物件)");
                return null;
            }
            pList = m_pool[pool][type];
            if (pList.Count == 0)
            {
                Debug.LogError("物件不在池子裡卻呼叫了AccessGameObjectFromPool(須先準備物件)");
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
                        goToReturn.transform.parent = null;
                        goToReturn.transform.position = position;
                        goToReturn.transform.rotation = rotation;
                        goToReturn.SetActive(true);
                        break;
                    }
                }
                if (goToReturn == null)
                {
                    InstantiatePoolObjects(1, pool, type, obj);
                    Debug.LogWarning("池子物件: " + type + "不夠用，多生成一份");
                }
                else break;
            }
            return goToReturn;
        }

        /// <summary>
        /// 從池裡拿一份指定類型的Gameobject
        /// </summary>
        /// <returns>The game object from pool.</returns>
        /// <param name="pool">Pool.</param>
        /// <param name="type">Type.</param>
        /// <param name="parent">指定他的parent</param>
        /// <param name="position">指定他的位置</param>
        /// <param name="rotation">指定他的Rotation</param>
        public GameObject AccessGameObjectFromPool(PoolKey pool, Enum type, Transform parent, Vector3 position, Quaternion rotation)
        {
            List<PoolingObject> pList;
            if (m_pool.ContainsKey(pool) == false || m_pool[pool].ContainsKey(type) == false)
            {
                Debug.LogError("物件不在池子裡卻呼叫了AccessGameObjectFromPool(須先準備物件)");
                return null;
            }
            pList = m_pool[pool][type];
            if (pList.Count == 0)
            {
                Debug.LogError("物件不在池子裡卻呼叫了AccessGameObjectFromPool(須先準備物件)");
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
                        goToReturn.transform.parent = parent;
                        goToReturn.transform.position = position;
                        goToReturn.transform.rotation = rotation;
                        goToReturn.SetActive(true);
                        break;
                    }
                }
                if (goToReturn == null)
                {
                    InstantiatePoolObjects(1, pool, type, obj);
                    Debug.LogWarning("池子物件: " + type + "不夠用，多生成一份");
                }
                else break;
            }
            return goToReturn;
        }

        /// <summary>
        /// 把GameObject還給物件池，並重設他的transform
        /// </summary>
        /// <param name="go">要還的物件</param>
        /// <param name="pool">Pool.</param>
        /// <param name="type">Type.</param>
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
                    go.transform.parent = pList[i].m_origin.parent;
                    go.transform.position = pList[i].m_origin.position;
                    go.transform.rotation = pList[i].m_origin.rotation;
                    go.transform.localScale = pList[i].m_origin.localScale;
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
        public Transform m_origin;
    }
}
