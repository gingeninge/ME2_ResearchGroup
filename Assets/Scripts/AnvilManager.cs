using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class AnvilManager : MonoBehaviour
{
  
        public XRSocketInteractor swordSocket;
        public float qteDuration = 15f;
        public float minInterval = 0.3f;
        public float maxInterval = 2f;

        private Forgablescript currentSword;

        public void StartQTEIfSwordInSocket()
        {
            if (swordSocket.hasSelection)
            {
            IXRSelectInteractable interactable = swordSocket.GetOldestInteractableSelected();

            if (interactable != null)
            {
                GameObject swordObj = interactable.transform.gameObject;
                Forgablescript currentSword = swordObj.GetComponent<Forgablescript>();

                if (currentSword != null)
                {
                    currentSword.StartQTE(qteDuration, minInterval, maxInterval);
                }
            }
        }
        }
    
}
