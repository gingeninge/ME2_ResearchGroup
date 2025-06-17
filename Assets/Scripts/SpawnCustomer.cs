using UnityEngine;

public class SpawnCustomer : MonoBehaviour
{
    public GameObject[] Customers;
    public Transform spawnPos;
    public bool InScene;

    // Update is called once per frame
    void Update()
    {
        GameObject tags = GameObject.FindGameObjectWithTag("Customer");
        if(tags != null) 
        {
            InScene = false;
        }
        else 
        {
            InScene = true;
        }
        if (InScene) 
        {
            SpawnCustomers();
        } 
    }
    public void SpawnCustomers() 
    {
        int randomIndexc = Random.Range(0, Customers.Length);
        Instantiate(Customers[randomIndexc], spawnPos.position, spawnPos.rotation);
    }
}
