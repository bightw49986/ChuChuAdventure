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
    }
}


