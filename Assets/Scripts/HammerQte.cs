using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class HammerQte : MonoBehaviour
{
    // QTE timing settings
    public float perfectHitTime = 2.0f;
    public float allowedErrorMargin = 0.5f;

    // Tracks time elapsed since QTE start
    private float timeSinceQTEStart = 0f;

    // Flag to indicate whether QTE is active
    private bool qteActive = false;

    // Audio and particle feedback
    public AudioSource successSound;
    public AudioSource failSound;
    public ParticleSystem perfectSparkEffect;

    // XR component for detecting haptic feedback
    private XRGrabInteractable grabInteractable;

    // --- Hit Indicator Setup ---
    public Transform[] indicatorTargets; // Assign specific zones
    public GameObject hitIndicatorPrefab; // Prefab for visual guidance
    private GameObject[] hitIndicators;

    // Haptic feedback settings
    public float[] hapticAmplitudes;
    public float[] hapticDurations;
    public float failHapticAmplitude = 0.2f;
    public float failHapticDuration = 0.1f;

    // Reference to the ForgeItemQuality system (this script manages item quality based on strike performance)


    void Start()
    {
        // Get the XRGrabInteractable component
        grabInteractable = GetComponent<XRGrabInteractable>();

        // Initialize QTE
        qteActive = false;
        timeSinceQTEStart = 0f;

        // Instantiate hit indicators based on assigned indicator positions
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

        // Ensure haptic arrays match the number of assigned zones
        int numZones = indicatorTargets.Length;
        ValidateHapticArrays(numZones);
    }

    void Update()
    {
        if (qteActive)
        {
            timeSinceQTEStart += Time.deltaTime;
        }

        // Keep hit indicators positioned correctly
        if (hitIndicators != null)
        {
            for (int i = 0; i < hitIndicators.Length; i++)
            {
                if (hitIndicators[i] != null && indicatorTargets[i] != null)
                    hitIndicators[i].transform.position = indicatorTargets[i].position;
            }
        }
    }

    /// <summary> Starts the QTE when the player presses the Start QTE button. </summary>
    public void StartQTE()
    {
        qteActive = true;
        timeSinceQTEStart = 0f;
        ShowHitIndicators();
        Debug.Log("QTE Started!");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!qteActive)
            return;

        if (collision.gameObject.CompareTag("Blade"))
        {
            OnHammerHit(collision);
        }
    }

    /// <summary> Processes the hammer hit and determines strike quality. </summary>
    private void OnHammerHit(Collision collision)
    {
        bool isPerfectHit = Mathf.Abs(timeSinceQTEStart - perfectHitTime) < allowedErrorMargin;

        // Determine hit zone closest to the impact point
        int zone = GetClosestIndicatorZone(collision.contacts[0].point);

        // Register the strike with the ForgeItemQuality system
        // if()
        // {
        //                         // True for perfect strikes, False for misses

        // }
        
        // Play success/failure effects based on strike quality
        if (isPerfectHit)
        {
            successSound?.Play();
            perfectSparkEffect?.Play();
            SendHapticFeedbackForZone(zone);
        }
        else
        {
            failSound?.Play();
            SendHapticFeedbackForFailure();
        }

        qteActive = false; // End the QTE after the strike
    }

    /// <summary> Finds the nearest hit indicator zone based on the collision impact. </summary>
    private int GetClosestIndicatorZone(Vector3 hitPoint)
    {
        if (indicatorTargets.Length == 0)
            return 0;

        float minDistance = float.MaxValue;
        int closestZone = 0;

        for (int i = 0; i < indicatorTargets.Length; i++)
        {
            float dist = Vector3.Distance(hitPoint, indicatorTargets[i].position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closestZone = i;
            }
        }

        return closestZone;
    }

    /// <summary> Ensures haptic feedback arrays have correct length. </summary>
    private void ValidateHapticArrays(int numZones)
    {
        if (hapticAmplitudes == null || hapticAmplitudes.Length != numZones)
        {
            hapticAmplitudes = new float[numZones];
            for (int i = 0; i < numZones; i++)
                hapticAmplitudes[i] = (i == 0) ? 0.7f : (i == 1) ? 0.5f : 0.3f;
        }

        if (hapticDurations == null || hapticDurations.Length != numZones)
        {
            hapticDurations = new float[numZones];
            for (int i = 0; i < numZones; i++)
                hapticDurations[i] = (i == 0) ? 0.2f : (i == 1) ? 0.15f : 0.1f;
        }
    }

    /// <summary> Displays hit indicators at designated zones. </summary>
    private void ShowHitIndicators()
    {
        foreach (var indicator in hitIndicators)
        {
            if (indicator != null)
                indicator.SetActive(true);
        }
    }

    /// <summary> Sends haptic feedback based on the hit zone. </summary>
    private void SendHapticFeedbackForZone(int zone)
    {
        IXRSelectInteractor interactor = grabInteractable.GetOldestInteractorSelecting();
        if (interactor is MonoBehaviour interactorMB)
        {
            Vector3 interactorPos = interactorMB.transform.position;
            InputDevice device = GetClosestInputDevice(interactorPos);
            if (device.isValid)
                device.SendHapticImpulse(0, hapticAmplitudes[zone], hapticDurations[zone]);
        }
    }

    /// <summary> Sends generic haptic feedback for missed strikes. </summary>
    private void SendHapticFeedbackForFailure()
    {
        IXRSelectInteractor interactor = grabInteractable.GetOldestInteractorSelecting();
        if (interactor is MonoBehaviour interactorMB)
        {
            Vector3 interactorPos = interactorMB.transform.position;
            InputDevice device = GetClosestInputDevice(interactorPos);
            if (device.isValid)
                device.SendHapticImpulse(0, failHapticAmplitude, failHapticDuration);
        }
    }

    /// <summary> Finds the closest InputDevice to send haptic feedback. </summary>
    private InputDevice GetClosestInputDevice(Vector3 position)
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDeviceCharacteristics characteristics = InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Controller;
        InputDevices.GetDevicesWithCharacteristics(characteristics, devices);

        InputDevice bestDevice = default;
        float bestDistance = float.MaxValue;
        foreach (var device in devices)
        {
            if (device.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 devicePos))
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


    /// <summary>
    /// Ends the forging process and determines item quality.
    /// Call this method after all strikes have been completed.
    /// </summary>
   





}
