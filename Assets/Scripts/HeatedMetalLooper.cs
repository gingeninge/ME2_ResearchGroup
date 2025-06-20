using UnityEngine;


/// <summary>
/// This script simulates the cooling effect of heated metal by adjusting a materialís emission value over time.
/// </summary>

public class HeatedMetalLooper : MonoBehaviour
{
    // Material using the lit shader graph
    public Material heatedMaterial;

    // Texture when metal is hot
    public Texture2D heatedTexture;

    // Texture when metal is cooled
    public Material cooledMaterial;

    // Duration in seconds for the metal to fully cool
    public float coolingDuration = 5f;

    // Internal timer for cooling effect
    private float coolingTimer;

    // Flag to indicate if cooling is complete
    private bool hasCooled = false;

    // Cached reference to the Renderer component
    private Renderer renderer;

    private void Start()
    {
        // Initialize the cooling timer
        coolingTimer = coolingDuration;

        // Cache the Renderer component to improve performance
        renderer = GetComponent<Renderer>();

        // Check that all required assets are assigned
        if (heatedMaterial != null && heatedTexture != null && cooledMaterial != null)
        {
            // Set the initial texture for the hot metal
            heatedMaterial.SetTexture("_MetalTexture", heatedTexture);
        }
        else
        {
            Debug.LogError("[HeatedMetalLooper] Missing material or texture assignment.");
        }

        // Start fully hot: EmissionLerp = 0 (per the Lerp setup)
        heatedMaterial.SetFloat("_EmissionLerp", 0f);

        // Assign the material to the GameObject's renderer
        if (renderer != null)
        {
            renderer.material = heatedMaterial;
        }
        else
        {
            Debug.LogError("[HeatedMetalLooper] Renderer component is missing.");
        }
    }

    private void Update()
    {
        // If cooling has not finished, decrease the timer
        if (!hasCooled && coolingTimer > 0f)
        {
            coolingTimer -= Time.deltaTime;

            // Calculate the normalized cooling factor (0 = hot, 1 = fully cooled)
            float t = Mathf.Clamp01(1f - (coolingTimer / coolingDuration));

            // Update the shader's EmissionLerp value to visually represent cooling
            heatedMaterial.SetFloat("_EmissionLerp", t);

            // If cooling timer reaches zero, switch texture to the cooled metal
            if (coolingTimer <= 0f)
            {
                GetComponent<Renderer>().material = cooledMaterial;
                Debug.Log("[HeatedMetalLooper] Metal has fully cooled.");
                hasCooled = true;
            }
        }
    }

    // Method to restart the heating process manually
    public void ReheatMetal()
    {
        // Ensure reheating only happens after cooling is complete
        if (hasCooled)
        {
            coolingTimer = coolingDuration;
            hasCooled = false;

            // Set the texture back to the heated version
            heatedMaterial.SetTexture("_MetalTexture", heatedTexture);
            heatedMaterial.SetFloat("_EmissionLerp", 0f);

            Debug.Log("[HeatedMetalLooper] Metal is reheated!");
        }
    }

}
