using UnityEngine;

[System.Serializable]
public class NeuralNetwork {

    // the weight of the conections 
    // weights[0] has the connections from the input layer to the hidden layer
    // weights[1] has the connections from the hidden layer to the output layer
    public float [][][] weights;
	public int layerCount;
    // each position has the count of neurons of the layer
	public int [] layerNodes;

    // Creates a new NeuralNetwork getting a combination of other 2, and mutates 1 of its weights
	public NeuralNetwork(NeuralNetwork Dad, NeuralNetwork Mom, float mutationRate)
	{
		layerNodes = Mom.layerNodes;
		initializeVariables ();

		for (int i = 0; i < layerNodes.Length - 1; i++)
        {
			weights [i] = new float[layerNodes [i]][];

			for (int j = 0; j < layerNodes [i]; j++)
            {
				weights [i] [j] = new float[layerNodes [i + 1]];

                for (int k = 0; k < layerNodes [i + 1]; k++)
                {
                    if(Random.Range(0.0f,100.0f) >= mutationRate)
                    {
                        if (Random.Range(0, 2) == 0)
                            weights[i][j][k] = Mom.weights[i][j][k];
                        else
                            weights[i][j][k] = Dad.weights[i][j][k];
                    }
                    else
                    {
                        weights[i][j][k] = GetRandomWeight();
                    }
						
                }
			}
		}

		//int mutationLayer = Random.Range(0, weights.Length);
		//int mutationLeft  = Random.Range(0, weights[mutationLayer].Length);
		//int mutationRight = Random.Range(0, weights[mutationLayer][mutationLeft].Length);

        //weights [mutationLayer] [mutationLeft] [mutationRight] = GetRandomWeight ();
	}
			
    // Creates NeuralNetwork with the given layers and nodes
	public NeuralNetwork(int [] _layerNodes)
	{
		layerNodes = _layerNodes;
		initializeVariables ();

		for (int i = 0; i < layerNodes.Length - 1 ; i++)
        {
			weights[i] = new float[layerNodes[i]][];

			for (int j = 0; j < layerNodes [i]; j++)
            {
				weights[i][j] =  new float[layerNodes[i+1]];

                for (int k = 0; k < layerNodes [i + 1]; k++)
					weights [i] [j] [k] = GetRandomWeight ();
			}
		}

	}

    private void initializeVariables()
    {
        weights = new float[layerNodes.Length - 1][][];
        layerCount = layerNodes.Length;
    }

    public float [] Process(float [] inputs)
	{
        float[] outputs;
        // Needs to get the value of every layer but the last
		for (int i = 0; i < (layerCount-1); i++)
        {
			// Create the output of i+1
			outputs = new float[layerNodes [i+1]];

            // Set the value of each output node multiplying its value by the weight of the connection
			for (int j = 0; j < inputs.Length; j++)
				for (int k = 0; k < outputs.Length; k++) 
					outputs [k] += inputs [j] * weights [i] [j] [k];

            // set the outputs as the inputs for the next layer
            inputs = new float[outputs.Length];

			for (int l = 0; l < outputs.Length; l++) 
				inputs [l] = Sigmoid(outputs [l] * 5);
		}
		return inputs;
	}

	private float GetRandomWeight()
    {
		return Random.Range(-1.0f, 1.0f);              
    }

    private float Sigmoid(float x)
    {
        return 1 / (1 + Mathf.Exp(-x));
    }

}
