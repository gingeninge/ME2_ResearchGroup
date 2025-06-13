using System;
using UnityEngine;

public class OreMelt : MonoBehaviour
{
    public bool isIron = false;
    public bool isSteel = false;
    public bool isCopper = false;
    public GameObject IronSword;
    public GameObject SteelSword;
    public GameObject CopperSword;
    public Transform spawn;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SpawnObject() 
    {
        if (isSteel)
        {
            Instantiate(SteelSword, spawn.position, spawn.rotation);
            isSteel = false;
        }

        if (isCopper) 
        {
            Instantiate(CopperSword, spawn.position, spawn.rotation);
            isCopper = false;
        }

        if (isIron) 
        {
            Instantiate(IronSword, spawn.position, spawn.rotation);
            isIron = false;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("IronOre")|| other.gameObject.CompareTag("CopperOre") || other.gameObject.CompareTag("SteelOre"))
        {
            
            if (other.gameObject.CompareTag("CopperOre") || other.gameObject.CompareTag("SteelOre"))
            {
              
               
                if (other.gameObject.CompareTag("SteelOre"))
                {
                ;
                  isSteel = true;
                } 
                else
                {
                    isCopper = true;
                   
             
                }
                
            }
            else
            {
               isIron = true;
                
              
            }
           

        }
        
    }
    
}
