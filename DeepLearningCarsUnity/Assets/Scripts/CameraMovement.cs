using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public GameObject gHandler;
    public float camSpeed;
    public GameObject[] cars;


    private void Start()
    {
        //Find a way to get a reference to the cars array from gHandler
    }

    private void SetHandler(GameObject gHandler)
    {
        this.gHandler = gHandler;
    }

    private GameObject FindBest()
    {
        cars = gHandler.GetComponent<GHandler>().cars;
        int index = 0;
        float bestFitness = 0;
        for (int i = 0; i < cars.Length; i++)
        {
            float testFit = cars[i].GetComponent<CarMovement>().fitness;
            if (testFit > bestFitness)
            {
                bestFitness = testFit;
                index = i;
            }
        }

        return cars[index];
    }

    private void MoveCam()
    {
        Vector3 currentPos = this.transform.position;
        Vector3 toViewPos = FindBest().transform.position;
        Vector3 posDif = toViewPos - currentPos;
        posDif.y = 0;
        transform.position += posDif * camSpeed * Time.deltaTime;
    }

    void Update()
    {
        MoveCam();

    }
}
