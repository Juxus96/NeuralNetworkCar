using UnityEngine;


public class NeuralController : MonoBehaviour
{

    // Inspector change
	public float sensorLenght;
    public int timeScale;
	public int population;

    // Variables of each car
    [SerializeField] private float driveTime = 0;
    private Vector3 lastFramePos;
    private Vector3 fRaycast;
    private Vector3 rRaycast;
    private Vector3 lRaycast;
    private RaycastHit hit;
    private Rigidbody rb;

    // Output variables
    public static float steering;
	public static float motor;

    // for UI
	public static int staticPopulation;
	public static int generation = 0;
	public static int currentCar = 0;
	public static float bestDistance = 0;

	[SerializeField] private float[] distance;
    private float[] results;
	private float[] sensors;

    private NeuralNetwork[] carNetworks;

    void Start()
    {
        // 3- Input Layer | 4 - Hidden Layer | 2 - Output Layer
		int[] parameters = { 3, 4, 2 };

        rb = GetComponent<Rigidbody>();
		staticPopulation = population;
        lastFramePos = transform.position;

        results  = new float[2];
        distance = new float[population];
        sensors  = new float[3];

		// Sensor directions
        fRaycast = Vector3.forward * 2;
        rRaycast = new Vector3(0.4f, 0, 0.7f);
        lRaycast = new Vector3(-0.4f, 0, 0.7f);
       
        carNetworks = new NeuralNetwork[population];

        for (int i = 0; i < population; i++)
			carNetworks[i] = new NeuralNetwork(parameters);

    }

	void FixedUpdate()
	{
		sensors [0] = GetSensor (lRaycast);
		sensors [1] = GetSensor (fRaycast);
		sensors [2] = GetSensor (rRaycast);

		results  = carNetworks[currentCar].Process(sensors);
		steering = results [0];
		motor    = results  [1];

		driveTime += Time.deltaTime;

		distance[currentCar] += Vector3.Distance(lastFramePos, transform.position);
		lastFramePos = transform.position;

	}
	
	void Update ()
    {
        
		Time.timeScale = timeScale;
        
        // Check if its stopped
		if(driveTime > 3 && rb.velocity.magnitude<0.005)
        {
            OnCollisionEnter(null);
        }

	}

	private void OnCollisionEnter (Collision col)
	{
        Crashed();
	}

    private void ResetCarPosition()
    {
        rb.Sleep();
        transform.position = new Vector3(0, 0.36f, 0);
        transform.rotation = new Quaternion(0, 0, 0, 0);
    }

    // Returns a value from 1 to 0 depending on the distance to the obejct hit, 1 - close 0 - far
	private float GetSensor(Vector3 _direction)
	{
		Vector3 direction = transform.TransformDirection(_direction);
        float result = 0.0f;
		if (Physics.Raycast (transform.position, direction, out hit))
        {
			if (hit.distance < sensorLenght)
            {
				Debug.DrawRay (transform.position, direction * sensorLenght, Color.red, 0, true);
				result = 1f - hit.distance / sensorLenght;
			}
            else
				Debug.DrawRay (transform.position, direction * sensorLenght, Color.green, 0, true);
		}
		else
			Debug.DrawRay(transform.position, direction * sensorLenght, Color.green, 0, true);

		return result;
	}
    // Returns the best car depending on the distance.
    private NeuralNetwork GetFittest()
    {
        float maxValue = distance[0];
        int maxIndex = 0;

        for (int i = 1; i < population; i++)
            if (distance[i] > maxValue)
            {
                maxIndex = i;
                maxValue = distance[i];
            }

        if (distance[maxIndex] > bestDistance)
            bestDistance = distance[maxIndex];

        distance[maxIndex] = float.MinValue;

        return carNetworks[maxIndex];
    }

    // Creates a new generation with the best 2 of the last one
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

    // Resets the car and if was the last one creates the new gen
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

