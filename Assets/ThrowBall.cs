using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowBall : MonoBehaviour
{
    public static bool hide = false;
    public static int throws = -1;
    public static Vector3 throwPos = Vector3.zero;
    public LineRenderer measuringLine = null;
    public LineRenderer measuringLine2 = null;
    float outofhand = 0;
    Rigidbody rb = null;
    Vector3 startPos = Vector3.zero;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPos = transform.position;
    }

    void Update()
    {
        if ((gameObject.layer == 8 && transform.position != startPos) || (outofhand > 0 && outofhand < 3))
        {
            if (outofhand == 0){
                throwPos = transform.position;
            }
            outofhand += Time.deltaTime;
        }
        else
        {
            outofhand = 0;
            if(!Score.measuring || transform.position != startPos)
                Score.RandomTarget();
        }
        GetComponent<Renderer>().enabled = (outofhand < 0.1f || !hide) && throws != 0;

        measuringLine.GetComponent<Renderer>().enabled = transform.position == startPos;
        measuringLine2.GetComponent<Renderer>().enabled = transform.position == startPos;

        if (Input.GetKeyDown(KeyCode.H))
        {
            hide = !hide;
            outofhand = 0;
            Logger.Log(hide ? "Hidden ball" : "Visible ball");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = startPos;
        outofhand = hide ? outofhand : 0;
    }
}
