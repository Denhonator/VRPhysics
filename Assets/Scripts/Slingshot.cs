using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour
{
    [SerializeField] float shotSpeed = 20;
    [SerializeField] float fallSpeed = 2;
    [SerializeField] float maxStretch = 0.35f;
    [SerializeField] float shotDuration = 3.0f;
    [SerializeField] AudioClip release = null;

    public static bool canShoot = true;
    public static Vector3 shootPos = Vector3.zero;

    LineRenderer lr = null;
    Vector3[] lrverts = new Vector3[11];
    [SerializeField] Transform bowstring = null;
    Collider stringcol = null;
    [SerializeField] Transform arrow = null;
    Rigidbody arrowrb = null;
    Rigidbody stringrb = null;
    Collider arrowcol = null;
    Vector3 arrowRest = Vector3.zero;
    Quaternion arrowRestRot = Quaternion.identity;
    float stretchAmount = 0;
    float lastStretch = 0;
    Vector3 linestart = Vector3.zero;
    Vector3 lineend = Vector3.zero;
    bool readytolaunch = false;
    float shot = 0;
    Vector3 dist = Vector3.zero;
    [SerializeField] float hangDist = 1;
    [SerializeField] Transform tip = null;
    ParticleSystem trail = null;
    [SerializeField] LineRenderer aim = null;

    AudioSource ac = null;

    private void Awake()
    {

    }

    private void Start()
    {
        ac = GetComponent<AudioSource>();
        lr = GetComponent<LineRenderer>();
        lr.positionCount = lrverts.Length;
        stringcol = bowstring.GetComponent<Collider>();
        arrowrb = arrow.GetComponent<Rigidbody>();
        stringrb = bowstring.GetComponent<Rigidbody>();
        arrowcol = arrow.GetComponent<Collider>();
        arrowRest = arrow.localPosition;
        arrowRestRot = arrow.localRotation;
        bowstring.transform.parent = transform.parent;
        linestart = lr.GetPosition(0);
        lineend = lr.GetPosition(1);
        trail = arrow.GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            var em = trail.emission;
            em.enabled = !em.enabled;
        }

        dist = tip.position - bowstring.position;
        float distm = dist.magnitude;
        if (distm > maxStretch)
        {
            bowstring.position = tip.position - dist.normalized * maxStretch;
            distm = maxStretch;
        }
        if (distm >= hangDist)
        {
            if (distm < hangDist * 1.1f)
                stringrb.velocity = Vector3.Lerp(stringrb.velocity, (tip.position + Vector3.down * hangDist) - bowstring.position, 0.1f);
            else
            {
                stringrb.AddForce(dist, ForceMode.Impulse);
                stringrb.velocity *= 0.9f;
            }
            stringrb.useGravity = bowstring.position.y>tip.position.y;
        }
        else
            stringrb.useGravity = true;

        bowstring.tag = gameObject.layer == 8 ? "Untagged" : "Grabbable";
        stringcol.enabled = gameObject.layer == 10;
        stretchAmount = Mathf.Min(distm, maxStretch);
        aim.enabled = bowstring.gameObject.layer == 10;
        aim.SetPosition(0, aim.transform.position);
        aim.SetPosition(1, aim.transform.position + dist.normalized*100);
        Color c = Color.Lerp(Color.green, Color.red, stretchAmount / maxStretch);
        c.a = 0.2f;
        aim.sharedMaterial.color = c;
        if (Mathf.Abs(stretchAmount - lastStretch) > 0.03f && bowstring.gameObject.layer == 10)
        {
            lastStretch = stretchAmount;
            if (shot <= 0 || true)
            {
                ac.pitch = Mathf.Sqrt(1 + stretchAmount);
                ac.Play();
                VRHand.Vibrate(bowstring, 0, 0.2f, 80, stretchAmount * stretchAmount / maxStretch);
            }
        }

        if (shot > 0 && SlingshotBall.collided && stretchAmount > maxStretch*0.5f && bowstring.gameObject.layer == 10)
        {
            shot = 0.0001f;
        }

        if (readytolaunch && bowstring.gameObject.layer == 8)
            if (readytolaunch)
            {
                arrowrb.velocity = (tip.position-bowstring.position) * stretchAmount * shotSpeed;
                stringrb.velocity = arrowrb.velocity;
                arrowrb.angularVelocity = -transform.up * fallSpeed / arrowrb.velocity.magnitude * 1.3f;
                arrow.parent = transform.parent;
                shot = shotDuration;
                ac.PlayOneShot(release);
                trail.Play();
                SlingshotBall.collided = false;
                shootPos = transform.position;
            }
        readytolaunch = stretchAmount>maxStretch*0.5f && bowstring.gameObject.layer == 10 && shot <= 0 && canShoot;
        if (shot > 0)
        {
            trail.startSize = 0.2f + Vector3.Distance(arrow.position, transform.position) * 0.03f;
            shot -= Time.deltaTime;
            if (shot < shotDuration - 0.1f)
                arrowcol.enabled = true;
            if (shot < shotDuration - 0.05f)
            {
                //arrowrb.velocity += Vector3.down * Time.deltaTime * (fallSpeed / arrowrb.velocity.magnitude * 30);
                arrowrb.velocity += Vector3.up * Time.deltaTime * Physics.gravity.y;
            }
            if (shot <= 0)
            {
                arrow.parent = bowstring;
                arrowcol.enabled = false;
                trail.Stop();
            }
        }

        bowstring.parent = transform;

        for (int i = 0; i < lrverts.Length; i++)
        {
            lrverts[i] = Vector3.Lerp(linestart, lineend, (float)i / (lrverts.Length - 1));
            if (i != 0 && i != lrverts.Length - 1)
            {
                float linear = lrverts.Length / 2 - Mathf.Abs(i - lrverts.Length / 2);
                lrverts[i] = Vector3.Lerp(lrverts[i], bowstring.localPosition, linear*2.1f/lrverts.Length);
                //lrverts[i].z += Mathf.Log10(linear + 1) * stretchAmount;
                //lrverts[i].y += Mathf.Lerp(0, arrowRest.y, linear / 5);
                if (i == 5 && shot <= 0)
                {
                    arrow.localPosition = arrowRest;
                    arrow.localRotation = arrowRestRot;
                    arrowrb.velocity = Vector3.zero;
                    arrowrb.angularVelocity = Vector3.zero;
                }
            }
        }

        bowstring.parent = transform.parent;

        lr.SetPositions(lrverts);
    }
}
