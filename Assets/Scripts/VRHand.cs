using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class VRHand : MonoBehaviour
{
    public static float preventGrab = 0;
    public bool canConjure = true;
    public static bool canWind = true;
    public static Vector3 windPos = Vector3.zero;
    public static Vector3 windDir = Vector3.zero;
    public static float windTime = -50;
    public static float windDifficulty = 0.55f;
    static Dictionary<VRHand, Transform> currentlyHeld = new Dictionary<VRHand, Transform>();
    [SerializeField] Collider col = null;
    [SerializeField] float throwBoost = 1.5f;
    [SerializeField] float maxThrow = 7;
    [SerializeField] float rigidSpeed = 40;
    ParticleSystem poof = null;
    public static List<Rigidbody> windrbs = new List<Rigidbody>();
    Transform grabber = null;
    Transform grabPoint = null;
    Vector3 angDif = Vector3.zero;
    Vector3 lastAng = Vector3.zero;
    int angCounter = 0;
    List<Transform> nearObject = new List<Transform>();
    Transform pickedUpObject = null;
    Rigidbody pickedUpRigid = null;
    float delay = 0;
    Follow pickedUpFollow = null;

    public SteamVR_Action_Boolean grabGripAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabGrip");
    public SteamVR_Input_Sources hand;

    void Start()
    {
        grabber = transform.Find("GrabPoint");
        grabber = grabber ? grabber : transform;
        poof = GetComponentInChildren<ParticleSystem>();

        grabGripAction.RemoveAllListeners(hand);

        grabGripAction.AddOnStateDownListener(OnGrip, hand);
        grabGripAction.AddOnStateUpListener(UnGrip, hand);

        preventGrab = 0;
        nearObject = new List<Transform>();
    }

    public void OnGrip(SteamVR_Action_Boolean action, SteamVR_Input_Sources source)
    {
        if (nearObject.Count > 0 && nearObject[0].gameObject.layer == 8 && nearObject[0].tag == "Grabbable")
        {
            if(!nearObject[0].GetComponent<Renderer>() || nearObject[0].GetComponent<Renderer>().enabled)
                PickUp(nearObject[0]);
        }
        else if (nearObject.Count > 0 && nearObject[0].tag != "Grabbable")
            nearObject.Remove(nearObject[0]);
    }

    public void UnGrip(SteamVR_Action_Boolean action, SteamVR_Input_Sources source)
    {
        if(pickedUpObject)
            LetGo();
    }

    void Update()
    {
        ProcessHandObject();

        col.enabled = !pickedUpObject && delay <= 0;
        delay = Mathf.Max(delay - Time.deltaTime, 0);
        preventGrab = Mathf.Max(preventGrab - Time.deltaTime, 0);
    }

    private void FixedUpdate()
    {

    }

    void PickUp(Transform pobject)
    {
        pickedUpObject = pobject;
        pickedUpRigid = pickedUpObject.GetComponent<Rigidbody>();
        pickedUpFollow = pickedUpObject.GetComponent<Follow>();

        Transform grab = pickedUpObject.Find("grab");
        if (grab)
        {
            grabPoint = grab;
            grabber.localRotation = grabPoint.localRotation;
        }
        else
        {
            Transform grabdyn = pickedUpObject.Find("grabdyn");
            if (grabdyn)
            {
                //grabdyn.position = pickedUpObject.GetComponent<Collider>().bounds.ClosestPoint(transform.position);
                grabdyn.position = transform.position;
                grabber.rotation = pickedUpObject.rotation;
                grabPoint = grabdyn;
            }
            else
                grabPoint = pickedUpObject;
        }

        pickedUpObject.gameObject.layer = 10;
        if (pickedUpFollow)
            pickedUpFollow.enabled = false;
        if (pickedUpRigid)
            pickedUpRigid.constraints = RigidbodyConstraints.None;

        Transform togglecol = pickedUpObject.Find("togglecol");
        if (togglecol)
            togglecol.GetComponent<Collider>().enabled = false;

        currentlyHeld[this] = pickedUpObject;
    }

    void LetGo(bool force = false)
    {
        if (pickedUpObject.GetComponent<Slingshot>())
        {
            return;
        }
        if (pickedUpRigid)
        {
            pickedUpRigid.constraints = RigidbodyConstraints.None;
            pickedUpRigid.useGravity = true;
            pickedUpRigid.velocity *= throwBoost;
            float mag = pickedUpRigid.velocity.magnitude;
            pickedUpRigid.velocity = mag!=0 ? Mathf.Min(maxThrow, mag) * pickedUpRigid.velocity / mag : Vector3.zero;
            pickedUpRigid.angularVelocity = angDif / 180 * Mathf.PI * 24;
            pickedUpRigid = null;
        }

        if (pickedUpFollow)
            pickedUpFollow.enabled = true;

        nearObject.Remove(pickedUpObject);

        pickedUpObject.gameObject.layer = 8;

        Transform togglecol = pickedUpObject.Find("togglecol");
        if (togglecol)
            togglecol.GetComponent<Collider>().enabled = true;

        pickedUpObject = null;
        delay = 0.3f;

        currentlyHeld[this] = pickedUpObject;
    }

    public static void LetGoOf(Transform pobject, bool triggerPoof = false)
    {
        VRHand target = null;
        foreach (KeyValuePair<VRHand, Transform> entry in currentlyHeld)
        {
            if (entry.Value == pobject)
            {
                target = entry.Key;
            }
        }
        if (target)
        {
            target.LetGo();
        }
    }

    public static void Vibrate(Transform o, float delay, float dur, float freq, float amp)
    {
        VRHand target = null;
        foreach (KeyValuePair<VRHand, Transform> entry in currentlyHeld)
        {
            if (entry.Value == o)
            {
                target = entry.Key;
            }
        }
        if (target)
        {
            SteamVR_Actions.default_Haptic[target.hand].Execute(delay, dur, freq, amp);
        }
    }

    void ProcessHandObject()
    {
        if (pickedUpRigid)
        {
            pickedUpRigid.velocity = Vector3.zero;
            pickedUpRigid.angularVelocity = Vector3.zero;
        }

        if (pickedUpObject)
        {
            Vector3 posBefore = grabPoint.position;
            pickedUpObject.rotation = grabber.rotation;
            pickedUpObject.position += posBefore - grabPoint.position;

            if (pickedUpRigid)
            {
                pickedUpRigid.useGravity = false;
                pickedUpRigid.velocity += (transform.position - grabPoint.position) * rigidSpeed;
            }
            else
                pickedUpObject.position = Vector3.Lerp(pickedUpObject.position, 2 * transform.position - grabPoint.position, 0.3f);
        }

        if (angCounter <= 0)
        {
            angDif = lastAng - transform.rotation.eulerAngles;
            lastAng = transform.rotation.eulerAngles;
            angDif.x = angDif.x < -180 ? angDif.x + 360 : angDif.x > 180 ? angDif.x - 360 : angDif.x;
            angDif.y = angDif.y < -180 ? angDif.y + 360 : angDif.y > 180 ? angDif.y - 360 : angDif.y;
            angDif.z = angDif.z < -180 ? angDif.z + 360 : angDif.z > 180 ? angDif.z - 360 : angDif.z;
            angCounter = 3;
        }
        else
            angCounter -= 1;
    }
    
    void OnTriggerEnter(Collider other)
    {
        //Logger.ShowText(Time.unscaledTime.ToString() + ": " + other.name + " entered", 3);
        if (other.tag == "Grabbable" && !nearObject.Contains(other.transform) && !Events.loading)
            nearObject.Insert(0, other.transform);
    }

    void OnTriggerExit(Collider other)
    {
        //Logger.ShowText(Time.unscaledTime.ToString() + ": Tried to remove " + other.name, 3);
        nearObject.Remove(other.transform);        
    }
}
