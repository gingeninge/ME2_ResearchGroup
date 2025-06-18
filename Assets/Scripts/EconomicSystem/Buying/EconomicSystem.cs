using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class ShopItem
{
    public string itemName;
    public int itemPrice;
    public GameObject prefab;
    public Transform spawnPoint;
    public Button buyButton;

}
public class EconomicSystem : MonoBehaviour
{
    public int playerMoney;
    public TextMeshProUGUI moneyText;
   
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

    public void BuyItem(ShopItem shopItem)
    {
        if (playerMoney >= shopItem.itemPrice)
        {
            playerMoney -= shopItem.itemPrice;
            Instantiate(shopItem.prefab, shopItem.spawnPoint.position,Quaternion.identity);
          
        }

        UpdateCurrency();
    }

    public void AddMoney(int amount)
    {
        playerMoney += amount;
        UpdateCurrency();
    }

    private void UpdateCurrency()
    {
        moneyText.text = $"Money: {playerMoney}";

    }
    public class ColliderDebug : MonoBehaviour
    {
        void OnTriggerEnter(Collider other)
        {
            Debug.Log($"Trigger entered by: {other.name}");
        }
    }
    
}
