using UnityEngine;

public class VilligerwayPoints : MonoBehaviour
{
    public GameObject[] wayPoints;
    public float Movementspeed = 1;
    public bool isAtCounter;
    public bool hasSword;
    int currentWaypoint = 0;
    float WayPointRadius = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(wayPoints[currentWaypoint].transform.position, transform.position) < WayPointRadius)
        {
            currentWaypoint++;
            if (currentWaypoint >= wayPoints.Length)
            {

                currentWaypoint = 0;

            }
        }

        transform.position = Vector3.MoveTowards(transform.position, wayPoints[currentWaypoint].transform.position, Time.deltaTime * Movementspeed);
    }
}
