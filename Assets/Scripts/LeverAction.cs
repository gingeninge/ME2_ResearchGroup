using UnityEngine;
using UnityEngine.Events;

public class LeverAction : MonoBehaviour
{
    public HingeJoint hingeJoint;
    public UnityEvent onLeverPulled;
    public AudioSource sound;

    [Header("Lever Settings")]
    public float pullAngleThreshold = -60f;    
    public float resetThreshold = 10f;        
    public float returnSpeed = 100f;         

    private bool wasPulledUp = false;

    void Start()
    {
        hingeJoint.useMotor = false;
    }

    void Update()
    {
        float currentAngle = hingeJoint.angle;

        // 🔹 Detect full upward pull
        if (!wasPulledUp && currentAngle <= pullAngleThreshold)
        {
            wasPulledUp = true;

            // Start return motor
            JointMotor motor = hingeJoint.motor;
            motor.force = 500f;
            motor.targetVelocity = returnSpeed; // moves back toward 0°
            hingeJoint.motor = motor;
            hingeJoint.useMotor = true;
        }

        // 🔸 Fire the event once it returns to upright
        if (wasPulledUp && Mathf.Abs(currentAngle) <= resetThreshold)
        {
            onLeverPulled.Invoke();
            wasPulledUp = false;
            hingeJoint.useMotor = false;
            sound.Play();
        }
    }
}
