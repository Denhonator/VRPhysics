using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Events : MonoBehaviour
{
    float grav = 1;
    public float gravMod = 10;
    public static bool loading = true;
    public static float curGrav = 1;
    public static float releaseTime = 0;
    public static int slope = 0;
    public InputField inpfield = null;
    GameObject release = null;
    Rigidbody push = null;
    public List<Transform> slopes = new List<Transform>();

    void Start()
    {
        SceneManager.sceneLoaded += Loaded;
        SetSlope(0);
        loading = false;
        releaseTime = 0;

    }

    void SetSlope(int v)
    {
        for(int i = 0; i < slopes.Count; i++)
        {
            slopes[i].gameObject.SetActive(v == i);
        }
        //if (v > 0 && v < 3)
        //    release = slopes[v].GetChild(0).gameObject;
        //else
        //    release = null;
        //if (v == 4)
        //    push = slopes[v].GetChild(0).GetComponent<Rigidbody>();
        //else
        //    push = null;

        if (v == 1 || v == 2)
        {
            Score.moveTarget = slopes[v];
            ThrowBall.throws = 20;
        }

        //Slingshot.canShoot = v == 0 || v == 3;
        Time.timeScale = 1;
        Logger.Log("Activity: " + v.ToString());
        slope = v;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !inpfield.isFocused)
        {
            grav = grav == 1 ? gravMod : 1;
            curGrav = grav;
            Physics.gravity = Vector3.down * 9.81f * grav;
            Logger.Log("Gravity: " + curGrav.ToString());
        }
        for(int i = 0; i < 10; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0+i) && !inpfield.isFocused)
            {
                SetSlope(i);
            }
        }
        if (Input.GetKeyDown(KeyCode.Return) && release)
        {
            release.SetActive(false);
            releaseTime = Time.time;
        }
        if (Input.GetKeyDown(KeyCode.Return) && push)
        {
            if (Time.timeScale == 0)
                Time.timeScale = 1;
            else
                push.AddForce(Vector3.forward*10, ForceMode.Impulse);
        }
        if(Input.GetKeyDown(KeyCode.Backspace) && push && Time.timeScale == 0)
        {
            Time.timeScale = 1;
            push.transform.position = push.GetComponent<Rolling>().startPos;
            push.angularVelocity = Vector3.zero;
            push.velocity = Vector3.zero;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            loading = true;
            SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
        }
    }

    void Loaded(Scene s, LoadSceneMode l)
    {
        SceneManager.SetActiveScene(s);
        grav = 1;
        Physics.gravity = Vector3.down * 9.81f * grav;
        loading = false;
    }
}
