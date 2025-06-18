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
            SellingEconomicSystem sellSystem = FindAnyObjectByType<SellingEconomicSystem>();
            if (sellSystem != null)
            {
                sellSystem.RegisterActiveVillager(this); 
            }

            canMove = false;
            waitingForTask = true;
        }
        if (other.CompareTag("KillZone")) 
        {
            Destroy(this.gameObject);
        }
    }
}
