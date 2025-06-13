using UnityEngine;
using UnityEngine.Events;

public class LeverAction : MonoBehaviour
{
    public HingeJoint hingeJoint;
    public float activationAngle = 45f;
    public float resetThreshold = 10f; // when the lever is nearly upright again
    public UnityEvent onLeverPulled;

    private bool hasActivated = false;

    void Start()
    {
        // Optional: configure motor on startup
        JointMotor motor = hingeJoint.motor;
        motor.force = 100;
        motor.targetVelocity = 0;
        hingeJoint.motor = motor;
        hingeJoint.useMotor = false;
    }

    void Update()
    {
        float currentAngle = Mathf.Abs(hingeJoint.angle);

        // Activate event if pulled far enough and hasn't already activated
        if (!hasActivated && currentAngle >= activationAngle)
        {
            hasActivated = true;
            onLeverPulled.Invoke();

            // Start motor to return to upright
            JointMotor motor = hingeJoint.motor;
            motor.targetVelocity = -50f * Mathf.Sign(hingeJoint.angle); // move back toward 0
            hingeJoint.motor = motor;
            hingeJoint.useMotor = true;
        }

        // Reset once it's nearly upright again
        if (hasActivated && currentAngle <= resetThreshold)
        {
            hasActivated = false;
            hingeJoint.useMotor = false;
        }
    }
}
