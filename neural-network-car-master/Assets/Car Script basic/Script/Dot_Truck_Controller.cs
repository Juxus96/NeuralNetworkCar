using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Dot_Truck : System.Object
{
	public WheelCollider leftWheel;
	public GameObject leftWheelMesh;
	public WheelCollider rightWheel;
	public GameObject rightWheelMesh;
	public bool motor;
	public bool steering;
}

public class Dot_Truck_Controller : MonoBehaviour {

	public float maxMotorTorque;
	public float maxSteeringAngle;
	public List<Dot_Truck> truck_Infos;

	public void VisualizeWheel(Dot_Truck wheelPair)
	{
		Quaternion rot;
		Vector3 pos;
		wheelPair.leftWheel.GetWorldPose ( out pos, out rot);
		wheelPair.leftWheelMesh.transform.position = pos;
		wheelPair.leftWheelMesh.transform.rotation = rot;
		wheelPair.rightWheel.GetWorldPose ( out pos, out rot);
		wheelPair.rightWheelMesh.transform.position = pos;
		wheelPair.rightWheelMesh.transform.rotation = rot;
	}

	public void Update()
	{
        // Get the output values motor =   steering -> 0 Left | 1 Right
        if (NeuralController.motor >= 1) print("a");
		float motor = maxMotorTorque * (float)NeuralController.motor + 0.2f;
		float steering = maxSteeringAngle * (float)(NeuralController.steering -0.5f) * 2;
		float brakeTorque = 0;

        if (NeuralController.motor < 0.25f)
        {
            motor = 0;
            brakeTorque = maxMotorTorque;
        }
        else
            brakeTorque = 0;


        foreach (Dot_Truck truck_Info in truck_Infos)
		{
			if (truck_Info.steering) 
				truck_Info.leftWheel.steerAngle = truck_Info.rightWheel.steerAngle = steering;
			
			if (truck_Info.motor)
				truck_Info.leftWheel.motorTorque = truck_Info.rightWheel.motorTorque = motor;

			truck_Info.leftWheel.brakeTorque = truck_Info.rightWheel.brakeTorque = brakeTorque;

			VisualizeWheel(truck_Info);
		}

	}


}