using TMPro;
using UnityEngine;


public class NeuralController : MonoBehaviour
{

    public static NeuralController instance;

    public bool Started { get; set; }
    // Inspector change
    public float sensorLenght;
    public int populationPerGen;
    public int mutationChance;

    // Variables of each car
    private float driveTime = 0;
    private Vector3 lastFramePos;
    private Vector3 fRaycast;
    private Vector3 rRaycast;
    private Vector3 lRaycast;
   
    private Rigidbody rb;

    // Output variables
    public float steering;
	public float motor;

    // for UI
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI timeScaleText;
    [SerializeField] private TextMeshProUGUI generationText;
    [SerializeField] private TextMeshProUGUI numberText;
    [SerializeField] private TextMeshProUGUI bestDistanceText;
    

    private int generation = 1;
    public int Generation
    {
        get { return generation; }
        set
        {
            generation = value;
            generationText.text = "Generation " + generation;
        }
    }

    private int currentCar;
    public int CurrentCar
    {
        get { return currentCar; }
        set
        {
            currentCar = value;
            numberText.text = currentCar + 1 + " out of " + populationPerGen;
        }
    }

	private float bestDistance;
    public float BestDistance
    {
        get { return bestDistance; }
        set
        {
            bestDistance = value;
            bestDistanceText.text = "Best Distance: " + bestDistance.ToString("F2");
        }
    }

    [SerializeField] private float[] distance;
    private float[] results;
	private float[] sensors;

    private NeuralNetwork[] carNetworks;

    private void Awake()
    {
        instance = this;

        rb = GetComponent<Rigidbody>();
        lastFramePos = transform.position;

        results = new float[2];
        distance = new float[populationPerGen];
        sensors = new float[3];

        // Sensor directions
        fRaycast = Vector3.forward * 2;
        rRaycast = new Vector3(0.4f, 0, 0.7f);
        lRaycast = new Vector3(-0.4f, 0, 0.7f);
    }

    public void Generate()
    {
        if (!Started)
        {
            Started = true;

            // 3- Input Layer | 4 - Hidden Layer | 2 - Output Layer
            int[] parameters = { 3, 4, 2 };

            carNetworks = new NeuralNetwork[populationPerGen];

            for (int i = 0; i < populationPerGen; i++)
                carNetworks[i] = new NeuralNetwork(parameters);
        }
    }

	void FixedUpdate()
	{
        if (Started)
        {
            sensors[0] = GetSensor(lRaycast);
            sensors[1] = GetSensor(fRaycast);
            sensors[2] = GetSensor(rRaycast);

            results = carNetworks[currentCar].Process(sensors);
            steering = results[0];
            motor = results[1];

            driveTime += Time.deltaTime;

            distance[currentCar] += Vector3.Distance(lastFramePos, transform.position);
            lastFramePos = transform.position;
        }
	}
	
	void Update ()
    {
        if (Started)
        {
            // Check if its stopped
            if (driveTime > 3 && rb.velocity.magnitude < 0.005)
            {
                Crashed();
            }
        }
            
	}

	private void OnCollisionEnter (Collision col)
	{
        if (col.collider.CompareTag("Circuit"))
            Crashed();
    }

    private void ResetCarPosition()
    {
        rb.Sleep();
        transform.position = new Vector3(0, 0.72f, 0);
        transform.rotation = new Quaternion(0, 0, 0, 0);
    }

    // Returns a value from 1 to 0 depending on the distance to the obejct hit, 1 - close 0 - far
	private float GetSensor(Vector3 _direction)
	{
        RaycastHit hit;
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

        for (int i = 1; i < populationPerGen; i++)
            if (distance[i] > maxValue)
            {
                maxIndex = i;
                maxValue = distance[i];
            }

        if (distance[maxIndex] > bestDistance)
            BestDistance = distance[maxIndex];

        distance[maxIndex] = float.MinValue;

        return carNetworks[maxIndex];
    }

    // Creates a new generation with the best 2 of the last one
    private void CreateNewGen()
    {
        NeuralNetwork father = GetFittest();
        NeuralNetwork mother = GetFittest();
        for (int i = 0; i < populationPerGen; i++)
        {
            distance[i] = 0;
            carNetworks[i] = new NeuralNetwork(father, mother, mutationChance);
        }
        ++Generation;
    }

    // Resets the car and if was the last one creates the new gen
    private void Crashed()
    {
        ResetCarPosition();
        driveTime = 0;
        GetComponent<TrailRenderer>().Clear();
        ++CurrentCar;
        lastFramePos = transform.position;

        // New gen
        if (CurrentCar >= populationPerGen)
        {
            CreateNewGen();
            CurrentCar = 0;
        }
    }

    public void SetTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
        timeScaleText.text = "TimeScale: " + timeScale;
    }

   
}

