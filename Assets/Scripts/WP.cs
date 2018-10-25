using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WP : MonoBehaviour {
    public List<GameObject> m_Neibors;
    public bool bLink = false;
    public int iFloor = 0;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnDrawGizmos()
    {
        if (m_Neibors != null && m_Neibors.Count > 0) {
            foreach (GameObject g in m_Neibors) {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(this.transform.position, g.transform.position);
             }
        }
    }
}
