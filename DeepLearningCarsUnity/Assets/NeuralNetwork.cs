using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralNetwork : MonoBehaviour
{
    public float tempID;

    [Header("Inputs")]
    public HeadlampRay Sensors;
    public float sensorA;
    public float sensorB;
    public float sensorC;
    public float sensorD;
    public float sensorE;

    [Header("Outputs")]
    public float turn;
    public float engine;

    public struct Layer
    {
        public int layerNum;
        public int numNeurons;
        public Matrix neurons;
        public Matrix weights;
        public float[] biases;
        public Layer(int layerNum, int numNeurons, int numWieghts)
        {
            this.layerNum = layerNum;
            this.numNeurons = numNeurons;
            neurons = new Matrix(1, numNeurons);
            weights = new Matrix(numNeurons, numWieghts);
            biases = new float[numNeurons];
        }

        //Returns the neuron matrix for the next layer
        public Matrix Next()
        {
            Matrix output = neurons * weights;
            for (int i = 0; i < output.entries.Length; i++)
            {
                output.entries[i] += biases[i];
                output.entries[i] = (float)System.Math.Tanh(output.entries[i]);
            }
            return output;
        }

        public void SetNeurons(Matrix matrix)
        {
            if(neurons.entries.Length != matrix.GetRow(1).Length)
            {
                return;
            }
            neurons.FillRow(matrix.GetRow(1), 1);
        }

        public void SetBiases(float[] bias)
        {
            if(biases.Length != bias.Length)
            {
                return;
            }
            for(int i = 0; i < biases.Length; i++)
            {
                biases[i] = bias[i];
            }
        }

        public void SetWeights(Matrix wMatrix)
        {
            if (weights.entries.Length != wMatrix.entries.Length)
            {
                return;
            }

            for (int i = 0; i < weights.entries.Length; i++)
            {
                weights.entries[i] = wMatrix.entries[i];
            }
        }

        public void RandomBias()
        {
            for (int i = 0; i < numNeurons; i++)
            {
                biases[i] = Random.Range(-1f, 1f);
            }
        }

        public void RandomWeight()
        {
            for(int i = 0; i < weights.entries.Length; i++)
            {
                weights.entries[i] = Random.Range(-1f, 1f);
            }
        }

        public void Print()
        {
            Debug.Log(weights.ToString());
            string biasString = "";
            for(int i = 0; i < biases.Length; i++)
            {
                biasString += " " + biases[i].ToString();
            }
            Debug.Log(biasString);
        }
    }

    /*TODO / Build Order
     * 1. Create instances of layer structs
     * 2. Set Weights and biases through randomness or from genetic algorithm
     * 3. While running through the maze calculate neurons of the next layer with Layer.Next()
     * 4. Repeat until reaching the output layer, TODO: attach these values to the CarMovement script
     * 5. Develop Genetic Algorithm
     */
    //Layers
    public Layer input = new Layer(0, 5, 4);
    public Layer h1 = new Layer(1, 4, 3);
    public Layer h2 = new Layer(1, 3, 2);
    Matrix outputLayer = new Matrix(1, 2);

    //Fills the weights and biases with randomly generated numbers
    public void InitRandom()
    {
        input.RandomBias();
        input.RandomWeight();
        h1.RandomBias();
        h1.RandomWeight();
        h2.RandomBias();
        h2.RandomWeight();
    }

    public void SetInputs()
    {
        sensorA = Sensors.FSensor;
        sensorB = Sensors.FRSensor;
        sensorC = Sensors.FLSensor;
        sensorD = Sensors.LSensor;
        sensorE = Sensors.RSensor;
        float[] inputArray = { sensorA, sensorB, sensorC, sensorD, sensorE };
        input.neurons.FillRow(inputArray, 1);
    }

    public void CalculateOutputs()
    {
        SetInputs();
        h1.SetNeurons(input.Next());
        h2.SetNeurons(h1.Next());
        outputLayer = h2.Next();
        turn = outputLayer.entries[0];
        engine = outputLayer.entries[1];
    }

    public void CopyLayers(NeuralNetwork toCopy)
    {

        input.SetWeights(toCopy.input.weights);
        input.SetBiases(toCopy.input.biases);
        h1.SetWeights(toCopy.h1.weights);
        h1.SetBiases(toCopy.h1.biases);
        h2.SetWeights(toCopy.h2.weights);
        h2.SetBiases(toCopy.h2.biases);
    }


    public void FixedUpdate()
    {
        //If you see this in the future and everything still works delete these
        //SetInputs();
        //CalculateID();
    }

    public void Start() 
    {
        tempID = CalculateID();
    }

    public float CalculateID()
    {
        float a = 0f;
        for (int i = 0; i < input.weights.entries.Length; i++)
        {
            a += input.weights.entries[i];
        }

        for (int i = 0; i < h1.weights.entries.Length; i++)
        {
            a += h1.weights.entries[i];
        }

        for (int i = 0; i < h2.weights.entries.Length; i++)
        {
            a += h2.weights.entries[i];
        }

        for (int i = 0; i < input.biases.Length; i++)
        {
            a += input.biases[i];
        }
        for (int i = 0; i < h1.biases.Length; i++)
        {
            a += h1.biases[i];
        }
        for (int i = 0; i < h2.biases.Length; i++)
        {
            a += h2.biases[i];
        }

        return a;
    }
}
