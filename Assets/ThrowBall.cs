using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowBall : MonoBehaviour
{
    public static bool hide = false;
    public static int throws = -1;
    public static Vector3 throwPos = Vector3.zero;
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
        if ((gameObject.layer == 8 && hide && rb.velocity.sqrMagnitude > 0.1f) || (outofhand > 0 && outofhand < 3))
        {
            if (outofhand == 0)
                throwPos = transform.position;
            outofhand += Time.deltaTime;
        }
        else
        {
            outofhand = 0;
            Score.RandomTarget();
        }
        GetComponent<Renderer>().enabled = outofhand < 0.1f && throws != 0;

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
    }
}
