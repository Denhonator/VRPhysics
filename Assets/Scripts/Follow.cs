using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform target = null;
    [SerializeField] float speed = 0.3f;
    [SerializeField] Vector3 globalOffset = Vector3.zero;
    [SerializeField] Vector3 localOffset = Vector3.zero;
    [SerializeField] Vector3 rotate = Vector3.zero;
    [SerializeField] Vector3 rotationOffset = Vector3.zero;
    [SerializeField] bool hideNear = true;
    Rigidbody rb = null;
    Collider col = null;
    Renderer r = null;

    void Start()
    {
        col = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        r = GetComponent<Renderer>();
    }

    void Update()
    {
        Vector3 dist = target.position - transform.position + globalOffset + transform.forward*localOffset.z + transform.right*localOffset.x + transform.up*localOffset.y;

        if (rb)
        {
            if (dist.sqrMagnitude > 0.2f)
                col.enabled = false;
            rb.velocity = dist * speed * 10;
        }
        else
        {
            transform.position += dist * speed;
        }

        Vector3 thisrot = transform.rotation.eulerAngles;
        Vector3 targetrot = target.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(rotate.x != 0 ? targetrot.x : thisrot.x, rotate.y != 0 ? targetrot.y + rotationOffset.y : thisrot.y, rotate.z != 0 ? targetrot.z : thisrot.z);

        if(r&&hideNear)
            r.enabled = !hideNear || dist.sqrMagnitude > 0.05f;
    }
}
