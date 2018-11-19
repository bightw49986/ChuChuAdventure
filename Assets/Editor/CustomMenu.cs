using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using PathFinding;

public class CustomMenu : Editor
{
    [MenuItem("Waypoint/RefreshNeighbours")]
    public static void RefreshNeighbours()
    {
        WaypointManager[] managers = FindObjectsOfType<WaypointManager>();
        foreach (var ma in managers)
        {
            ma.m_waypoints.Clear();
            Waypoint[] waypoints = ma.GetComponentsInChildren<Waypoint>();
            foreach (var p in waypoints)
            {
                p.AreaID = ma.AreaID;
                if (ma.m_waypoints.Contains(p)==false)
                {
                    ma.m_waypoints.Add(p);
                }
            }
            foreach (Waypoint wp in ma.m_waypoints)
            {
                wp.Neighbours.Clear();
                List<Waypoint> maybe = new List<Waypoint>();
                foreach (var node in ma.m_waypoints)
                {
                    if (wp.Equals(node)) continue;
                    Vector3 dir = node.Position - wp.Position;
                    RaycastHit[] hits = Physics.RaycastAll(wp.Position, dir, 10f);
                    bool isOK = true;
                    if (hits != null)
                    {
                        foreach (var hit in hits)
                        {
                            if (hit.transform.gameObject.layer != LayerMask.NameToLayer("Ground"))
                            {
                                isOK = false;
                            }
                        }
                    }
                    if (isOK)
                    {
                        if (maybe.Contains(node) == false)
                        {
                            maybe.Add(node);
                        }
                    }
                }
                foreach (Waypoint neighbour in maybe)
                {
                    float distance = Vector3.Distance(wp.transform.position + Vector3.up * 0.2f, neighbour.transform.position + Vector3.up * 0.2f);
                    if (distance <= 8f)
                    {
                        wp.Neighbours.Add(neighbour);
                        if (neighbour.Neighbours.Contains(wp) == false)
                        {
                            neighbour.Neighbours.Add(wp);
                        }
                        Debug.Log(wp + " 加入一個鄰居");
                    }
                }
            }
        }
    }
}
