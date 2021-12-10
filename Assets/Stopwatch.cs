using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

public class Stopwatch : MonoBehaviour
{
    public SteamVR_Action_Boolean button = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Teleport");
    public Text display = null;

    float time = 0;
    float estimate = 0;
    bool running = false;

    void Start()
    {
        button.RemoveAllListeners(SteamVR_Input_Sources.Any);
        button.AddOnStateDownListener(Hit, SteamVR_Input_Sources.Any);
    }
    void Hit(SteamVR_Action_Boolean action, SteamVR_Input_Sources source)
    {
        running = !running;
        if (running)
        {
            time = Time.time;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        float rolltime = Time.time - Events.releaseTime;
        float error = estimate - rolltime;
        display.text += string.Format("\n{0}%", error / rolltime);
        Destroy(collision.gameObject);
    }

    void Update()
    {
        if (running && Events.releaseTime == 0)
        {
            estimate = Time.time - time;
        }
        display.text = string.Format("{0}s", estimate);
    }
}
