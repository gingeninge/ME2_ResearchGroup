using UnityEngine;

public class GetMoney : MonoBehaviour
{
    public EconomicSystem economicSystem;
    public CustomerRequest customer;
    public GameObject craftedSword;

    public void SellSword()
    {
        if(customer == null)
        {
            Debug.Log("Next customer please!");
            return;
        }
        SwordQuality sword = craftedSword.GetComponent<SwordQuality>();
        int payment = customer.SwordCheck(sword);

        economicSystem.playerMoney += payment;
        economicSystem.AddMoney(payment);

        Debug.Log($"Player now has ${economicSystem.playerMoney}");
    }

    public void SetMadeSword(GameObject swordObj)
    {
        craftedSword = swordObj;
    }
}
