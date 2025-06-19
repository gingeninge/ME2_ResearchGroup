using UnityEngine;

public class HammerHitDetector : MonoBehaviour
{
    public GameObject sparkEffectPrefab; 

    private void OnCollisionEnter(Collision collision)
    {
        Forgablescript sword = collision.gameObject.GetComponent<Forgablescript>();
        if (sword != null)
        {
            sword.OnHammerHit();

          
            ContactPoint contact = collision.contacts[0];
            Vector3 hitPoint = contact.point;
            Quaternion hitRotation = Quaternion.LookRotation(contact.normal);

            GameObject spark = Instantiate(sparkEffectPrefab, hitPoint, hitRotation);
            Destroy(spark, 1.5f); 
        }
    }
}
