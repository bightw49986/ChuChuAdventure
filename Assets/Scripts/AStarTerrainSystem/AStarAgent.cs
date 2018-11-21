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
        /// GameObject of this node.
        /// </summary>
        public GameObject gameObject;

        /// <summary>
        /// Position of this node.
        /// </summary>
        public Vector3 Position { get;set; }

        /// <summary>
        /// The waypoint this node is from.
        /// </summary>
        public Waypoint WP;

        /// <summary>
        /// AreaID of this node.
        /// </summary>
        public int AreaID { get; set; }

        /// <summary>
        /// return if this node is a link between two area.
        /// </summary>
        public bool IsLink;

        /// <summary>
        /// The neighbours of this node.
        /// </summary>
        public List<AStarNode> Neighbours = new List<AStarNode>();

        /// <summary>
        /// The cost rate of this node. The harder the terrain is , the higher this value should be;
        /// </summary>
        public float m_Cost = 1f;

        /// <summary>
        /// The cost form start.
        /// </summary>
        public float m_fCFS;

        /// <summary>
        /// The cost to goal.
        /// </summary>
        public float m_fCTG;

        /// <summary>
        /// Total costs.
        /// </summary>
        public float m_fTTC;

        /// <summary>
        /// Node states.
        /// </summary>
        public enum NodeState
        {
            none, open, closed
        }

        /// <summary>
        /// Node state of this node.
        /// </summary>
        public NodeState m_NodeState = NodeState.none;

        /// <summary>
        /// Which node does this node searched from.
        /// </summary>
        public AStarNode m_searchFrom;

    }
    #endregion

    #region Astar Agent
    public class AStarAgent : MonoBehaviour, ILocationData
    {
        #region Properties
        /// <summary>
        /// 這個場景的所有節點
        /// </summary>
        Dictionary<int, List<AStarNode>> m_nodes;
        /// <summary>
        /// 這個場景的所有連接點
        /// </summary>
        Dictionary<int, AStarNode[]> m_links;
        /// <summary>
        /// 這個物件目前的區域ID
        /// </summary>
        public int AreaID { get; set; }
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
            m_nodes = new Dictionary<int, List<AStarNode>>(); //把節點的dictionary初始化
            LoadWP(); //讀取節點
        }

        void Start()
        {
            
        }

        void Update()
        {

        }

        #region Waypoint Loading

        void LoadWP()
        {
            waypointManagers = FindObjectsOfType<WaypointManager>();

            foreach (var area in waypointManagers)
            {
                if (m_nodes.ContainsKey(area.AreaID) == false)
                {
                    AStarNode[] linksToAdd = new AStarNode[2];
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
                        if (node.IsLink)
                        {
                            if (linksToAdd[0] == null) linksToAdd[0] = node;
                            else if (linksToAdd[1] == null) linksToAdd[1] = node;
                        }
                        node.m_Cost = area.m_waypoints[i].Cost;
                        nodesToAdd.Add(node);
                    }
                    m_nodes.Add(area.AreaID, nodesToAdd);
                    m_links.Add(area.AreaID, linksToAdd);
                    SetNodeNeighbours(nodesToAdd);
                }
            }


        }

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

        internal void ClearAStarData()
        {
            for (int i = 0; i < m_nodes.Count - 1 ; i++)
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

        internal AStarNode FindNearestNode(Vector3 vPos)
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

        internal AStarNode FindNearestNode(ILocationData location)
        {
            AStarNode nearestNode = null;
            float fMinSqrDist = 10000f;
            if (location.AreaID != AreaID)
            {
                foreach (var link in m_links[AreaID])
                {
                    Vector3 vLinkDir = location.Position - link.Position;
                    float fLinkSqrDist = Vector3.SqrMagnitude(vLinkDir);
                    if (fLinkSqrDist <= Mathf.Epsilon)
                    {
                        nearestNode = link;
                        break;
                    }
                    if (fLinkSqrDist < fMinSqrDist)
                    {
                        nearestNode = link;
                        fMinSqrDist = fLinkSqrDist;
                    }
                }
            }
            else
            {
                nearestNode = FindNearestNode(location.Position);
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

        public List<Vector3> GetPath(Vector3 startPos, Vector3 goalPos)
        {
            ClearAStarData();
            AStarNode startNode = FindNearestNode(startPos);
            AStarNode goalNode = FindNearestNode(goalPos);
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

