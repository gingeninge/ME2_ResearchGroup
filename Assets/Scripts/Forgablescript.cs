using System.Collections;
using UnityEngine;

public class Forgablescript : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public Material defaultMaterial;
    public Material highlightMaterial;
    public AudioSource audioSource;
    public AudioClip successClip;
    public AudioClip failClip;

    private bool isHittable = false;

    public void StartQTE(float totalDuration, float minInterval, float maxInterval)
    {
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

        Debug.Log($"{gameObject.name}: QTE finished");
    }

    public bool IsHittable()
    {
        return isHittable;
    }

    public void OnHammerHit()
    {
        if (IsHittable())
        {
            Debug.Log("Success hit on sword!");
            audioSource.PlayOneShot(successClip);
        }
        else
        {
            Debug.Log("Failed hit on sword.");
            audioSource.PlayOneShot(failClip);
        }
    }
}
