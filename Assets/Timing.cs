using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

public class Timing : MonoBehaviour
{
    [SerializeField] bool dry = true;
    public static float droptime = 0;
    public static float speed = 10.0f;
    public Transform sphere = null;
    public SteamVR_Action_Boolean button = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Teleport");

    void Release(SteamVR_Action_Boolean action, SteamVR_Input_Sources source)
    {
        Debug.Log("Button");
        if (gameObject.activeInHierarchy && sphere.gameObject.activeInHierarchy && 
            Mathf.Abs((transform.localPosition.z+speed*Time.deltaTime)-sphere.localPosition.z) < Mathf.Abs(transform.localPosition.z - sphere.localPosition.z))
        {
            if (dry)
            {
                float platformUpper = transform.position.y + transform.lossyScale.y * 0.25f;
                float sphereLower = sphere.position.y - sphere.lossyScale.y * 0.5f;
                float dist = sphereLower - platformUpper;
                float sphereTime = Mathf.Sqrt(2 * dist / Mathf.Abs(Physics.gravity.y));
                float platformDist = transform.position.z - sphere.position.z;
                float platformTime = Mathf.Abs(platformDist / speed);
                sphere.position = new Vector3(sphere.position.x, transform.position.y + Random.Range(5.0f, 20.0f), sphere.position.z);
                Logger.Log(string.Format("Time: {3}\nFall: {0}s\nPlat: {1}s\nError: {2}s", sphereTime, platformTime, sphereTime - platformTime, System.DateTime.Now));
            }
            else
            {
                sphere.GetComponent<Rigidbody>().useGravity = true;
                droptime = Time.time;
                Debug.Log("Release");
            }
        }
    }

    private void OnEnable()
    {
        button.RemoveAllListeners(SteamVR_Input_Sources.Any);
        button.AddOnStateDownListener(Release, SteamVR_Input_Sources.Any);
    }
    void Update()
    {
        transform.localPosition += Vector3.forward * speed * Time.deltaTime;
        if (speed > 0 && transform.localPosition.z > 41)
            speed = -speed;
        else if (speed < 0 && transform.localPosition.z < -5.5f)
            speed = -speed;
        //if(Mathf.Abs(transform.localPosition.z - release.transform.localPosition.z) < bigThreshold)
        //    Release(button, SteamVR_Input_Sources.Any);
    }
}
