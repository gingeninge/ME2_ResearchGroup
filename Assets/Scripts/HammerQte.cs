using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class HammerQte : MonoBehaviour
{
    // A reference for context (for example, the sword blade's parent object)
    public Transform swordBlade;

    // QTE timing settings.
    public float perfectHitTime = 2.0f;
    public float allowedErrorMargin = 0.5f;

    // Timer tracking elapsed time since QTE start.
    private float timeSinceHeated = 0f;

    // Flag that indicates whether the QTE is active.
    private bool qteActive = false;

    // Audio feedback sources.
    public AudioSource successSound;
    public AudioSource failSound;

    // Particle effect to play on a perfect hit.
    public ParticleSystem perfectSparkEffect;

    // XR component on the hammer (this script is attached to the hammer).
    private XRGrabInteractable grabInteractable;

    // --- Hit Indicator Setup ---
    // If randomizeIndicatorPositions is false, then these indicatorTargets define fixed positions.
    public Transform[] indicatorTargets;

    // When randomizeIndicatorPositions is true, these values determine the random offset along the sword blade.
    public bool randomizeIndicatorPositions = true;
    public float minIndicatorOffset = 0f;
    public float maxIndicatorOffset = 0.5f;

    // The prefab for the hit indicator (for example, a glowing ring).
    public GameObject hitIndicatorPrefab;

    // Array to hold the instantiated hit indicator GameObjects.
    private GameObject[] hitIndicators;

    // --- Haptic Feedback Settings for Successful Hits (per zone) ---
    public float[] hapticAmplitudes; // e.g., tip = 0.7, middle = 0.5, end = 0.3.
    public float[] hapticDurations;  // e.g., tip = 0.2 sec, middle = 0.15 sec, end = 0.1 sec.

    // Haptic settings for a failed (mistimed) hit.
    public float failHapticAmplitude = 0.2f;
    public float failHapticDuration = 0.1f;

    void Start()
    {
        // Get the XRGrabInteractable component.
        grabInteractable = GetComponent<XRGrabInteractable>();

        // QTE is inactive initially.
        qteActive = false;
        ResetTimer();

        // Instantiate hit indicators.
        // If randomization is enabled, we’ll attach them to the swordBlade and ignore preset indicatorTargets.
        if (randomizeIndicatorPositions)
        {
            // Determine the count of indicators from indicatorTargets if provided; otherwise, default to 3.
            int count = (indicatorTargets != null && indicatorTargets.Length > 0) ? indicatorTargets.Length : 3;
            hitIndicators = new GameObject[count];
            for (int i = 0; i < count; i++)
            {
                // Instantiate at the swordBlade’s position.
                hitIndicators[i] = Instantiate(hitIndicatorPrefab, swordBlade.position, Quaternion.identity);
                // Parent to the swordBlade for convenience.
                hitIndicators[i].transform.SetParent(swordBlade);
                hitIndicators[i].SetActive(false);
            }
        }
        else
        {
            // Use preset fixed positions.
            if (indicatorTargets != null && indicatorTargets.Length > 0 && hitIndicatorPrefab != null)
            {
                hitIndicators = new GameObject[indicatorTargets.Length];
                for (int i = 0; i < indicatorTargets.Length; i++)
                {
                    hitIndicators[i] = Instantiate(hitIndicatorPrefab, indicatorTargets[i].position, Quaternion.identity);
                    hitIndicators[i].transform.SetParent(indicatorTargets[i]);
                    hitIndicators[i].SetActive(false);
                }
            }
            else
            {
                Debug.LogWarning("Indicator Targets or Hit Indicator Prefab not assigned.");
            }
        }

        // Initialize haptic arrays if not assigned or if their lengths do not match.
        int indicatorCount = (hitIndicators != null) ? hitIndicators.Length : 0;
        if (hapticAmplitudes == null || hapticAmplitudes.Length != indicatorCount)
        {
            hapticAmplitudes = new float[indicatorCount];
            for (int i = 0; i < indicatorCount; i++)
            {
                if (i == 0)
                    hapticAmplitudes[i] = 0.7f;
                else if (i == 1)
                    hapticAmplitudes[i] = 0.5f;
                else
                    hapticAmplitudes[i] = 0.3f;
            }
        }
        if (hapticDurations == null || hapticDurations.Length != indicatorCount)
        {
            hapticDurations = new float[indicatorCount];
            for (int i = 0; i < indicatorCount; i++)
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
        // Only update the QTE timer if the QTE is active.
        if (qteActive)
        {
            timeSinceHeated += Time.deltaTime;
        }
        // Update the positions of the hit indicators.
        if (hitIndicators != null)
        {
            if (randomizeIndicatorPositions)
            {
                // When randomization is active, positions are re-randomized only at QTE start.
                // (We might want to animate them or leave them fixed during the QTE.)
                //  I'll keep their positions fixed after being randomized at start.
            }
            else
            {
                // For fixed positioning, keep indicators at their preset target positions.
                for (int i = 0; i < hitIndicators.Length; i++)
                {
                    if (hitIndicators[i] != null && indicatorTargets[i] != null)
                        hitIndicators[i].transform.position = indicatorTargets[i].position;
                }
            }
        }
    }

    /// <summary>
    /// Called from Start QTE Button to begin the QTE.
    /// Activates the QTE, resets the timer, and (if randomizing) randomizes indicator positions.
    /// </summary>
    public void StartQTE()
    {
        qteActive = true;
        ResetTimer();
        Debug.Log("QTE Started!");
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Process the hit only if the QTE is active.
        if (!qteActive)
            return;

        if (collision.gameObject.CompareTag("Blade"))
        {
            OnHammerHit(collision);
        }
    }

    /// <summary>
    /// Processes the hammer hit by checking timing and determining the hit zone.
    /// </summary>
    /// <param name="collision">Collision info from the hit.</param>
    public void OnHammerHit(Collision collision)
    {
        bool isStrikeTimedCorrectly = Mathf.Abs(timeSinceHeated - perfectHitTime) < allowedErrorMargin;
        int zone = GetClosestIndicatorZone(collision);

        if (isStrikeTimedCorrectly)
            ForgeBlade(zone);
        else
            ShowMistakeEffect(zone);

        // Stop the QTE after the hit (or you can keep it active for multiple attempts).
        qteActive = false;
    }

    /// <summary>
    /// Determines which indicator is closest to the collision contact point.
    /// If randomizing, the hit indicators' order is arbitrary.
    /// </summary>
   
    private int GetClosestIndicatorZone(Collision collision)
    {
        if (collision.contacts.Length == 0 || hitIndicators == null || hitIndicators.Length == 0)
            return 0;

        Vector3 hitPoint = collision.contacts[0].point;
        float minDistance = float.MaxValue;
        int zone = 0;
        for (int i = 0; i < hitIndicators.Length; i++)
        {
            if (hitIndicators[i] != null)
            {
                float dist = Vector3.Distance(hitPoint, hitIndicators[i].transform.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    zone = i;
                }
            }
        }
        return zone;
    }

    /// <summary>
    /// Called on a perfect hit.
    /// Plays success audio/particle effects, hides hit indicators, and sends haptic feedback for the given zone.
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
    /// Called on a mistimed hit.
    /// Plays failure audio and sends generic haptic feedback.
    /// </summary>
  
    void ShowMistakeEffect(int zone)
    {
        Debug.Log("Missed the hit on the sword blade, zone: " + zone);
        if (failSound != null)
            failSound.Play();
        SendHapticFeedbackForFailure();
    }

    /// <summary>
    /// Resets the QTE timer and shows all hit indicators.
    /// If randomizing positions, positions are randomized here.
    /// </summary>
    public void ResetTimer()
    {
        timeSinceHeated = 0f;
        if (randomizeIndicatorPositions)
            RandomizeIndicatorPositions();
        else
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
    /// Randomizes the positions of the hit indicators along the sword blade.
    /// They will appear at random offsets along the swordBlade's local up direction.
    /// </summary>
    private void RandomizeIndicatorPositions()
    {
        if (hitIndicators != null && swordBlade != null)
        {
            foreach (GameObject indicator in hitIndicators)
            {
                if (indicator != null)
                {
                    float offset = Random.Range(minIndicatorOffset, maxIndicatorOffset);
                    // Set position relative to swordBlade's position along its up vector.
                    indicator.transform.position = swordBlade.position + swordBlade.up * offset;
                }
            }
            ShowHitIndicators();
        }
    }

    /// <summary>
    /// Sends a haptic impulse based on the hit zone.
    /// </summary>
    
    private void SendHapticFeedbackForZone(int zone)
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
                    float amplitude = (zone < hapticAmplitudes.Length) ? hapticAmplitudes[zone] : 0.5f;
                    float duration = (zone < hapticDurations.Length) ? hapticDurations[zone] : 0.2f;
                    device.SendHapticImpulse(0, amplitude, duration);
                }
            }
        }
    }

    /// <summary>
    /// Sends generic haptic impulse for a failed hit.
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
    /// position is closest to the provided reference position.
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
