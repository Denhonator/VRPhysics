using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowBall : MonoBehaviour
{
    public static bool hide = false;
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
            outofhand += Time.deltaTime;
        }
        else
        {
            outofhand = 0;
        }
        GetComponent<Renderer>().enabled = outofhand < 0.1f;

        if (Input.GetKeyDown(KeyCode.H))
        {
            hide = !hide;
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
