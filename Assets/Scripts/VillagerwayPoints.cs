using Unity.VisualScripting;
using UnityEngine;

public class VillagerwayPoints : MonoBehaviour
{
    GameObject[] wayPoints;
    int currentWaypoint = 0;

    public float Movementspeed = 1;
    float WayPointRadius = 1;
    public bool taskCompleted = false;
    bool canMove = true;
    bool waitingForTask = false;

    void Start()
    {
        WayPointManager manager = FindFirstObjectByType<WayPointManager>();
        wayPoints = manager.waypoints; 
    }

    void Update()
    {
        if (!canMove)
        {
            if (waitingForTask && taskCompleted)
            {
                canMove = true;
                waitingForTask = false;
                taskCompleted = false;
            }
            else return;
        }
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
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("StopZone") && !waitingForTask)
        {
            canMove = false;
            waitingForTask = true;
           
        }
    }
}
