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
    public class AStarNode
    {
        /// <summary>
        /// GameObject of this node.
        /// </summary>
        public GameObject GameObject;

        /// <summary>
        /// Position of this node.
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// The waypoint this node is from.
        /// </summary>
        public Waypoint WP;

        /// <summary>
        /// AreaID of this node.
        /// </summary>
        public int AreaID;

        /// <summary>
        /// return if this node is a link between two area.
        /// </summary>
        public bool IsLink;

        /// <summary>
        /// The neighbours of this node.
        /// </summary>
        public List<AStarNode> Neighbours = new List<AStarNode>();

        /// <summary>
        /// The neighbours name of this node. Used to load waypoint data so don't manually modifidy this variable.
        /// </summary>
        public List<string> NeighboursID = new List<string>();

        /// <summary>
        /// The cost rate of this node. The harder the terrain is , the higher this value should be;
        /// </summary>
        public float m_fCostRate = 1f;

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
    public class AStarAgent
    {
        #region Properties
        public enum WaypointMode { Individual = 0, GridNodes = 1 }
        WaypointMode m_AgentMode;
        List<AStarNode> m_nodes;
        Dictionary<int, GridGenerater> m_Maps;

        #endregion

        #region Constructors
        public AStarAgent(int iGridID)
        {
            m_AgentMode = WaypointMode.GridNodes;
            LoadWP(iGridID);
        }

        public AStarAgent(List<Waypoint> waypoints)
        {
            m_AgentMode = WaypointMode.Individual;
            LoadWP(waypoints);
        }
        #endregion

        #region Waypoint Loading

        void LoadWP(int iGridID)
        {
            GameObject[] grids = GameObject.FindGameObjectsWithTag("GridMap");
            foreach (var grid in grids)
            {
                GridGenerater map = grid.GetComponent<GridGenerater>();
                if (m_Maps.ContainsKey(map.GridID)==false)
                m_Maps.Add(map.GridID, map);
            }
            foreach (Waypoint wp in m_Maps[iGridID].m_grid)
            {
                AStarNode node = new AStarNode
                {
                    GameObject = wp.gameObject,
                    Position = wp.transform.position,
                    AreaID = wp.AreaID,
                    IsLink = wp.IsLink,
                    NeighboursID = wp.NeighboursID,
                    m_fCostRate = wp.Traversable ? (wp.HardToTravel ? 1.1f : 1f) : 10f
                };
                m_nodes.Add(node);
            }
            SetNodeNeighbours();
        }

        void LoadWP(List<Waypoint> waypoints)
        {
            foreach (var wp in waypoints)
            {
                AStarNode node = new AStarNode
                {
                    GameObject = wp.gameObject,
                    Position = wp.transform.position,
                    AreaID = wp.AreaID,
                    IsLink = wp.IsLink,
                    NeighboursID = wp.NeighboursID,
                    m_fCostRate = wp.Traversable ? (wp.HardToTravel ? 1.1f : 1f) : 10f
                };
                m_nodes.Add(node);
            }
            SetNodeNeighbours();
        }

        void SetNodeNeighbours()
        {
            foreach (AStarNode node in m_nodes)
            {
                foreach (string id in node.NeighboursID)
                {
                    GameObject neiGO = GameObject.Find(id);
                    foreach (AStarNode n in m_nodes)
                    {
                        if (n.GameObject == neiGO && node.Neighbours.Contains(n) == false)
                        {
                            node.Neighbours.Add(n);
                        }
                    }
                }
            }
        }
        #endregion

        #region Other Methods

        internal void ClearAStarData()
        {
            for (int i = 0; i < m_nodes.Count; i++)
            {
                m_nodes[i].m_searchFrom = null;
                m_nodes[i].m_fCFS = 0;
                m_nodes[i].m_fCTG = 0;
                m_nodes[i].m_fTTC = 0;
                m_nodes[i].m_NodeState = AStarNode.NodeState.none;
            }
        }

        internal AStarNode FindNearestNode(Vector3 Pos, WaypointMode mode)
        {
            switch (mode)
            {
                case WaypointMode.Individual:
                    {
                        return new AStarNode();
                    }
                case WaypointMode.GridNodes: //fixme 效能issue
                    {
                        for (int i = 0; i <m_Maps.Count ; i++)
                        {
                            Waypoint waypoint =  m_Maps[i].GetWPFormWorldPos(Pos);
                            foreach(var node in m_nodes)
                            {
                                if(node.WP == waypoint)
                                {
                                    return node;
                                }
                            }
                        }
                        return null;
                    }
            }
            return null;
        }
        #endregion

        #region A Star 
        public List<Vector3> GetPath(Vector3 startPos, Vector3 goalPos)
        {
            ClearAStarData();
            AStarNode startNode = FindNearestNode(startPos, m_AgentMode);
            AStarNode goalNode = FindNearestNode(goalPos, m_AgentMode);
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
                        float newCost = neighbour.m_fCostRate * neighbour.m_fCFS + Vector3.Distance(currentNode.Position, neighbour.Position);
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

