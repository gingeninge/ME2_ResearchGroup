using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class OreMelt : MonoBehaviour
{
    public XRSocketInteractor Socket;
    public bool isIron = false;
    public bool isSteel = false;
    public bool isCopper = false;
    public bool isSword = false;
    public bool isHilt = false;
    public GameObject IronSword;
    public GameObject SteelSword;
    public GameObject CopperSword;
    public GameObject Hilt;
    public Transform spawn;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MoldCheck();
    }
    public void MoldCheck() 
    {
        IXRSelectInteractable selected = Socket.GetOldestInteractableSelected();
        if (selected == null) return;

        GameObject mold = selected.transform.gameObject;
        string tag = mold.tag;
        switch (tag)
        {
            case "SwordMold":
                isSword = true;
                break;
            case "SwordHiltMold":
                isHilt = true;
                break;
            default:
                Debug.LogWarning("Unknown mold tag: " + tag);
                return;
        }
    }
    public void SpawnObject() 
    {
        if (isSteel && isSword)
        {
            Instantiate(SteelSword, spawn.position, spawn.rotation);
            isSteel = false;
            isSword=false;
        }

        if (isCopper && isSword) 
        {
            Instantiate(CopperSword, spawn.position, spawn.rotation);
            isCopper = false;
            isSword=false;
        }

        if (isIron && isSword) 
        {
            Instantiate(IronSword, spawn.position, spawn.rotation);
            isIron = false;
            isSword=false;
        }
        if (isIron && isHilt)
        {
            Instantiate(Hilt, spawn.position, spawn.rotation);
            isIron = false;
            isHilt = false;
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
                    isSteel = true;
                    Destroy(other.gameObject);
                }
                else
                {
                    isCopper = true;
                    Destroy(other.gameObject);
                }   
            }
            else
            {
               isIron = true;
                Destroy(other.gameObject);
            }
        }
    } 
}
