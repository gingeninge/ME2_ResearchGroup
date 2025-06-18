using UnityEngine;

public class HammerHitDetector : MonoBehaviour
{
   
    
        private void OnCollisionEnter(Collision collision)
        {
            Forgablescript sword = collision.gameObject.GetComponent<Forgablescript>();
            if (sword != null)
            {
                sword.OnHammerHit();
            
            }
        }
    
}
