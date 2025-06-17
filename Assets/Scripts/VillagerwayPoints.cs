using UnityEngine;

public class VillagerwayPoints : MonoBehaviour
{
    GameObject[] wayPoints;
    int currentWaypoint = 0;

    public float Movementspeed = 1;
    float WayPointRadius = 1;

    void Start()
    {
        WayPointManager manager = FindFirstObjectByType<WayPointManager>();
        wayPoints = manager.waypoints; 
    }

    void Update()
    {
        if (wayPoints == null || wayPoints.Length == 0) return;

        GameObject target = wayPoints[currentWaypoint];
        if (target == null) return;

        if (Vector3.Distance(transform.position, target.transform.position) < WayPointRadius)
        {
            currentWaypoint++;
            if (currentWaypoint >= wayPoints.Length)
            {
                currentWaypoint = 0;
            }
        }

        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, Time.deltaTime * Movementspeed);
    }
}
