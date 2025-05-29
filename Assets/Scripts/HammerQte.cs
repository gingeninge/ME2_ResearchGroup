using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class HammerQte : MonoBehaviour
{
    // Reference to the metal object be forged
    // This metal object should have a collider and a tag as "Metal"
    public Transform metal;

    // Time Settings
    // PerfectHitTime: The ideal time (in seconds) after heating when the hit is optimal.
    // allowedErrorMargin: The allowed error margin for a perfect hit.
    public float perfectHitTime = 2.0f;
    public float allowedErrorMargin = 0.5f;

    // Timer to track how long the metal has been heated.
    private float timeSinceHeated = 0f;

    // Audio Feedback for hits
    public AudioSource successSound;
    public AudioSource failSound;

    // Particle System for perfect hit spark effect.
    //Assign the spark effect in the inspector.
    public ParticleSystem perfectSparkEffect;

    // For VR interactions: XR Grab Interactable allows the player to grab and swing the hammer.
    private XRGrabInteractable grabInteractable;

    private void Start()
    {
        // Get the XR Grab Interactable component attached to this hammer.
        grabInteractable = GetComponent<XRGrabInteractable>();

        // Reset the timer when the game starts or when the metal is (re)heated.

        ResetTimer();


    }

    private void Update()
    {
        // Increment the timer.We might enable this only when the metal is heated.
        timeSinceHeated += Time.deltaTime;

    }

    /// <summary>
    /// Resets the timer back to zero.
    /// Call this when the metal is heated up for forging.
    /// </summary>

    private void ResetTimer()
    {
        timeSinceHeated = 0f;
    }

    /// <summary>
    /// Detects collisions with the metal object.
    /// When the hammer collides with an object tagged "Metal", process the QTE.
    /// </summary>

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Metal")) 
        {

            OnHammerHit();

        }
    }


    /// <summary>
    /// Processes the hammer hit by checking if the hit timing is within the allowed margin.
    /// </summary>

    private void OnHammerHit()
    {
        // Determine if the player's hit is within the acceptable timing window.
        bool isStrikeTimedCorrectly = Mathf.Abs(timeSinceHeated - perfectHitTime) < allowedErrorMargin;

        if (isStrikeTimedCorrectly)
        {
            ForgeMetal(); // Execute the perfect hit effects.
        }
        else
        {
            ShowMistakeEffect(); // Execute the failure effects.

        }

        // Reset the timer for each new hit attempt.
        ResetTimer();

    }

    /// <summary>
    /// Called when the player hits the metal with correct timing.
    /// Triggers success sound, visual feedback, and plays the sparks particle effect.
    /// </summary>
    private void ForgeMetal()
    {
        Debug.Log("Perfect hit! Metal is shaping correctly.");

        // Play success sound.
        if (successSound != null)
        {
            successSound.Play();
        }

        // Play the sparks particle system effect.
        perfectSparkEffect.Stop();
        perfectSparkEffect.Play();

    }

    // Here we could add additional logic, such as visual changes to the metal or advancing the forging process.


    /// <summary>
    /// Called when the player's hit is mistimed.
    /// Triggers failure sound and any other feedback (e.g., a warp effect on the metal).
    /// </summary>

    private void ShowMistakeEffect()
    {
        Debug.Log("Missed timing! Metal is warping.");

        // Play failure sound.
        if (failSound != null)
        {
            failSound.Play();
        }

        // We could add additional visual or gameplay penalty effects here.

    }
}
