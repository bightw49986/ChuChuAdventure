using UnityEngine;

public class GridGenerater : MonoBehaviour
{
    public int GridID; //The ID of grid map in the world
    public Waypoint[,] m_grid; //Define the world as a 2D array with nodes 
    [SerializeField] Vector2 m_gridWorldSize; //Use to store the world's size
    [SerializeField] float m_nodeUnit; // The unit scale in this grid
    [SerializeField] LayerMask m_hardToTravel;
    [SerializeField] LayerMask m_untraversable; // If an object is marked with this layer , it will block the node
    [SerializeField] bool m_drawGizmo;
    int gridSizeX; //The amount of grids in x axis
    int gridSizeY; //The amount of grids in y axis
    GameObject waypointPrefab;

    void Start()
    {
        waypointPrefab = (GameObject)Resources.Load("WaypointPrefab");
        gridSizeX = Mathf.RoundToInt(m_gridWorldSize.x / m_nodeUnit);
        gridSizeY = Mathf.RoundToInt(m_gridWorldSize.y / m_nodeUnit);
        CreateGrid();
    }

    void CreateGrid()
    {
        m_grid = new Waypoint[gridSizeX, gridSizeY];
        Vector3 origin = transform.position - (Vector3.right * m_gridWorldSize.x / 2 + Vector3.forward * m_gridWorldSize.y / 2);

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPos = origin + Vector3.right * (x * m_nodeUnit + m_nodeUnit / 2) + Vector3.forward * (y * m_nodeUnit + m_nodeUnit / 2);
                bool traversable = (!Physics.CheckSphere(worldPos, m_nodeUnit / 2, m_untraversable));
                bool hardToTravel = (!Physics.CheckSphere(worldPos, m_nodeUnit / 2, m_hardToTravel));
                m_grid[x, y] = Instantiate(waypointPrefab, transform).GetComponent<Waypoint>();
                m_grid[x, y].name += "(" + x + "," + y + ")";
                m_grid[x, y].transform.position = worldPos;
                m_grid[x, y].Traversable = traversable;
                m_grid[x, y].HardToTravel = traversable && hardToTravel;
                m_grid[x, y].AreaID = GridID;
                if (x==0 || x==gridSizeX || y==0 || y==gridSizeY)
                {
                    m_grid[x, y].IsLink = true;
                }
            }
        }
        SetNeighbourhood();
        print(gridSizeX * gridSizeY + " girds made.");
    }

    void SetNeighbourhood()
    {
        for (int x = 0; x < gridSizeX -1 ; x++)
        {
            for (int y = 0; y < gridSizeY -1 ; y++)
            {
                if (x == 0)
                {
                    if(!m_grid[x, y].Neighbours.Contains(m_grid[x + 1, y]))
                    m_grid[x, y].Neighbours.Add(m_grid[x + 1, y]);
                    continue;
                }
                if (x == gridSizeX)
                {
                    if (!m_grid[x, y].Neighbours.Contains(m_grid[x - 1, y]))
                        m_grid[x, y].Neighbours.Add(m_grid[x - 1, y]);
                    continue;
                }
                else
                {

                        m_grid[x, y].Neighbours.Add(m_grid[x - 1, y]);

                        m_grid[x, y].Neighbours.Add(m_grid[x + 1, y]);
                }

            }
        }
        for (int x = 0; x < gridSizeX-1; x++)
        {
            for (int y = 0; y < gridSizeY-1; y++)
            {
                if (y == 0)
                {
                    if(!m_grid[x, y].Neighbours.Contains(m_grid[x, y + 1]))
                    m_grid[x, y].Neighbours.Add(m_grid[x, y + 1]);
                    continue;
                }

                if (y == gridSizeY)
                {
                    if (!m_grid[x, y].Neighbours.Contains(m_grid[x, y - 1]))
                        m_grid[x, y].Neighbours.Add(m_grid[x, y - 1]);
                    continue;
                }
                else
                {

                        m_grid[x, y].Neighbours.Add(m_grid[x, y - 1]);

                        m_grid[x, y].Neighbours.Add(m_grid[x, y + 1]);
                }

            }
        }
    }

    public Waypoint GetWPFormWorldPos(Vector3 worldPos)
    {
        float proportionX = Mathf.Clamp01((worldPos.x + gridSizeX / 2) / gridSizeX);
        float proportionY = Mathf.Clamp01((worldPos.z + gridSizeY / 2) / gridSizeY);

        int XPos = Mathf.RoundToInt((gridSizeX - 1) * proportionX);
        int YPos = Mathf.RoundToInt((gridSizeY - 1) * proportionY);

        return m_grid[XPos, YPos];
    }


    void OnDrawGizmos()
    {
        if (m_drawGizmo == true)
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(gridSizeX, 0f, gridSizeY));
            if (m_grid != null)
            {
                foreach (Waypoint wayPoint in m_grid)
                {
                    Gizmos.color = (wayPoint.Traversable) ? Color.white : Color.red;
                    Gizmos.DrawCube(wayPoint.transform.position, Vector3.one * (m_nodeUnit - 0.1f));
                }
            }
        }
    }

}
