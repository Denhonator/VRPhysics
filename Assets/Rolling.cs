using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rolling : MonoBehaviour
{
    public float maxRoll = 1000;
    public float slowdown = 0;
    public Vector3 fallPos = Vector3.zero;
    public Vector3 startPos = Vector3.zero;
    float lastVel = 0;

    void Start()
    {
        GetComponent<Rigidbody>().maxAngularVelocity = maxRoll;
        startPos = transform.position;
    }

    void Update()
    {
        if (slowdown > 0)
        {
            GetComponent<Rigidbody>().angularVelocity *= slowdown;
        }
        if(GetComponent<Rigidbody>().velocity.y != lastVel && lastVel > -0.001f)
        {
            fallPos = transform.position;
            //Debug.Log(string.Format("Fall time: {0} Fall z: {1}", Time.unscaledTime, transform.position.z));
        }
        lastVel = GetComponent<Rigidbody>().velocity.y;
    }
}
