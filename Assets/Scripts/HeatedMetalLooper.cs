using UnityEngine;


/// <summary>
/// This script simulates the cooling effect of heated metal by adjusting a material’s emission value over time.
/// </summary>

public class HeatedMetalLooper : MonoBehaviour
{
    // The heated material using a shader that supports emission control.
    public Material heatedMaterial;

    // The noise texture that will be applied (as a normal map) for additional visual detail on the metal.
    public Texture2D metalTexture;

    // Duration (in seconds) for the metal to fully transition from "hot" to "cooled".
    public float coolingDuration = 5f;

    // Internal timer tracking the cooling process.
    private float coolingTimer;

    // Flag to indicate if the metal has finished cooling.
    private bool hasCooled = false;

    private void Start()
    {
        // Initial setup of the cooling timer.
        coolingTimer = coolingDuration;

        // Ensure that both the heated material and metal texture have been assigned.
        if (heatedMaterial != null && metalTexture != null)
        {

            // Assign the metalTexture to the shader property "_MetalTexture".
            heatedMaterial.SetTexture("_MetalTexture", metalTexture);


        }
        else 
        {

            Debug.LogError("Heated Material or Metal Texture is missing.");


        }

        // Set the initial emission amount.
        // Here, 0 means the metal is fully hot (maximum glow/intensity according to the shader setup).
        heatedMaterial.SetFloat("_EmissionLerp", 0f);

        // Assign the heatedMaterial to the current GameObject’s renderer.
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {

            renderer.material = heatedMaterial;

        }
        else 
        {

            Debug.LogError("[HeatedMetalLooper] Renderer component is missing on the GameObject.");


        }



    }

    private void Update()
    {
        // Only process cooling if the metal hasn't yet finished cooling.
        if (!hasCooled && coolingTimer > 0f)
        {
            // Decrease the timer based on the time that has passed since the last frame.
            coolingTimer -= Time.deltaTime;

            // Calculate the normalized cooling factor.
            // When coolingTimer equals coolingDuration, t = 0 (hot).
            // When coolingTimer is 0, t = 1 (fully cooled).
            float t = Mathf.Clamp01(1f - ( coolingTimer / coolingDuration));

            // Update the shader's "_EmissionLerp" to reflect the cooling progress.
            heatedMaterial.SetFloat("_EmissionLerp", t);

            // Once the timer runs out, the metal is fully cooled.
            if (coolingTimer <= 0f)
            {

                Debug.Log("[HeatedMetalLooper] Metal has fully cooled.");
                hasCooled = true;

            }


        }

    }

    /// <summary>
    /// Resets the cooling process, reheating the metal.
    /// Call this method when the metal is put back into the forge.
    /// </summary>
    public void ReheatMetal() 
    {

        // Reset the cooldown timer.
        coolingTimer = coolingDuration;

        // Metal is now hot again, so mark it as not fully cooled.
        hasCooled = false ;

        // Immediately set the metal appearance to fully hot.
        heatedMaterial.SetFloat("_EmissionLerp", 0f);

        Debug.Log("[HeatedMetalLooper] Metal has been reheated in the forge.");
    }

    // OPTIONAL: If the forge is represented by a trigger collider, we can automatically detect
    // when the metal is placed into the forge by using OnTriggerEnter.

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider belongs to an object tagged "Forge".
        if (other.CompareTag("Forge"))
        {
            // Reheat the metal if it's placed back into the forge.
            ReheatMetal();

        }

    }

}
