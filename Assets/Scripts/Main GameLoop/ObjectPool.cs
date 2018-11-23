using System;
using System.Collections.Generic;
using UnityEngine;

namespace ResourcesManagement
{
    public enum PoolKey { None = -1, Npc = 0, PlayerFX = 1, HitFX = 2, BossFX_Fire = 3, BossFX_Ice = 4, BossFX_Stone = 5, NpcFX = 6, Other = 7 }

    public enum Npc { None = -1, Goblin = 0 }
    public enum PlayerFX { None = -1, HeavySpearEffect = 0, LightSpearEffect = 1, Madness = 2 }
    public enum HitFX { None = -1, Hit_Blue1 = 0, Hit_Blue2 = 1, Hit_Blue3 = 2, Hit_Red1 = 3, Hit_Red2 = 4, Hit_Red3 = 5, Hit_White1 = 6, Hit_White2 = 7, Hit_White3 = 8, Hit_White4 = 9, }
    public enum BossFX_Fire { None = -1, Fire2FireBall = 0, Fire2HitEffect = 1, Fire3Onfloor = 2, Fire3Onhand = 3, FireJumpDown = 4, FireJumpUp = 5, FireTransformOver = 6, FireTransformStart = 7, Fire1OnArm = 8 }
    public enum BossFX_Ice { None = -1, Ice1Fan = 0, Ice1OnHand = 1, Ice2IceLance = 2, Ice3FistsExplosion = 3, IceJumpDown = 4, IceJumpUp = 5, IceTransformOver = 6, IceTransformStart = 7, }
    public enum BossFX_Stone { None = -1, Stone1Clap = 0, Stone2Throw = 1, Stone3Floor = 2, StoneTransformOver = 3, StoneTransformStart = 4 }
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
            LoadReferences();

        }

        /// <summary>undone
        /// 把所有會讀到的物件都先讀起來一份
        /// </summary>
        void LoadReferences()
        {
            References = new Dictionary<PoolKey, Dictionary<Enum, UnityEngine.Object>>
            {
                { PoolKey.PlayerFX, new Dictionary<Enum, UnityEngine.Object>{{PlayerFX.HeavySpearEffect, Resources.Load("PlayerFX/HeavySpearEffect") },//待套入
                                                                            { PlayerFX.LightSpearEffect, Resources.Load("PlayerFX/LightSpearEffect") },//待套入
                                                                            { PlayerFX.Madness, Resources.Load("PlayerFX/Madness") }}},//待套入
                { PoolKey.HitFX, new Dictionary<Enum, UnityEngine.Object>{{HitFX.Hit_Blue1,Resources.Load("HitFX/Blue/Hit_Blue1") },
                                                                         {HitFX.Hit_Blue2,Resources.Load("HitFX/Blue/Hit_Blue2") },
                                                                         {HitFX.Hit_Blue3,Resources.Load("HitFX/Blue/Hit_Blue3") },
                                                                         {HitFX.Hit_Red1,Resources.Load("HitFX/Red/Hit_Red1") },
                                                                         {HitFX.Hit_Red2,Resources.Load("HitFX/Red/Hit_Red2") },
                                                                         {HitFX.Hit_Red3,Resources.Load("HitFX/Red/Hit_Red3") },
                                                                         {HitFX.Hit_White1,Resources.Load("HitFX/White/Hit_White1") },
                                                                         {HitFX.Hit_White2,Resources.Load("HitFX/White/Hit_White2") },
                                                                         {HitFX.Hit_White3,Resources.Load("HitFX/White/Hit_White3") },
                                                                         {HitFX.Hit_White4,Resources.Load("HitFX/White/Hit_White4") }} } ,
                { PoolKey.BossFX_Fire,new Dictionary<Enum, UnityEngine.Object> { { BossFX_Fire.Fire2FireBall,Resources.Load("BossFX/Fire/Fire2FireBall") },
                                                                                { BossFX_Fire.Fire2HitEffect,Resources.Load("BossFX/Fire/Fire2HitEffect") },
                                                                                { BossFX_Fire.Fire3Onfloor,Resources.Load("BossFX/Fire/Fire3Onfloor") },
                                                                                { BossFX_Fire.Fire3Onhand,Resources.Load("BossFX/Fire/Fire3Onhand") },
                                                                                { BossFX_Fire.FireJumpDown,Resources.Load("BossFX/Fire/FireJumpDown") },
                                                                                { BossFX_Fire.FireJumpUp,Resources.Load("BossFX/Fire/FireJumpUp") },
                                                                                { BossFX_Fire.Fire1OnArm,Resources.Load("BossFX/Fire/Fire1OnArm") },
                                                                                { BossFX_Fire.FireTransformOver,Resources.Load("BossFX/Fire/FireTransformOver") },
                                                                                { BossFX_Fire.FireTransformStart,Resources.Load("BossFX/Fire/FireTransformStart") }} },
                { PoolKey.BossFX_Ice,new Dictionary<Enum, UnityEngine.Object> {  {BossFX_Ice.Ice1Fan, Resources.Load("BossFX/Ice/Ice1Fan") },
                                                                                {BossFX_Ice.Ice1OnHand, Resources.Load("BossFX/Ice/Ice1OnHand") },
                                                                                {BossFX_Ice.Ice2IceLance, Resources.Load("BossFX/Ice/Ice2IceLance") },
                                                                                {BossFX_Ice.Ice3FistsExplosion, Resources.Load("BossFX/Ice/Ice3FistsExplosion") },
                                                                                {BossFX_Ice.IceJumpDown, Resources.Load("BossFX/Ice/IceJumpDown") },
                                                                                {BossFX_Ice.IceJumpUp, Resources.Load("BossFX/Ice/IceJumpUp") },
                                                                                {BossFX_Ice.IceTransformOver, Resources.Load("BossFX/Ice/IceTransformOver") },
                                                                                {BossFX_Ice.IceTransformStart, Resources.Load("BossFX/Ice/IceTransformStart") },} },
                { PoolKey.BossFX_Stone,new Dictionary<Enum, UnityEngine.Object> {{BossFX_Stone.Stone1Clap,Resources.Load("BossFX/Stone/Stone1Clap") },
                                                                                {BossFX_Stone.Stone2Throw,Resources.Load("BossFX/Stone/Stone2Throw") },
                                                                                {BossFX_Stone.Stone3Floor,Resources.Load("BossFX/Stone/Stone3Floor") },
                                                                                {BossFX_Stone.StoneTransformOver,Resources.Load("BossFX/Stone/StoneTransformOver") },
                                                                                {BossFX_Stone.StoneTransformStart,Resources.Load("BossFX/Stone/StoneTransformStart") },} },
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
