using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Forgablescript : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public XRGrabInteractable grabInteractable;
    public Material defaultMaterial;
    public Material highlightMaterial;
    public AudioSource audioSource;
    public AudioClip successClip;
    public AudioClip failClip;

    private bool isHittable = false;

    [Header("Interaction Layers")]
    public InteractionLayerMask grabbableLayer;
    public InteractionLayerMask lockedLayer;

    public void StartQTE(float totalDuration, float minInterval, float maxInterval)
    {
        // Lock grabbing
        if (grabInteractable != null)
        {
            grabInteractable.interactionLayers = lockedLayer;
        }

        StartCoroutine(QTECoroutine(totalDuration, minInterval, maxInterval));
    }

    private IEnumerator QTECoroutine(float totalDuration, float minInterval, float maxInterval)
    {
        float elapsed = 0f;

        while (elapsed < totalDuration)
        {
            float waitTime = Random.Range(minInterval, maxInterval);
            if (elapsed + waitTime > totalDuration)
                waitTime = totalDuration - elapsed;

            yield return new WaitForSeconds(waitTime);
            elapsed += waitTime;

            isHittable = true;
            meshRenderer.material = highlightMaterial;

            float flashDuration = Mathf.Min(1f, totalDuration - elapsed);
            yield return new WaitForSeconds(flashDuration);
            elapsed += flashDuration;

            isHittable = false;
            meshRenderer.material = defaultMaterial;
        }

       
        if (grabInteractable != null)
        {
            grabInteractable.interactionLayers = grabbableLayer;
        }

        Debug.Log($"{gameObject.name}: QTE finished and unlocked.");
    }

    public bool IsHittable() => isHittable;

    public void OnHammerHit()
    {
        if (IsHittable())
        {
            audioSource.PlayOneShot(successClip);
        }
        else
        {
            audioSource.PlayOneShot(failClip);
        }
    }
}