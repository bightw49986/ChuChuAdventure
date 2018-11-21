using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace PathFinding
{
    #region Node

    /// <summary>
    /// Basic structure for A* node
    /// </summary>
    public class AStarNode : ILocationData
    {
        /// <summary>
        /// 節點的GameObject
        /// </summary>
        public GameObject gameObject;

        /// <summary>
        /// 節點的位置
        /// </summary>
        public Vector3 Position { get;set; }

        /// <summary>
        /// 這個節點的原生航點
        /// </summary>
        public Waypoint WP;

        /// <summary>
        /// 這個節點的AreaID
        /// </summary>
        public int AreaID { get; set; }

        /// <summary>
        /// 這個節點是不是連接點
        /// </summary>
        public bool IsLink;

        /// <summary>
        /// 這個節點的鄰居節點
        /// </summary>
        public List<AStarNode> Neighbours = new List<AStarNode>();

        /// <summary>
        /// 這個節點的旅行成本
        /// </summary>
        public float m_Cost = 1f;

        /// <summary>
        /// 從起點累積的成本
        /// </summary>
        public float m_fCFS;

        /// <summary>
        /// 到目標所需的成本
        /// </summary>
        public float m_fCTG;

        /// <summary>
        /// 總計旅行成本
        /// </summary>
        public float m_fTTC;

        /// <summary>
        /// 節點在Astar過程中會經過的狀態
        /// </summary>
        public enum NodeState
        {
            none, open, closed
        }
        /// <summary>
        /// 節點的當前狀態
        /// </summary>
        public NodeState m_NodeState = NodeState.none;
        /// <summary>
        /// 節點目前從哪個鄰居搜尋過來
        /// </summary>
        public AStarNode m_searchFrom;

    }
    #endregion

    #region Astar Agent
    public class AStarAgent : MonoBehaviour, ILocationData
    {
        #region Properties
        /// <summary>
        /// 這個場景的所有航點
        /// </summary>
        Dictionary<int, List<AStarNode>> m_nodes;
        /// <summary>
        /// 這個物件目前的區域ID
        /// </summary>
        public int AreaID { get { return _areaID; } set { _areaID = value; } }
        [SerializeField] int _areaID;
        /// <summary>
        /// 這個物件在地形上的位置
        /// </summary>
        /// <value>The position.</value>
        public Vector3 Position { get { return GetPosOnPlane(); } set { transform.position = value; } }
        /// <summary>
        /// 地板的Layer
        /// </summary>
        public LayerMask GroundLayers;
        /// <summary>
        /// 物件的高度
        /// </summary>
        public float Height = 2f;
        /// <summary>
        /// 地板容許的誤差值，應該差不多等於跳躍高度
        /// </summary>
        public float GroundOffset = 3f;
        /// <summary>
        /// 每個區域的WaypointManager
        /// </summary>
        WaypointManager[] waypointManagers;

        /// <summary>
        ///  算出所在的地板位置
        /// </summary>
        /// <returns>The position on plane.</returns>
        public Vector3 GetPosOnPlane()
        {
            Ray ray = new Ray(gameObject.transform.position + Vector3.up * Height, Vector3.down); //往上拉兩單位
            RaycastHit hit;
            Physics.Raycast(ray, out hit, GroundOffset, GroundLayers);//往下打5單位
            return hit.point == Vector3.zero ? transform.position : hit.point; //回傳有打到：地板位置，沒打到：transform位置
        }
        #endregion

        void Awake()
        {
            m_nodes = new Dictionary<int, List<AStarNode>>(); //把航點的dictionary初始化
            LoadWP(); //讀取航點
        }

        void Start()
        {
            
        }

        void Update()
        {

        }

        #region Waypoint Loading

        /// <summary>
        /// 讀取場景中所有航點
        /// </summary>
        void LoadWP()
        {
            waypointManagers = FindObjectsOfType<WaypointManager>();

            foreach (var area in waypointManagers)
            {
                if (m_nodes.ContainsKey(area.AreaID) == false)
                {
                    List<AStarNode> nodesToAdd = new List<AStarNode>();
                    for (int i = 0; i < area.m_waypoints.Count;i++)
                    {
                        AStarNode node = new AStarNode
                        {
                            WP = area.m_waypoints[i],
                            gameObject = area.m_waypoints[i].gameObject,
                            Position = area.m_waypoints[i].transform.position,
                            AreaID = area.AreaID,
                            IsLink = area.m_waypoints[i].IsLink
                        };
                        node.m_Cost = area.m_waypoints[i].Cost;
                        nodesToAdd.Add(node);
                    }
                    m_nodes.Add(area.AreaID, nodesToAdd);
                    SetNodeNeighbours(nodesToAdd);
                }
            }


        }
        /// <summary>
        /// 設定清單中的節點鄰居關係
        /// </summary>
        /// <param name="nodes">Nodes.</param>
        void SetNodeNeighbours(List<AStarNode> nodes)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                foreach(var node in nodes)
                {
                    if (nodes[i] == node) continue;
                    if (nodes[i].WP.Neighbours.Contains(node.WP) && nodes[i].Neighbours.Contains(node)==false)
                    {
                        nodes[i].Neighbours.Add(node);
                    }
                }
            }
        }
        #endregion

        #region Other Methods

        /// <summary>
        /// 清空現在的節點cost資訊
        /// </summary>
        internal void ClearAStarData()
        {
            for (int i = 1; i < m_nodes.Count ; i++)
            {
                foreach (var node in m_nodes[i])
                {
                    node.m_searchFrom = null;
                    node.m_fCFS = 0;
                    node.m_fCTG = 0;
                    node.m_fTTC = 0;
                    node.m_NodeState = AStarNode.NodeState.none;
                }
            }
        }

        /// <summary>
        /// 在目前區域中找到最近的節點
        /// </summary>
        /// <returns>回傳最近的節點</returns>
        /// <param name="vPos">目標位置</param>
        internal AStarNode FindNearestNodeInArea(Vector3 vPos)
        {
            AStarNode nearestNode = null;
            float fMinSqrDis = 10000f;
            foreach (var node in m_nodes[AreaID])
            {
                Vector3 vNodeDir = vPos - node.Position;
                float fNodeSqrDist = Vector3.SqrMagnitude(vNodeDir);
                if(fNodeSqrDist <= Mathf.Epsilon)
                {
                    nearestNode = node;
                    break;
                }
                if (fNodeSqrDist < fMinSqrDis)
                {
                    nearestNode = node;
                    fMinSqrDis = fNodeSqrDist;
                }
            }
            return  nearestNode;
        }

        /// <summary>
        /// 找到場景中最近的節點
        /// </summary>
        /// <returns>回傳最近的節點.</returns>
        /// <param name="location">目標位置</param>
        internal AStarNode FindNearestNode(ILocationData location)
        {
            AStarNode nearestNode = null;
            float fMinSqrDist = 10000f;
            if (location.AreaID != AreaID)
            {
                foreach (var node in m_nodes[location.AreaID])
                {
                    Vector3 vNodeDir = location.Position - node.Position;
                    float fNodeSqrDist = Vector3.SqrMagnitude(vNodeDir);
                    if (fNodeSqrDist <= Mathf.Epsilon)
                    {
                        nearestNode = node;
                        break;
                    }
                    if (fNodeSqrDist < fMinSqrDist)
                    {
                        nearestNode = node;
                        fMinSqrDist = fNodeSqrDist;
                    }
                }
            }
            else
            {
                nearestNode = FindNearestNodeInArea(location.Position);
            }
            print(nearestNode.gameObject.name);
            foreach(var n in nearestNode.Neighbours)
            {
                print(nearestNode.gameObject.name + "有鄰居：" + n.gameObject.name);
            }
            return nearestNode;
        }

        #endregion

        #region A Star
        public List<Vector3> GetPath(ILocationData startLocation,ILocationData goalLocation)
        {
            ClearAStarData();
            AStarNode startNode = FindNearestNode(startLocation);
            AStarNode goalNode = FindNearestNode(goalLocation);
            if (startNode == null)
            {
                Debug.LogWarning("StartNode = null,check your start location.");
                return null;
            }
            if (goalNode == null)
            {
                Debug.LogWarning("GoalNode = null,check your goal location.");
                return null;
            }

            List<Vector3> path = new List<Vector3>();
            List<AStarNode> Open = new List<AStarNode>();
            List<AStarNode> Closed = new List<AStarNode>();

            if (startNode == goalNode)
            {
                path.Add(startLocation.Position);
                path.Add(goalLocation.Position);
                return path;
            }

            startNode.m_fCFS = 0f;
            startNode.m_fCTG = Vector3.Distance(startNode.Position, goalNode.Position);
            startNode.m_fTTC = startNode.m_fCTG;
            startNode.m_searchFrom = null;
            Open.Add(startNode);

            while (Open.Count > 0)
            {
                Open.OrderBy(n => n.m_fTTC);
                AStarNode currentNode = Open[0];
                Open.Remove(Open[0]);
                if (currentNode.Equals(goalNode))
                {
                    path.Add(goalLocation.Position);
                    path.Add(goalNode.Position);
                    AStarNode previous = goalNode.m_searchFrom;
                    while (previous != startNode)
                    {
                        path.Add(previous.Position);
                        previous = previous.m_searchFrom;
                    }
                    path.Add(startNode.Position);
                    path.Add(startLocation.Position);
                    path.Reverse();
                    return path;
                }
                else
                {
                    foreach (var neighbour in currentNode.Neighbours)
                    {
                        float newCost = neighbour.m_Cost * neighbour.m_fCFS + Vector3.Distance(currentNode.Position, neighbour.Position);
                        if ((Open.Contains(neighbour) || Closed.Contains(neighbour)) && (currentNode.m_fCFS <= newCost)) continue;
                        neighbour.m_searchFrom = currentNode;
                        neighbour.m_fCFS = newCost;
                        neighbour.m_fCTG = Vector3.Distance(neighbour.Position, goalNode.Position);
                        neighbour.m_fTTC = neighbour.m_fCFS + neighbour.m_fCTG;
                        if (Closed.Contains(neighbour))
                        {
                            Closed.Remove(neighbour);
                        }
                        if (Open.Contains(neighbour))
                        {
                            continue;
                        }
                        else
                        {
                            Open.Add(neighbour);
                        }
                    }
                }
                Closed.Add(currentNode);
            }
            return path;
        }

        /// <summary>
        /// 用 a star 演算法在區域中取得路徑
        /// </summary>
        /// <returns>回傳位置的清單</returns>
        /// <param name="startPos">開始位置</param>
        /// <param name="goalPos">目標位置</param>
        public List<Vector3> GetPathInArea(Vector3 startPos, Vector3 goalPos)
        {
            ClearAStarData();
            AStarNode startNode = FindNearestNodeInArea(startPos);
            AStarNode goalNode = FindNearestNodeInArea(goalPos);
            if (startNode == null)
            {
                Debug.LogWarning("StartNode = null,check your start location.");
                return null;
            }
            if (goalNode == null)
            {
                Debug.LogWarning("GoalNode = null,check your goal location.");
                return null;
            }

            List<Vector3> path = new List<Vector3>();
            List<AStarNode> Open = new List<AStarNode>();
            List<AStarNode> Closed = new List<AStarNode>();

            if (startNode == goalNode)
            {
                path.Add(startPos);
                path.Add(goalPos);
                return path;
            }

            startNode.m_fCFS = 0f;
            startNode.m_fCTG = Vector3.Distance(startNode.Position, goalNode.Position);
            startNode.m_fTTC = startNode.m_fCTG;
            startNode.m_searchFrom = null;
            Open.Add(startNode);

            while (Open.Count > 0)
            {
                Open.OrderBy(n => n.m_fTTC);
                AStarNode currentNode = Open[0];
                Open.Remove(Open[0]);
                if (currentNode.Equals(goalNode))
                {
                    path.Add(goalPos);
                    path.Add(goalNode.Position);
                    AStarNode previous = goalNode.m_searchFrom;
                    while (previous != startNode)
                    {
                        path.Add(previous.Position);
                        previous = previous.m_searchFrom;
                    }
                    path.Add(startNode.Position);
                    path.Add(startPos);
                    path.Reverse();
                    return path;
                }
                else
                {
                    foreach (var neighbour in currentNode.Neighbours)
                    {
                        float newCost = neighbour.m_Cost * neighbour.m_fCFS + Vector3.Distance(currentNode.Position, neighbour.Position);
                        if ((Open.Contains(neighbour) || Closed.Contains(neighbour)) && (currentNode.m_fCFS <= newCost)) continue;
                        neighbour.m_searchFrom = currentNode;
                        neighbour.m_fCFS = newCost;
                        neighbour.m_fCTG = Vector3.Distance(neighbour.Position, goalNode.Position);
                        neighbour.m_fTTC = neighbour.m_fCFS + neighbour.m_fCTG;
                        if (Closed.Contains(neighbour))
                        {
                            Closed.Remove(neighbour);
                        }
                        if (Open.Contains(neighbour))
                        {
                            continue;
                        }
                        else
                        {
                            Open.Add(neighbour);
                        }
                    }
                }
                Closed.Add(currentNode);
            }
            return path;
        }


        #endregion
    }
    #endregion

}

