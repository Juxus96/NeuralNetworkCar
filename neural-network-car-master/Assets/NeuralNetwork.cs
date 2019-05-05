using UnityEngine;

[System.Serializable]
public class NeuralNetwork {

	public float [][][] weights;
	public int [] parameters;

	public int lenght;

    

	public NeuralNetwork(NeuralNetwork Dad, NeuralNetwork Mom)
	{
		parameters = Mom.parameters;
		initializeVariables ();

		for (int i = 0; i < parameters.Length - 1; i++) {

			weights [i] = new float[parameters [i]][];

			for (int j = 0; j < parameters [i]; j++) {

				weights [i] [j] = new float[parameters [i + 1]];

                for (int k = 0; k < parameters [i + 1]; k++) {

					if (Random.Range (0, 2) == 0) {
						weights [i] [j] [k] = Mom.weights [i] [j] [k];
					} else {
						weights [i] [j] [k] = Dad.weights [i] [j] [k];		
					}
                }
			}
		}

		int mutationLayer = Random.Range(0, weights.Length);
		int mutationLeft  = Random.Range(0, weights[mutationLayer].Length);
		int mutationRight = Random.Range(0, weights[mutationLayer][mutationLeft].Length);
        

        weights [mutationLayer] [mutationLeft] [mutationRight] = GetRandomWeight ();
	}
			

	public NeuralNetwork(int [] _parameters)
	{
		parameters = _parameters;
		initializeVariables ();

		for (int i = 0; i < parameters.Length - 1 ; i++) {
			
			weights[i] = new float[parameters[i]][];

			for (int j = 0; j < parameters [i]; j++) {
				
				weights[i][j] =  new float[parameters[i+1]];

                for (int k = 0; k < parameters [i + 1]; k++) {
				
					weights [i] [j] [k] = GetRandomWeight ();
				}

			}
		}

	}

    private float Sigmoid(float x)
    {
        return 1 / (1 + Mathf.Exp(-x));
    }

    private void initializeVariables()
    {
        weights = new float[parameters.Length - 1][][];
        lenght = parameters.Length;
    }

    public float [] Process(float [] inputs)
	{
        float[] outputs;
		//for each layer
		for (int i = 0; i < (lenght-1); i++) {
			
			outputs = new float[parameters [i+1]];


			//for each input neuron
			for (int j = 0; j < inputs.Length; j++) {
			
				//and for each output neuron
				for (int k = 0; k < outputs.Length; k++) {
					//Debug.Log (i + " " + j + " " + k);
					//a++;
					//increase the load of an output neuron by the value of each input neuron multiplied by the weight between them
					outputs [k] += inputs [j] * weights [i] [j] [k];
				}
			}

			//we have the proper output values, now we have to use them as inputs to the next layer and so on, until we hit the last layer
			inputs = new float[outputs.Length];

			//after all output neurons have their values summed up, apply the activation function and save the value into new inputs
			for (int l = 0; l < outputs.Length; l++) {
				inputs [l] = Sigmoid(outputs [l] * 5);
			}



		}
		return inputs;
	}

	private float GetRandomWeight()
    {
		return Random.Range(-1.0f, 1.0f);              
    }

}
