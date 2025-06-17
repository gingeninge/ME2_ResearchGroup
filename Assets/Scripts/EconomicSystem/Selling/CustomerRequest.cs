using UnityEngine;

public class CustomerRequest : MonoBehaviour
{
    public SwordQuality requestedSword;
    public int price;
    public int penaltyPrice;

    public void MakeRequest()
    {
        string[] sword = { "Iron", "Steel", "Magic" };
        string randomType = sword[Random.Range(0, sword.Length)];
        int qualityRandom = Random.Range(1, 11);

        requestedSword = new SwordQuality { swordType = randomType, quality = qualityRandom };
        Debug.Log($"Swordsman wnats a {requestedSword.swordType} sword with quality >= {requestedSword.quality}");

    }

    public int SwordCheck(SwordQuality madeSword)
    {
        if (madeSword.swordType == requestedSword.swordType && madeSword.quality >= requestedSword.quality)
        {
            Debug.Log($"Thank you noble Blacksmith! You got paid {price}.");
            return price;
        }
        else
        {
            Debug.Log($"This is not my sword! You got paid {penaltyPrice}");
            return penaltyPrice;
        }
    }
}
