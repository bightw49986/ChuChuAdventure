using UnityEngine;

namespace PathFinding
{
    [RequireComponent(typeof(Collider))]
    public class AreaIndicater : MonoBehaviour
    {
        public int[] Edge = new int[2];

        void OnTriggerEnter(Collider other)
        {
            ILocationData locationData = other.GetComponent<ILocationData>();
            if (locationData != null)
            {
                if (locationData.AreaID == Edge[0]) locationData.AreaID = Edge[1];
                else if (locationData.AreaID == Edge[1]) locationData.AreaID = Edge[0];
            }
        }
    }

    public interface ILocationData
    {
        Vector3 Position { get; set; }
        int AreaID { get; set; }
    }
}


