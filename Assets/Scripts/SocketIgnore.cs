using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class AutoIgnoreSocketCollision : MonoBehaviour
{
    private XRSocketInteractor socket;

    [Tooltip("The root object (e.g., Blade) whose colliders should ignore collisions with snapped parts.")]
    public GameObject parentObject;

    void Awake()
    {
        socket = GetComponent<XRSocketInteractor>();
        socket.selectEntered.AddListener(OnObjectSnapped);
    }

    private void OnObjectSnapped(SelectEnterEventArgs args)
    {
        GameObject snappedObject = args.interactableObject.transform.gameObject;

        if (parentObject == null)
        {
            Debug.LogWarning("AutoIgnoreSocketCollision: No parentObject assigned!");
            return;
        }

        // Ignore collisions between parent and snapped object
        Collider[] parentColliders = parentObject.GetComponentsInChildren<Collider>();
        Collider[] snappedColliders = snappedObject.GetComponentsInChildren<Collider>();

        foreach (var parentCol in parentColliders)
        {
            foreach (var snapCol in snappedColliders)
            {
                Physics.IgnoreCollision(parentCol, snapCol);
            }
        }

        // Disable interaction
        XRGrabInteractable grab = snappedObject.GetComponent<XRGrabInteractable>();
        if (grab != null)
        {
            grab.enabled = false;
        }

        // Stop physics movement and make it static
        Rigidbody rb = snappedObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        // Parent and align
        Transform attachTransform = socket.attachTransform;

        snappedObject.transform.SetParent(attachTransform, false);
        snappedObject.transform.localPosition = Vector3.zero;
        snappedObject.transform.localRotation = Quaternion.identity;
        snappedObject.transform.localScale = Vector3.one;

        // Optional: Fix attach point in case you want to re-enable grabbing later
        if (grab != null)
        {
            if (grab.attachTransform == null)
            {
                GameObject newAttach = new GameObject("AutoAttachTransform");
                newAttach.transform.SetPositionAndRotation(attachTransform.position, attachTransform.rotation);
                newAttach.transform.SetParent(snappedObject.transform);
                grab.attachTransform = newAttach.transform;
            }
            else
            {
                grab.attachTransform.position = attachTransform.position;
                grab.attachTransform.rotation = attachTransform.rotation;
            }
        }
    }
}