using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class HammerQte : MonoBehaviour
{
    // A reference for context (for example, the sword's parent).
    public Transform swordBlade;

    // QTE timing settings:
    // perfectHitTime: Ideal time (in seconds) after heating when the hit is perfect.
    // allowedErrorMargin: Acceptable deviation (in seconds) for a successful hit.
    public float perfectHitTime = 2.0f;
    public float allowedErrorMargin = 0.5f;

    // Timer tracking elapsed time since the sword was heated.
    private float timeSinceHeated = 0f;

    // Audio feedback sources (assign these in the Inspector).
    public AudioSource successSound;
    public AudioSource failSound;

    // Particle effect (e.g., sparks) to play on a perfect hit.
    public ParticleSystem perfectSparkEffect;

    // XR component on the hammer (this script is attached to the hammer).
    private XRGrabInteractable grabInteractable;

    // --- Hit Indicator Setup ---
    // Array of Transforms that designate the ideal hit positions on the sword blade.
    // For example: element 0 = tip, 1 = middle, 2 = end.
    public Transform[] indicatorTargets;

    // Prefab for the hit indicator (e.g., a glowing ring).
    public GameObject hitIndicatorPrefab;

    // Hold the instantiated hit indicator GameObjects.
    private GameObject[] hitIndicators;

    // --- Haptic Feedback Settings for Successful Hits (per zone) ---
    // Arrays should have one value per indicator target.
    // For example, tip = 0.7 (amplitude), 0.2 sec (duration); middle = 0.5, 0.15 sec; end = 0.3, 0.1 sec.
    public float[] hapticAmplitudes;
    public float[] hapticDurations;

    // Haptic settings for a failed (mistimed) hit.
    public float failHapticAmplitude = 0.2f;
    public float failHapticDuration = 0.1f;

    void Start()
    {
        // Get the XRGrabInteractable component from the hammer.
        grabInteractable = GetComponent<XRGrabInteractable>();

        // Initialize the QTE timer.
        ResetTimer();

        // Instantiate hit indicators at each designated target.
        if (indicatorTargets != null && indicatorTargets.Length > 0 && hitIndicatorPrefab != null)
        {
            hitIndicators = new GameObject[indicatorTargets.Length];
            for (int i = 0; i < indicatorTargets.Length; i++)
            {
                hitIndicators[i] = Instantiate(hitIndicatorPrefab, indicatorTargets[i].position, Quaternion.identity);
                // Parent the indicator to the target so it follows the sword.
                hitIndicators[i].transform.SetParent(indicatorTargets[i]);
                // Start inactive; they will be activated in each QTE cycle.
                hitIndicators[i].SetActive(false);
            }
        }
        else
        {
            Debug.LogWarning("Indicator Targets or Hit Indicator Prefab not assigned.");
        }

        // Initialize the haptic arrays if not assigned or lengths don’t match.
        if (hapticAmplitudes == null || hapticAmplitudes.Length != indicatorTargets.Length)
        {
            hapticAmplitudes = new float[indicatorTargets.Length];
            for (int i = 0; i < indicatorTargets.Length; i++)
            {
                if (i == 0)
                    hapticAmplitudes[i] = 0.7f;   // Tip: strongest.
                else if (i == 1)
                    hapticAmplitudes[i] = 0.5f;   // Middle: medium.
                else
                    hapticAmplitudes[i] = 0.3f;   // End: weakest.
            }
        }
        if (hapticDurations == null || hapticDurations.Length != indicatorTargets.Length)
        {
            hapticDurations = new float[indicatorTargets.Length];
            for (int i = 0; i < indicatorTargets.Length; i++)
            {
                if (i == 0)
                    hapticDurations[i] = 0.2f;
                else if (i == 1)
                    hapticDurations[i] = 0.15f;
                else
                    hapticDurations[i] = 0.1f;
            }
        }
    }

    void Update()
    {
        // Increment the timer.
        timeSinceHeated += Time.deltaTime;

        // In case the sword moves, update each hit indicator’s position.
        if (hitIndicators != null)
        {
            for (int i = 0; i < hitIndicators.Length; i++)
            {
                if (hitIndicators[i] != null && indicatorTargets[i] != null)
                    hitIndicators[i].transform.position = indicatorTargets[i].position;
            }
        }
    }

    // When the hammer collides with another collider…
    private void OnCollisionEnter(Collision collision)
    {
        // ...process the hit only if the collided object is tagged as "Blade".
        if (collision.gameObject.CompareTag("Blade"))
        {
            OnHammerHit(collision);
        }
    }

    /// <summary>
    /// Processes the hammer hit by checking timing and determining which zone was struck.
    /// </summary>

    public void OnHammerHit(Collision collision)
    {
        // Check whether the hit falls within the acceptable timing window.
        bool isStrikeTimedCorrectly = Mathf.Abs(timeSinceHeated - perfectHitTime) < allowedErrorMargin;

        // Determine the nearest hit zone based on the first contact point.
        int zone = GetClosestIndicatorZone(collision);

        if (isStrikeTimedCorrectly)
            ForgeBlade(zone);
        else
            ShowMistakeEffect(zone);

        // Reset timer for the next QTE attempt.
        ResetTimer();
    }

    /// <summary>
    /// Determines which indicator target (hit zone) is closest to the first collision contact point.
    /// </summary>
    /// <returns>The index of the closest zone (defaults to 0 if not determinable).</returns>
    private int GetClosestIndicatorZone(Collision collision)
    {
        if (collision.contacts.Length == 0 || indicatorTargets == null || indicatorTargets.Length == 0)
            return 0; // Default.

        Vector3 hitPoint = collision.contacts[0].point;
        float minDistance = float.MaxValue;
        int zone = 0;
        for (int i = 0; i < indicatorTargets.Length; i++)
        {
            float dist = Vector3.Distance(hitPoint, indicatorTargets[i].position);
            if (dist < minDistance)
            {
                minDistance = dist;
                zone = i;
            }
        }
        return zone;
    }

    /// <summary>
    /// Called on a perfect hit. Plays success audio, particle effects,
    /// hides hit indicators, and sends zone-specific haptic feedback.
    /// </summary>

    void ForgeBlade(int zone)
    {
        Debug.Log("Perfect hit on the sword blade, zone: " + zone);
        if (successSound != null)
            successSound.Play();
        if (perfectSparkEffect != null)
        {
            perfectSparkEffect.Stop();
            perfectSparkEffect.Play();
        }
        HideHitIndicators();
        SendHapticFeedbackForZone(zone);
    }

    /// <summary>
    /// Called on a mistimed hit. Plays failure audio and sends generic haptic feedback.
    /// </summary>

    void ShowMistakeEffect(int zone)
    {
        Debug.Log("Missed the hit on the sword blade, zone: " + zone);
        if (failSound != null)
            failSound.Play();
        SendHapticFeedbackForFailure();
    }

    /// <summary>
    /// Resets the QTE timer and activates all hit indicators for the next attempt.
    /// </summary>
    public void ResetTimer()
    {
        timeSinceHeated = 0f;
        ShowHitIndicators();
    }

    /// <summary>
    /// Activates all hit indicators.
    /// </summary>
    public void ShowHitIndicators()
    {
        if (hitIndicators != null)
        {
            foreach (GameObject indicator in hitIndicators)
            {
                if (indicator != null)
                    indicator.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Deactivates all hit indicators.
    /// </summary>
    public void HideHitIndicators()
    {
        if (hitIndicators != null)
        {
            foreach (GameObject indicator in hitIndicators)
            {
                if (indicator != null)
                    indicator.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Sends a haptic impulse based on the hit zone.
    /// To avoid any obsolete properties, we first get the currently selecting interactor,
    /// cast it to a MonoBehaviour to obtain its world position, and then search for the closest InputDevice.
    /// </summary>

    private void SendHapticFeedbackForZone(int zone)
    {
        // Obtain the currently selecting interactor.
        IXRSelectInteractor interactor = grabInteractable.GetOldestInteractorSelecting();
        if (interactor != null)
        {
            MonoBehaviour interactorMB = interactor as MonoBehaviour;
            if (interactorMB != null)
            {
                Vector3 interactorPos = interactorMB.transform.position;
                InputDevice device = GetClosestInputDevice(interactorPos);
                if (device.isValid)
                {
                    float amplitude = (zone < hapticAmplitudes.Length) ? hapticAmplitudes[zone] : 0.5f;
                    float duration = (zone < hapticDurations.Length) ? hapticDurations[zone] : 0.2f;
                    device.SendHapticImpulse(0, amplitude, duration);
                }
            }
        }
    }

    /// <summary>
    /// Sends a generic haptic impulse for a failed hit.
    /// </summary>
    private void SendHapticFeedbackForFailure()
    {
        IXRSelectInteractor interactor = grabInteractable.GetOldestInteractorSelecting();
        if (interactor != null)
        {
            MonoBehaviour interactorMB = interactor as MonoBehaviour;
            if (interactorMB != null)
            {
                Vector3 interactorPos = interactorMB.transform.position;
                InputDevice device = GetClosestInputDevice(interactorPos);
                if (device.isValid)
                {
                    device.SendHapticImpulse(0, failHapticAmplitude, failHapticDuration);
                }
            }
        }
    }

    /// <summary>
    /// Finds the InputDevice (with HeldInHand and Controller characteristics) whose reported
    /// position (via CommonUsages.devicePosition) is closest to a given position.
    /// </summary>

    private InputDevice GetClosestInputDevice(Vector3 position)
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDeviceCharacteristics characteristics = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Controller;
        InputDevices.GetDevicesWithCharacteristics(characteristics, devices);

        InputDevice bestDevice = default(InputDevice);
        float bestDistance = float.MaxValue;

        foreach (var device in devices)
        {
            Vector3 devicePos;
            if (device.TryGetFeatureValue(CommonUsages.devicePosition, out devicePos))
            {
                float distance = Vector3.Distance(position, devicePos);
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestDevice = device;
                }
            }
        }
        return bestDevice;
    }
}

