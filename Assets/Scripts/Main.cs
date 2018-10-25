using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour {

    public static Main m_Instance;

    private List<Obstacle> m_Obstacles;
    public GameObject m_Player;
    private void Awake()
    {
        m_Instance = this;
    }

    // Use this for initialization
    void Start () {
        m_Obstacles = new List<Obstacle>();
        GameObject [] gos = GameObject.FindGameObjectsWithTag("Obstacle");
        if (gos != null || gos.Length > 0)
        {
            Debug.Log(gos.Length);
            foreach (GameObject go in gos)
            {
                m_Obstacles.Add(go.GetComponent<Obstacle>());
            }
        }
    }

    public GameObject GetPlayer()
    {
        return m_Player;
    }

    public List<Obstacle> GetObstacles()
    {
        return m_Obstacles;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
