using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class SellingEconomicSystem : MonoBehaviour
{
    public TMP_Dropdown material;
  
    public TMP_Text customerRequest;
    public TMP_Text moneyText;

    private readonly string[] materials = { "IronMat", "SteelMat", "CopperMat" };
    private string requestedMat;
    private int reward;
    private int playerMoney = 100;
    public int penaltyAmount;
    public VillagerwayPoints otherVillager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RequestDropDown();
    
        GenerateRequest();
        UpdateMoney();
    }

    void RequestDropDown()
    {
        material.ClearOptions();
        material.AddOptions(new System.Collections.Generic.List<string>(materials));
    }

    void GenerateRequest()
    {
        int randomIndex = Random.Range(0, materials.Length);
        requestedMat = materials[randomIndex];
        reward = Random.Range(30, 200);

        customerRequest.text = $"Customer wants: <b>{requestedMat}</b> sword\nReward: <b>${reward}</b>";
    }

     public void SellSword()
    {
        string madeMaterial = material.options[material.value].text;

        if (madeMaterial == requestedMat)
        {
            playerMoney += reward;
            Debug.Log($"Thank you! +${reward}");

        }
        else
        {
            playerMoney -= penaltyAmount;
            Debug.Log($"This is not what I wanted. -${penaltyAmount}");
        }

        GenerateRequest();
        UpdateMoney();
        otherVillager.GetComponent<VillagerwayPoints>().taskCompleted = true;
    }

    void UpdateMoney()
    {
        moneyText.text = $"Money : <b>${playerMoney}<b>";
    }
}
