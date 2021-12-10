using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlingshotBall : MonoBehaviour
{
    public static bool collided = false;
    private void OnCollisionEnter(Collision collision)
    {
        collided = true;
    }
}
