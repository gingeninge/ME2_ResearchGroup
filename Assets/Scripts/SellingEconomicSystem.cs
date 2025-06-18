using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

using UnityEngine.UI;


public class SellingEconomicSystem : MonoBehaviour
{
  
    public XRSocketInteractor Socket;


    public TMP_Text customerRequest;
    public TMP_Text moneyText;

   
    private readonly string[] materials = { "Iron", "Steel", "Copper" };
    private string requestedMat;
    private int reward;
    private int playerMoney = 100;
    public int penaltyAmount;
   

    public bool isSteel;
    public bool isIron;
    public bool isCopper;
    private VillagerwayPoints activeVillager;
   
    void Start()
    {
        UpdateMoney();
    }


    private void Update()
    {
        CheckSword();
    }

    void GenerateRequest()
    {

        customerRequest.text = $"Customer wants: <b>{requestedMat}</b> sword\nReward: <b>${reward}</b>";
    }
    public void CheckSword()
    {
        IXRSelectInteractable selected = Socket.GetOldestInteractableSelected();
        if (selected == null) return;


        GameObject sword = selected.transform.gameObject;

        string Mat = sword.tag;
        switch (Mat)
        {


           case "SteelSword":
            isSteel = true;
            break;
        case "CopperSword":
            isCopper = true;
            break;
        case "IronSword":
            isIron = true;
            break;
        default:
            Debug.LogWarning("Unknown mold tag: " + tag);
            return;
        } 
    }
    public void SellSword()
    {
        if (requestedMat == "Iron" && isIron)
        {
            Debug.Log("thanks");
        }
        else {
            Debug.Log("wrong"); 
        }
        if (requestedMat == "Steel" && isSteel)
        {
            Debug.Log("thanks");
        }
        else
        {
            Debug.Log("wrong");
        }
        if (requestedMat == "Copper" && isCopper)
        {
            Debug.Log("thanks");
        }
        else
        {
            Debug.Log("wrong");
        }
        GenerateRequest(); 
        UpdateMoney();
        if (activeVillager != null) 
        {
            activeVillager.taskCompleted = true;
            Debug.Log("Villager task completed.");
            activeVillager = null;
        }

    }
    void UpdateMoney() { 
        moneyText.text = $"Money : <b>${playerMoney}<b>";
    }
public void RegisterActiveVillager(VillagerwayPoints villager)
{
    activeVillager = villager;
}
}
    
