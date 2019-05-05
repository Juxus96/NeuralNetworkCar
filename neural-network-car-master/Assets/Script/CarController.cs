using UnityEngine;

[System.Serializable]
public class Wheel 
{
	public WheelCollider wheelCollider;
	public GameObject wheelMesh;
	public bool motor;
	public bool steering;
}

public class CarController : MonoBehaviour {

	public float maxMotorTorque;
	public float maxSteeringAngle;
	public Wheel[] wheels;

	public void SetwheelVisuals(Wheel wheel)
	{
		Quaternion rot;
		Vector3 pos;
		wheel.wheelCollider.GetWorldPose ( out pos, out rot);
		wheel.wheelMesh.transform.position = pos;
		wheel.wheelMesh.transform.rotation = rot;
	}

	public void Update()
	{
        // Get the output values motor = 1 100% | 0 0%  / steering -> -1 Left | 1 Right
		float motor = maxMotorTorque * NeuralController.motor + 0.2f;
		float steering = maxSteeringAngle * (NeuralController.steering - 0.5f) * 2;
		float brakeTorque = 0;

        // if the motor is to slow, stop
        if (NeuralController.motor < 0.25f)
        {
            motor = 0;
            brakeTorque = maxMotorTorque;
        }
        else
            brakeTorque = 0;


        foreach (Wheel wheel in wheels)
		{
			if (wheel.steering) 
				wheel.wheelCollider.steerAngle = steering;
			
			if (wheel.motor)
				wheel.wheelCollider.motorTorque  = motor;

			wheel.wheelCollider.brakeTorque = brakeTorque;

			SetwheelVisuals(wheel);
		}

	}


}