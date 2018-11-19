using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathFinding
{
    public class WaypointManager : MonoBehaviour
    {
        /// <summary>
        /// 這個區域的Waypoint們
        /// </summary>
        public List<Waypoint> m_waypoints = new List<Waypoint>();
        public int AreaID;
        public bool ManualMode = true;
        public LayerMask ObstacleLayer;


    //    void Start()
    //    {
    //        m_waypoints.Clear();
    //        Waypoint[] waypoints = GetComponentsInChildren<Waypoint>();
    //        foreach (var wp in waypoints)
    //        {
    //            wp.AreaID = AreaID;
    //            if (m_waypoints.Contains(wp) == false)
    //            m_waypoints.Add(wp);
    //        }
    //        if (!ManualMode)
    //        DefineNeighbours(waypoints);
    //    }

    //    void Update()
    //    {

    //    }

    //    void DefineNeighbours(Waypoint[] wayPoints)
    //    {
    //        if (!ManualMode)
    //        {
    //            foreach (Waypoint wp in wayPoints)
    //            {
    //                wp.Neighbours.Clear();
    //                List<Waypoint> maybe = new List<Waypoint>(); 
    //                foreach (var node in wayPoints)
    //                {
    //                    if (wp.Equals(node)) continue;
    //                    Vector3 dir = node.Position - wp.Position;
    //                    RaycastHit[] hits = Physics.RaycastAll(wp.Position,dir,10f);
    //                    bool isOK = true;
    //                    if (hits != null)
    //                    {
    //                        foreach (var hit in hits)
    //                        { 
    //                            if (hit.transform.gameObject.layer != LayerMask.NameToLayer("Ground"))
    //                            {
    //                                Debug.Log("跑到這了");
    //                                isOK = false;
    //                            }
    //                        }
    //                    }
    //                    if (isOK)
    //                    {
    //                        if (maybe.Contains(node) == false)
    //                        {
    //                            maybe.Add(node);
    //                        }
    //                    }
    //                }
    //                foreach (Waypoint neighbour in maybe)
    //                {
    //                    float distance = Vector3.Distance(wp.transform.position + Vector3.up * 0.2f, neighbour.transform.position + Vector3.up * 0.2f);
    //                    if (distance <= 8f)
    //                    {
    //                        wp.Neighbours.Add(neighbour);
    //                        if (neighbour.Neighbours.Contains(wp)==false)
    //                        {
    //                            neighbour.Neighbours.Add(wp);
    //                        }
    //                        Debug.Log(wp + " 加入一個鄰居");
    //                    }
    //                }
    //            }
    //        }
    //    }
    }
}


