using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public Text score = null;
    public Text score2 = null;
    public bool isTarget = false;
    public static Transform moveTarget = null;
    public Transform measuringStick = null;
    public LineRenderer measuringLine = null;
    public LineRenderer measuringLine2 = null;
    public static bool measuring = false;
    public Transform trajectory = null;
    public Transform drop = null;
    public float randomRange = 14;
    static List<float> accuracy = new List<float>();
    static List<float> accuracy2 = new List<float>();
    static float cooldown = 0;
    static bool hashit = false;
    Vector3 startPos;

    void Start()
    {
        accuracy = new List<float>();
        accuracy2 = new List<float>();
        Random.InitState(0);
    }

    void Update()
    {
        if(cooldown > 0)
        {
            cooldown -= Time.deltaTime;
        }
        if(Input.GetKeyDown(KeyCode.M)){
            measuring = !measuring;
            measuringLine.gameObject.SetActive(measuring);
            measuringLine2.gameObject.SetActive(measuring);
            measuringStick.gameObject.SetActive(measuring);
            Logger.Log(measuring ? "Measurement visible" : "Measurement hidden");
        }
    }

    public static void RandomTarget()
    {
        if (Events.slope == 2 && Events.curGrav == 1 && hashit)
        {
            moveTarget.localPosition = new Vector3(-18.0f + Random.Range(-1.0f, 1.0f), moveTarget.localPosition.y, moveTarget.localPosition.z);
        }
        hashit = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (cooldown <= 0 && moveTarget && moveTarget.gameObject.activeInHierarchy)
        {
            if (isTarget)
                accuracy.Add(0);
            else
            {
                measuringStick.position = collision.GetContact(0).point;
                Vector3 toHit = collision.GetContact(0).point - ThrowBall.throwPos;
                Vector3 toTarget = moveTarget.position - ThrowBall.throwPos;
                toHit.y = 0;
                toTarget.y = 0;
                measuringLine.SetPositions(new Vector3[] {ThrowBall.throwPos, ThrowBall.throwPos+toHit});
                measuringLine2.SetPositions(new Vector3[] {ThrowBall.throwPos, ThrowBall.throwPos+toTarget});
                float diff = toHit.magnitude - toTarget.magnitude;
                //diff += diff > 0 ? -moveTarget.lossyScale.x * 0.5f : moveTarget.lossyScale.x * 0.5f;
                Logger.Log(string.Format("From target: {0}", diff));
                accuracy.Add(diff);
                hashit = true;
                ThrowBall.throws--;
            }
            //score.text = accuracy[accuracy.Count - 1] + "\n" + score.text;
            //score2.text = score.text;
            //collision.transform.position += Vector3.down * 20;
            //moveTarget.position = startPos + Vector3.forward * Random.Range(-randomRange, randomRange) + Vector3.right * Random.Range(-randomRange, randomRange);
            //if (isTarget)
            //    moveTarget.GetComponent<Renderer>().sharedMaterial.EnableKeyword("_EMISSION");
            //else
            //    moveTarget.GetComponent<Renderer>().sharedMaterial.DisableKeyword("_EMISSION");
            cooldown = 0.6f;
        }
        else if (trajectory.gameObject.activeInHierarchy)
        {
            //accuracy2.Add(collision.transform.localPosition.z - trajectory.localPosition.z);
            float calctime = (collision.transform.position.z - collision.transform.GetComponent<Rolling>().fallPos.z) / collision.transform.GetComponent<Rigidbody>().velocity.z;
            float guesstime = (trajectory.transform.position.z - collision.transform.GetComponent<Rolling>().fallPos.z) / collision.transform.GetComponent<Rigidbody>().velocity.z;
            float dropheight = collision.transform.GetComponent<Rolling>().fallPos.y - collision.transform.position.y;
            float calcgrav = 2*dropheight / (calctime * calctime) / 9.81f;
            float guessgrav = 2*dropheight / (guesstime * guesstime) / 9.81f;
            //Debug.Log(string.Format("Col time: {0} Col z: {1} Calc grav: {2}", calctime, collision.transform.position.z, calcgrav));
            //Debug.Log(string.Format("Guess time: {0} Guess z: {1} Guess grav: {2}", guesstime, trajectory.position.z, guessgrav));
            accuracy2.Add((guesstime - calctime) / calctime);
            //score.text = accuracy2[accuracy2.Count - 1] + "\n" + score.text;
            //score2.text = score.text;
            Destroy(collision.gameObject);
        }
        else if (drop.gameObject.activeInHierarchy)
        {
            float distError = collision.transform.localPosition.z - drop.localPosition.z;
            float timeError = distError / Timing.speed;
            float fallTime = Time.time - Timing.droptime;
            //accuracy2.Add(collision.transform.localPosition.z - drop.localPosition.z);
            accuracy2.Add(timeError / fallTime);
            //score.text = accuracy2[accuracy2.Count - 1] + "\n" + score.text;
            //score2.text = score.text;
            Destroy(collision.gameObject);
        }
    }
}
