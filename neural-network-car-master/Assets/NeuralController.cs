using UnityEngine;


public class NeuralController : MonoBehaviour {

	public float sensorLenght;
    private Rigidbody rb;

	private Vector3 fRaycast;
    private Vector3 rRaycast;
    private Vector3 lRaycast;

    public int timeScale;

	public int population;
	public static int staticPopulation;

    public float driveTime = 0;
	public static float steering;
	public static float braking;
	public static float motor;

	public static int generation = 0;
	public float [] distance;
	public float[] results;
	private float[] sensors;
	public static int currentCar = 0;

	public static float bestDistance = 0;


	NeuralNetwork [] carNetworks;
	RaycastHit hit;

	Vector3 lastFramePos;

    // Use this for initialization
    void Start()
    {
		int[] parameters = { 3, 4, 2 };
		staticPopulation = population;

        Time.timeScale = timeScale;

        rb = GetComponent<Rigidbody>();

        results = new float[2];
        distance = new float[population];
        sensors = new float[3];


		//default vector values
        fRaycast = Vector3.forward * 2;
        rRaycast = new Vector3(0.4f, 0, 0.7f);
        lRaycast = new Vector3(-0.4f, 0, 0.7f);
       
        lastFramePos = transform.position;
        carNetworks = new NeuralNetwork[population];


        for (int i = 0; i < population; i++)
        {
			carNetworks[i] = new NeuralNetwork(parameters);
        }

    }

	void FixedUpdate()
	{
		sensors [0] = GetSensor (lRaycast);
		sensors [1] = GetSensor (fRaycast);
		sensors [2] = GetSensor (rRaycast);


		results = carNetworks[currentCar].Process(sensors);
		steering =  results [0];
		motor = results [1];

		driveTime += Time.deltaTime;

		distance[currentCar] += Vector3.Distance(lastFramePos, transform.position);
		lastFramePos = transform.position;

	}
	
	// Update is called once per frame
	void Update () {
        
		Time.timeScale = timeScale;
        
		//check if the network is moving
		if(driveTime > 3 && rb.velocity.magnitude<0.005)
        {
			//Debug.Log ("This one stands still!");
            OnCollisionEnter(null);
        }

	}


	//game over, friend :/
	private void OnCollisionEnter (Collision col)
	{
        Crashed();
	}

    private void ResetCarPosition()
    {
        rb.Sleep();
        transform.position = new Vector3(0, 1, 0);
        transform.rotation = new Quaternion(0, 0, 0, 0);
    }


		

	private float GetSensor(Vector3 direction)
	{
		Vector3 fwd = transform.TransformDirection(direction);
        float result = 0.0f;
		if (Physics.Raycast (transform.position, fwd, out hit)) {
			if (hit.distance < sensorLenght) {
				Debug.DrawRay (transform.position, fwd * sensorLenght, Color.red, 0, true);
				result = 1f - hit.distance / sensorLenght;
			} else {
				Debug.DrawRay (transform.position, fwd * sensorLenght, Color.green, 0, true);
			}
		}
		else
			Debug.DrawRay(transform.position, fwd * sensorLenght, Color.green, 0, true);

		return result;
	}

    private NeuralNetwork GetFittest()
    {
        float maxValue = distance[0];
        int maxIndex = 0;

        for (int i = 1; i < population; i++)
        {
            if (distance[i] > maxValue)
            {
                maxIndex = i;
                maxValue = distance[i];
            }
        }
        if (distance[maxIndex] > bestDistance)
            bestDistance = distance[maxIndex];

        distance[maxIndex] = float.MinValue;

        return carNetworks[maxIndex];
    }

    private void CreateNewGen()
    {
        NeuralNetwork father = GetFittest();
        NeuralNetwork mother = GetFittest();
        for (int i = 0; i < population; i++)
        {
            distance[i] = 0;
            carNetworks[i] = new NeuralNetwork(father, mother);
        }
        generation++;
    }

    private void Crashed()
    {
        ResetCarPosition();
        driveTime = 0;
        currentCar++;
        lastFramePos = transform.position;

        // New gen
        if (currentCar >= population)
        {
            CreateNewGen();
            currentCar = 0;
        }
    }


}

