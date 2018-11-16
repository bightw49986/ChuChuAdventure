using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour 
{
    public int AreaID;
    public List<Waypoint> Neighbours = new List<Waypoint>();
    public List<string> NeighboursID;
    public bool Traversable = true;
    public bool HardToTravel = false;
    public bool IsLink;
    [SerializeField] bool drawNeighbourhood;

    void Start()
    {
        SetNeighbourID();
    }

    void SetNeighbourID()
    {
        foreach (var n in Neighbours)
        {
            if (NeighboursID.Contains(n.name) == false)
            {
                NeighboursID.Add(n.name);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (drawNeighbourhood)
        {
            foreach (Waypoint wp in Neighbours)
            {
                if (wp != null)
                {
                    Gizmos.color = wp.Traversable ? Color.green : Color.red;
                    Gizmos.DrawLine(transform.position, wp.transform.position);
                }
            }
        }
    }
}
