using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadlampRay : MonoBehaviour
{
    public float maxDistance;
    public LayerMask wallLayer;
    [Header("Sensors")]
    public float FSensor;
    public float FRSensor;
    public float FLSensor;
    public float RSensor;
    public float LSensor;
    // Start is called before the first frame update
    private float distCheck(Vector3 dir)
    {
        float sensor;
        Ray rayF = new Ray(transform.position, dir);
        sensor = 1;
        if (Physics.Raycast(rayF, out RaycastHit hit, maxDistance , wallLayer.value))
        {
            Debug.DrawRay(transform.position, dir * hit.distance, Color.red);
            sensor = hit.distance / maxDistance;
            //Debug.Log(distance);
        }
        return sensor;
    }
    // Update is called once per frame
    void Update()
    {
        Vector3 F = (transform.forward).normalized;
        Vector3 FR = (transform.forward + transform.forward + transform.right).normalized;
        Vector3 FL = (transform.forward + transform.forward - transform.right).normalized;
        Vector3 R = (transform.right + transform.right + transform.forward).normalized;
        Vector3 L = (-transform.right - transform.right + transform.forward).normalized;

        FSensor = distCheck(F);
        FRSensor = distCheck(FR);
        FLSensor = distCheck(FL);
        RSensor = distCheck(R);
        LSensor = distCheck(L);
    }
}
