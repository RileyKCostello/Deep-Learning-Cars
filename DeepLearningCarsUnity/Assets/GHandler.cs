using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GHandler : MonoBehaviour
{
    //numCars should be divisible by 5
    public GameObject carPrefab;
    [Header("Options")]
    public bool childrenAreAveraged;
    public bool randomChildren;
    public int numMutantMutations;
    public int numChildMutations;
    [Header("Car ratios")]
    public int numCars;
    public int numFrontRunners;
    public int numChildren;
    public int numMutants;
    public int numRandom;
    public int remainingCars;
    public GameObject[] cars;
    float[] fitnessList;
    public float timeSpeed = 1;
    public int generation = 1;


    /*Ideas to improve learning rate, may be implemented layer
     * Improve car stats or edit track
     * Frontrunners with significantly higher fitness than the rest will have more children
     * Increase the range of weights and biases, when mutating they will be moved up or down by 0.05 to 1
     * Add layers or increase size
     */

    /*Todo/ ideas:
     * Option to remove frontrunners from being initialized, they would still be kept for generating children and mutants until replaced
     * Create a gui
     * 
     */
    public void Start()
    {
        if(numChildren > (numFrontRunners * (numFrontRunners - 1))/2 && numChildMutations < 1)
        {
            Debug.Log("Current settings will create cloned children consider \n" +
                "Increasing the number of front runners\n" +
                "Lowering the number of children\n" +
                "Adding mutations to children");
        }
        numCars = numFrontRunners + numChildren + numMutants + numRandom;
        remainingCars = numCars;
        cars = new GameObject[numCars];
        fitnessList = new float[numCars];
        GenerateRandom(numCars);
    }

    public void FixedUpdate()
    {
        Time.timeScale = timeSpeed;

        if(remainingCars == 0)
        {
            remainingCars = numCars;
            SetCars();
            generation += 1;
        }
        //Debug.Log("TEMPID: " + cars[0].GetComponent<NeuralNetwork>().tempID.ToString());
    }

    //Generator functions

    public void GenerateRandom(int amount)
    {
        Color color = new Color(0.8f, 0, 0);
        for (int i = cars.Length - amount; i < cars.Length; i++)
        {
            cars[i] = GameObject.Instantiate(carPrefab);
            cars[i].GetComponent<NeuralNetwork>().InitRandom();
            cars[i].name = "Random";
            cars[i].GetComponent<Renderer>().material.color = color;
            cars[i].GetComponent<CarMovement>().id = i;
            cars[i].GetComponent<CarMovement>().gHandler = this;
        }
    }

    public void SetCars()
    {
        GameObject[] bestCars = GetBest();
        for(int i = 0; i < numCars; i++)
        {
            Destroy(cars[i]);
        }
        for (int i = 0; i < numFrontRunners; i++)
        {
            cars[i] = bestCars[i];
            cars[i].GetComponent<Renderer>().material.color = new Color(0, 0, 0.8f);
            cars[i].GetComponent<CarMovement>().id = i;
        }

        //Break incase of emergency
        /*
        NeuralNetwork a = cars[0].GetComponent<NeuralNetwork>();
        Debug.Log("Check 1: " + (a.input.weights[1, 1] + a.h1.weights[1, 1] + a.h2.weights[1, 1]));
        */
        if (randomChildren)
        {
            GenerateRandomChildren();
        }
        else
        {
            GenerateChildren();
        }
        GenerateMutants();
        GenerateRandom(numRandom);

        for (int i = 0; i < numCars; i++)
        {
            cars[i].GetComponent<CarMovement>().Reset();
        }
    }


    //Generates children with nonrandom parents
    public void GenerateChildren()
    {
        int index = numCars/5;
        NeuralNetwork a = cars[0].GetComponent<NeuralNetwork>();
        NeuralNetwork b = cars[numCars/5 - 1].GetComponent<NeuralNetwork>();
        if (childrenAreAveraged)
        {
            cars[index] = AverageNetworks(a, b);
        }
        else
        {
            cars[index] = MixNetworks(a, b);
        }
        cars[index].GetComponent<CarMovement>().id = index;
        for (int i = 1; i < (numCars/5); i++)
        {
            NeuralNetwork nNetA = cars[i].GetComponent<NeuralNetwork>();
            NeuralNetwork nNetB = cars[i - 1].GetComponent<NeuralNetwork>();
            if (childrenAreAveraged)
            {
                cars[i + index] = AverageNetworks(nNetA, nNetB);
            }
            else
            {
                cars[i + index] = MixNetworks(nNetA, nNetB);
            }
            cars[i + index].GetComponent<CarMovement>().id = i + index;
            if (numChildMutations > 0)
            {
                MutateSingle(cars[i + index].GetComponent<NeuralNetwork>(), numChildMutations);
            }

            //Code to generate a two childs
            //cars[(i * 2) + index] = MixNetworks(nNetA, nNetB);
            //cars[(i * 2) + index].GetComponent<CarMovement>().id = (i * 2) + index;
            //cars[(i * 2) + 1 + index] = MixNetworks(nNetB, nNetA);
            //cars[(i * 2) + 1 + index].GetComponent<CarMovement>().id = (i * 2) + 1 + index;
        }
    }

    //Generates children with random parents
    public void GenerateRandomChildren()
    {
        int p1;
        int p2 = 0;
        int loopCounter;
        int totalNumCombos = (numFrontRunners * (numFrontRunners-1)) / 2 ;
        int[] usedCombos = new int[numChildren];
        //Fill usedCombos with -1
        for (int i = 0; i < numChildren; i++) 
        {
            usedCombos[i] = -1;
        }

        for (int i = 0; i < numChildren; i++)
        {
            bool inArray = true;
            int pick = 0;
            loopCounter = 0;
            while (inArray)
            {
                loopCounter++;
                pick = Random.Range(0, totalNumCombos - 1);
                inArray = false;
                if (numChildMutations > 0 || loopCounter > 20)
                {
                    break;
                }
                foreach (int x in usedCombos)
                {
                    if (x == pick)
                    {
                        inArray = true;
                        break;
                    }
                }
            }
            //Extracts the first and second parent from the generated number
            usedCombos[i] = pick;
            p1 = 0;
            for (int x = numFrontRunners - 1; x > 0; x -= 1)
            {
                if(pick < x)
                {
                    p2 = pick + 1 + p1;
                    break;
                }
                p1++;
                pick -= x;
            }
            if (childrenAreAveraged)
            {
                cars[numFrontRunners + i] = AverageNetworks(cars[p1].GetComponent<NeuralNetwork>(), cars[p2].GetComponent<NeuralNetwork>());
            }
            else
            {
                cars[numFrontRunners + i] = MixNetworks(cars[p1].GetComponent<NeuralNetwork>(), cars[p2].GetComponent<NeuralNetwork>());
            }
            cars[numFrontRunners + i].GetComponent<CarMovement>().id = numFrontRunners + i;
            if (numChildMutations > 0)
            {
                MutateSingle(cars[numFrontRunners + i].GetComponent<NeuralNetwork>(), numChildMutations);
            }
        }
    }

    //Generates mutants half of them are layer mutants the rest are mutated once
    public void GenerateMutants()
    {
        int index = numFrontRunners + numChildren;

        //Code to generate layer mutants may be implemented in the future
        /*
        for (int i = 0; i < (numCars/5); i++)
        {
            cars[i + index] = Instantiate(cars[i]);
            cars[i + index].GetComponent<NeuralNetwork>().CopyLayers(cars[i].GetComponent<NeuralNetwork>());
            cars[i + index].GetComponent<CarMovement>().gHandler = this;
            cars[i + index].GetComponent<CarMovement>().id = i + index;
            cars[i + index].GetComponent<Renderer>().material.color = new Color(0.8f, 0.2f, 0.8f);
            cars[i + index].name = "Full Mutant";
            MutateLayer(cars[i + index].GetComponent<NeuralNetwork>());
        }*/

        int toMutate = 0;
        for (int i = 0; i < numMutants; i++)
        {
            cars[i + index] = Instantiate(cars[toMutate]);
            cars[i + index].GetComponent<NeuralNetwork>().CopyLayers(cars[toMutate].GetComponent<NeuralNetwork>());
            cars[i + index].GetComponent<CarMovement>().gHandler = this;
            cars[i + index].GetComponent<CarMovement>().id = i + index;
            cars[i + index].GetComponent<Renderer>().material.color = new Color(0, 0.8f, 0);
            cars[i + index].name = "Part Mutant";
            MutateSingle(cars[i + index].GetComponent<NeuralNetwork>(), numMutantMutations);

            //Ensures that only frontrunnners are mutated sometimes mutating each more than once if necessary
            toMutate++;
            if(toMutate > numFrontRunners)
            {
                toMutate = 0;
            }
        }
    }

    //Mutates just one weight and bias
    //Possibly change it to mutate a weight and bias in two or all layers
    public void MutateSingle(NeuralNetwork nNet, int numMutations)
    {
        Dictionary<int, NeuralNetwork.Layer> x = new Dictionary<int, NeuralNetwork.Layer>()
        {
            {0, nNet.input},
            {1, nNet.h1 },
            {2, nNet.h2 }
        };

        for (int i = 0; i < numMutations; i++)
        {
            int toMutate = Random.Range(0, 2);
            x[toMutate].weights.entries[Random.Range(0, x[toMutate].weights.entries.Length - 1)] = Random.Range(-1f, 1f);
            x[toMutate].biases[Random.Range(0, x[toMutate].biases.Length - 1)] = Random.Range(-1f, 1f);
        }

    }

    //Randomizes all the weights and biases in a layer
    public void MutateLayer(NeuralNetwork nNet)
    {
        Dictionary<int, NeuralNetwork.Layer> x = new Dictionary<int, NeuralNetwork.Layer>()
        {
            {0, nNet.input},
            {1, nNet.h1 },
            {2, nNet.h2 }
        };

        int toMutate = Random.Range(0, 2);
        x[toMutate].RandomWeight();
        x[toMutate].RandomBias();

        /*int toKeep = Random.Range(0, 2);
        foreach(var a in x)
        {
            if(a.Key != toKeep)
            {
                a.Value.RandomWeight();
                a.Value.RandomBias();
            }
        }*/

    }

    //Generates a new car with 2 layers from a and 1 from b
    //Has a chance of randomizing one of the layers
    public GameObject MixNetworks(NeuralNetwork a, NeuralNetwork b)
    {
        GameObject Child = GameObject.Instantiate(carPrefab);
        Child.name = "Child";
        NeuralNetwork ChildNetwork = Child.GetComponent<NeuralNetwork>();
        Child.GetComponent<CarMovement>().gHandler = this;
        Child.GetComponent<Renderer>().material.color = new Color(0.8f, 0, 0.8f);
        ChildNetwork.CopyLayers(a);
        ChildNetwork.h2.SetWeights(b.h2.weights);
        ChildNetwork.h2.SetBiases(b.h2.biases);

        //Chance to randomize a layer
        Dictionary<int, NeuralNetwork.Layer> x = new Dictionary<int, NeuralNetwork.Layer>()
        {
            {0, ChildNetwork.input},
            {1, ChildNetwork.h1 },
            {2, ChildNetwork.h2 }
        };
        int randInt = Random.Range(0, 5);
        if (randInt < 3)
        {
            x[randInt].RandomWeight();
            x[randInt].RandomBias();
        }
        return Child;
    }

    //Creates a new car with weights and biases that are the average of networks a and b
    public GameObject AverageNetworks(NeuralNetwork a, NeuralNetwork b)
    {
        GameObject Child = GameObject.Instantiate(carPrefab);
        Child.name = "Child";
        NeuralNetwork ChildNetwork = Child.GetComponent<NeuralNetwork>();
        Child.GetComponent<CarMovement>().gHandler = this;
        Child.GetComponent<Renderer>().material.color = new Color(0.8f, 0, 0.8f);

        Matrix childInput = CreateAverageMatrix(a.input.weights, b.input.weights);
        ChildNetwork.input.SetWeights(childInput);
        ChildNetwork.input.SetBiases(CreateAverageArray(a.input.biases, b.input.biases));

        Matrix childH1 = CreateAverageMatrix(a.h1.weights, b.h1.weights);
        ChildNetwork.h1.SetWeights(childH1);
        ChildNetwork.h1.SetBiases(CreateAverageArray(a.h1.biases, b.h1.biases));

        Matrix childH2 = CreateAverageMatrix(a.h2.weights, b.h2.weights);
        ChildNetwork.h2.SetWeights(childH2);
        ChildNetwork.h2.SetBiases(CreateAverageArray(a.h2.biases, b.h2.biases));
        return Child;
    }

    //Helper function for AverageNetworks
    //Creates a new matrix where each entry is the average of the entry in matrix a and b
    public Matrix CreateAverageMatrix(Matrix a, Matrix b)
    {
        if (a.numRows != b.numRows || a.numColumns != b.numColumns)
        {
            Debug.Log("Different matrix sizes used when averaging");
            return null;
        }
        Matrix output = new Matrix(a.numColumns, a.numRows);
        for(int i = 0; i < output.entries.Length; i++)
        {
            output.entries[i] = (a.entries[i] + b.entries[i]) / 2;
        }
        return output;
    }

    //Helper function for AverageNetworks
    //Creates an array where each entry is the average of the entry in array a and b
    public float[] CreateAverageArray(float[] a, float[] b)
    {
        int size = a.Length;
        if(size != b.Length)
        {
            Debug.Log("Different size arrays used when averaging");
            return null;
        }
        float[] output = new float[size];
        for(int i = 0; i < size; i++)
        {
            output[i] = (a[i] + b[i]) / 2;
        }
        return output;
    }
    
    //Collection and Ordering Functions

    //Saves a cars fitness and lowers the number of remaining cars
    public void EndCar(CarMovement car)
    {
        fitnessList[car.id] = car.fitness;
        remainingCars -= 1;
    }

    //Generates a list of the highest fitness cars
    public GameObject[] GetBest()
    {
        //Debug.Log("New Selection");
        GameObject[] bestCars = new GameObject[numFrontRunners];
        for (int i = 0; i < numFrontRunners; i++)
        {
            int maxFitIndex = 0;
            float maxFitness = 0;
            for (int j = 0; j < numCars; j++)
            {
                if (fitnessList[j] > maxFitness) 
                {
                    maxFitness = fitnessList[j];
                    maxFitIndex = j;
                }
            }
            //Debug.Log(cars[maxFitIndex].GetComponent<CarMovement>().id.ToString() + " -->" + i.ToString() + " kept with fitness" + maxFitness.ToString() + "and tempID: " + cars[maxFitIndex].GetComponent<NeuralNetwork>().CalculateID().ToString());
            bestCars[i] = Instantiate(cars[maxFitIndex]);
            bestCars[i].GetComponent<NeuralNetwork>().CopyLayers(cars[maxFitIndex].GetComponent<NeuralNetwork>());
            bestCars[i].name = "Front Runner";
            fitnessList[maxFitIndex] = 0;
        }

        return bestCars;
    }

}
