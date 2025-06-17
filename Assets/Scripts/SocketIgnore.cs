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

        // Ignore collisions with parent
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

        // Reset Rigidbody
        Rigidbody rb = snappedObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        // Snap and parent safely
        Transform attachTransform = socket.attachTransform;

        // Set position/rotation before parenting
        snappedObject.transform.SetPositionAndRotation(
            attachTransform.position,
            attachTransform.rotation
        );

        // Then parent to the socket to keep it locked in without warping
        snappedObject.transform.SetParent(socket.attachTransform, true);
    }
}