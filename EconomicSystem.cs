using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class ShopItem
{
    public string itemName;
    public int itemPrice;
    public GameObject prefab;
    public Transform spwanPoint;
    public Button buyButton;

}
public class EconomicSystem : MonoBehaviour
{
    public int playerMoney;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI feedbackText;
    public ShopItem[] shopItems;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateCurrency();
        foreach (ShopItem shopItem in shopItems)
        {
            shopItem.buyButton.onClick.AddListener(() => BuyItem(shopItem));
        }
    }

    private void BuyItem(ShopItem shopItem)
    {
        if (playerMoney >= shopItem.itemPrice)
        {
            playerMoney -= shopItem.itemPrice;
            Instantiate(shopItem.prefab, shopItem.spwanPoint.position,Quaternion.identity);
            feedbackText.text = $"Purchased {shopItem.itemName}";
        }
        else
        {
            feedbackText.text = "No money";
        }
        UpdateCurrency();
    }

    private void UpdateCurrency()
    {
        moneyText.text = $"Money: {playerMoney}";

    }
}
