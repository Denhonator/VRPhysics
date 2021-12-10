using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Catcher : MonoBehaviour
{

    public SteamVR_Action_Vector2 thumbstick = SteamVR_Input.GetAction<SteamVR_Action_Vector2>("Thumbstick");

    void Start()
    {

    }

    void Update()
    {
        if(Time.timeScale < 1)
            transform.localPosition += Vector3.forward * thumbstick.axis.x * 0.03f;
        if (transform.localPosition.z < 19)
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 19);
        else if (transform.localPosition.z > 41)
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 41);
    }
}
