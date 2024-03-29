using UnityEngine;

/// <summary>
/// Contains information related to a Wheel (Mesh, it's collider and if it's either for steering or accelerating)
/// </summary>
[System.Serializable]
public class Wheel 
{
	public WheelCollider wheelCollider;
	public GameObject wheelMesh;
	public bool motor;
	public bool steering;

    /// <summary>
    /// Update the wheel's position and rotation according to it's collider
    /// </summary>
    public void UpdateWheel()
    {
        Quaternion rot;
        Vector3 pos;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelMesh.transform.position = pos;
        wheelMesh.transform.rotation = rot;
    }
}

/// <summary>
/// Handles the car's movement
/// </summary>
public class CarController : MonoBehaviour
{
    // Car settings and wheels
	public float maxMotorTorque;
	public float maxSteeringAngle;
	public Wheel[] wheels;
	
	public void Update()
	{
        // Only update if the experiment started
        if (NeuralController.instance.Started)
        {
            // Get the output values motor = 1 100% | 0 0%  / steering -> -1 Left | 1 Right
            float motor = maxMotorTorque * NeuralController.instance.motor;
            float steering = maxSteeringAngle * (NeuralController.instance.steering - 0.5f) * 2;
            float brakeTorque = 0;

            // If the motor is to slow, stop it so we can get to the next one
            if (NeuralController.instance.motor < 0.1f)
            {
                motor = 0;
                brakeTorque = maxMotorTorque;
            }
            else
                brakeTorque = 0;

            // Update wheels
            foreach (Wheel wheel in wheels)
            {
                if (wheel.steering)
                    wheel.wheelCollider.steerAngle = steering;

                if (wheel.motor)
                    wheel.wheelCollider.motorTorque = motor;

                wheel.wheelCollider.brakeTorque = brakeTorque;

                wheel.UpdateWheel();
            }
        }
	}


}