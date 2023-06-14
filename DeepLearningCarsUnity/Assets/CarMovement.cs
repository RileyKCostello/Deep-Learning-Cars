using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    private float rotate;
    private Vector3 startPos = new Vector3(-104f, 2.54f, -43f);
    private Vector3 startRotation = new Vector3(0f,90f,0f);
    private Vector3 lastPos;
    private float totalDist;
    private float avgSpeed;
    private float desiredSpeed;
    private bool canDrive = true;
    public NeuralNetwork nNet;
    public GHandler gHandler;
    public float timer;
    public int id;

    [Header("Car Stats")]
    public float speed;
    public float minSpeed;
    public float maxSpeed;
    public float enginePower;
    public float turnSpeed = 2.8f;

    [Header("Outputs")]
    public float turn;
    public float engine;

    [Header("Fitness")]
    public float fitness;
    public float distanceMultiplier;
    public float speedMultiplier;


    private void Awake()
    {
        speed = minSpeed;
    }

    public void Reset()
    {
        canDrive = true;
        fitness = 0;
        timer = 0;
        totalDist = 0;
        avgSpeed = 0;
        transform.position = startPos;
        transform.eulerAngles = startRotation;
    }

    private void OnCollisionEnter(Collision collision)
    {
        canDrive = false;
        lastPos = transform.position;
        gHandler.EndCar(this);
    }

    // Calculates the fitness of the run based on the speed and total distance travelled
    private float calculateFitness()
    {
        float fitness;
        avgSpeed = totalDist / timer;
        fitness = totalDist * distanceMultiplier + avgSpeed * speedMultiplier;
        return fitness;
    }

    private void driveCar()
    {
        if (!canDrive)
        {
            return;
        }
        //Rotational Movement
        rotate = turn/2;
        transform.Rotate(new Vector3(0, rotate * turnSpeed, 0));
        //Engine Movement
        desiredSpeed = Mathf.Lerp(minSpeed, maxSpeed, (engine+1)/2);
        speed = speed + (desiredSpeed - speed) * enginePower;
        gameObject.transform.localPosition = transform.position + (transform.forward * speed * 0.02f);
        totalDist += speed;
    }

    private void FixedUpdate()
    {
        nNet.CalculateOutputs();
        turn = nNet.turn;
        engine = nNet.engine;
        driveCar();
        timer += 0.02f;
        fitness = calculateFitness();
    }
}
