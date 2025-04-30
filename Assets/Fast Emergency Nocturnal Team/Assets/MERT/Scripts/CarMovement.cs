using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    [SerializeField] WheelCollider frontRightWheel;
    [SerializeField] WheelCollider frontLeftWheel;
    [SerializeField] WheelCollider backRightWheel;
    [SerializeField] WheelCollider backLeftWheel;

    public float acceleration = 500f;
    public float brakingForce = 300f;
    public float maxTurnAngle = 15f;

    private float currentAcceleration = 0f;
    private float currentBrakingForce = 0f;
    private float currentTurnAngle = 0f;

    private void FixedUpdate()
    {
        int vert = 0;
        int horiz = 0;
        if (Input.GetKey(KeyCode.W)) {
            vert = 1;
        }
        if (Input.GetKey(KeyCode.S)) {
            vert = -1;
        }
        if (Input.GetKey(KeyCode.D)) {
            horiz = 1;
        }
        if (Input.GetKey(KeyCode.A)) {
            horiz = -1;
        }
        //currentAcceleration = -acceleration * Input.GetAxis("Vertical");
        currentAcceleration = -acceleration * vert;
        if (Input.GetKey(KeyCode.Space))
        {
            currentBrakingForce = brakingForce;
        }
        else currentBrakingForce = 0;

        frontRightWheel.motorTorque = currentAcceleration;
        frontLeftWheel.motorTorque = currentAcceleration;

        frontRightWheel.brakeTorque = currentBrakingForce;
        frontLeftWheel.brakeTorque = currentBrakingForce;
        backLeftWheel.brakeTorque = currentBrakingForce;
        backRightWheel.brakeTorque = currentBrakingForce;


        //currentTurnAngle = maxTurnAngle * Input.GetAxis("Horizontal");
        currentTurnAngle = maxTurnAngle * horiz;
        frontLeftWheel.steerAngle = currentTurnAngle;
        frontRightWheel.steerAngle = currentTurnAngle;
    }
}
