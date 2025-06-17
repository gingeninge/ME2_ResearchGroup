using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Diagnostics.Contracts;

public class SellingSword : MonoBehaviour
{
    public Button sellButton;
    public CustomerRequest customer;
    public GameObject sword;
    public EconomicSystem ecoSystem;

    void Start()
    {
        sellButton.onClick.AddListener(SellItem);  
    }

    private void SellItem()
    {
        if(customer == null || sword == null)
        {
            return;
        }

        SwordQuality swordComponent = sword.GetComponent<SwordQuality>();
        if(swordComponent == null)
        {
            return ;
        }

        int payment = customer.SwordCheck(swordComponent);
        ecoSystem.AddMoney(payment);

        Debug.Log($"Player now has ${ecoSystem.playerMoney}");
    }
}
