using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class SellingEconomicSystem : MonoBehaviour
{
    public TMP_Dropdown material;
    public Button sellButton;
    public TMP_Text customerRequest;
    public TMP_Text moneyText;

    private readonly string[] materials = { "Iron", "Steel", "Gold" };
    private string requestedMat;
    private int reward;
    private int playerMoney = 100;
    public int penaltyAmount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RequestDropDown();
        sellButton.onClick.AddListener(SellSword);
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

    void SellSword()
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
    }

    void UpdateMoney()
    {
        moneyText.text = $"Money : <b>${playerMoney}<b>";
    }
}
