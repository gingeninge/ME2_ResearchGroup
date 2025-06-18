using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class LeverActivationQTE : MonoBehaviour
{
    public XRSocketInteractor swordSocket;
    public float qteDuration = 15f;
    public float minInterval = 0.3f;
    public float maxInterval = 2f;

    public void OnLeverPulled()
    {
        
        IXRSelectInteractable interactable = swordSocket.GetOldestInteractableSelected();

        if (interactable != null)
        {
            GameObject swordObject = interactable.transform.gameObject;
            Forgablescript sword = swordObject.GetComponent<Forgablescript>();

            if (sword != null)
            {
                sword.StartQTE(qteDuration, minInterval, maxInterval);
                Debug.Log("Started QTE on sword: " + swordObject.name);
            }
            else
            {
                Debug.LogWarning("Socketed object has no ForgableSword script.");
            }
        }
        else
        {
            Debug.Log("No sword is currently in the anvil.");
        }
    }
}
