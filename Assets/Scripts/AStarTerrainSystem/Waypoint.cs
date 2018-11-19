using System.Collections.Generic;
using UnityEngine;

namespace PathFinding
{
    [SelectionBase]
    public class Waypoint : MonoBehaviour, ILocationData 
    {
        public int AreaID { get; set; }

        public Vector3 Position { get { return GetPosOnPlane(); } set { transform.position = value; } }
        public LayerMask GroundLayers;
        public Vector3 GetPosOnPlane()
        {
            Ray ray = new Ray(gameObject.transform.position + Vector3.up, Vector3.down);
            RaycastHit hit;
            Physics.Raycast(ray, out hit, 10f, GroundLayers);
            return hit.point == Vector3.zero ? transform.position : hit.point;
        }

        public List<Waypoint> Neighbours = new List<Waypoint>();
        public bool Traversable = true;
        public float Cost = 1f;
        public bool IsLink;
        [SerializeField] bool drawNeighbourhood;
        [SerializeField] bool drawPosition;

        void OnDrawGizmos()
        {
            if (drawPosition)
            {
                Gizmos.color = Traversable ? Color.blue : Color.red;
                Gizmos.DrawSphere(transform.position, 0.15f);
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
                        Gizmos.color =  Color.cyan;
                        Gizmos.DrawLine(transform.position, wp.transform.position);
                    }
                }
            }
        }
    }
}


