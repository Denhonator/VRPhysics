using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public Text score = null;
    public Text score2 = null;
    public bool isTarget = false;
    public Transform moveTarget = null;
    public Transform trajectory = null;
    public Transform drop = null;
    public float randomRange = 14;
    static List<float> accuracy = new List<float>();
    static List<float> accuracy2 = new List<float>();
    static float cooldown = 0;
    Vector3 startPos;

    void Start()
    {
        startPos = moveTarget.position;
        accuracy = new List<float>();
        accuracy2 = new List<float>();
        Random.InitState(0);
    }

    void Update()
    {
        if(isTarget && cooldown > 0)
        {
            cooldown -= Time.deltaTime;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (cooldown <= 0 && moveTarget.gameObject.activeInHierarchy)
        {
            if (isTarget)
                accuracy.Add(0);
            else
            {
                Vector3 toHit = collision.GetContact(0).point - Slingshot.shootPos;
                Vector3 toTarget = moveTarget.position - Slingshot.shootPos;
                toHit.y = 0;
                toTarget.y = 0;
                float diff = toHit.magnitude - toTarget.magnitude;
                diff += diff > 0 ? -moveTarget.lossyScale.x * 0.5f : moveTarget.lossyScale.x * 0.5f;
                Logger.Log(string.Format("From target: {0}", diff));
                accuracy.Add(diff);
            }
            score.text = accuracy[accuracy.Count - 1] + "\n" + score.text;
            score2.text = score.text;
            collision.transform.position += Vector3.down * 20;
            moveTarget.position = startPos + Vector3.forward * Random.Range(-randomRange, randomRange) + Vector3.right * Random.Range(-randomRange, randomRange);
            if (isTarget)
                moveTarget.GetComponent<Renderer>().sharedMaterial.EnableKeyword("_EMISSION");
            else
                moveTarget.GetComponent<Renderer>().sharedMaterial.DisableKeyword("_EMISSION");
            cooldown = 1;
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
            score.text = accuracy2[accuracy2.Count - 1] + "\n" + score.text;
            score2.text = score.text;
            Destroy(collision.gameObject);
        }
        else if (drop.gameObject.activeInHierarchy)
        {
            float distError = collision.transform.localPosition.z - drop.localPosition.z;
            float timeError = distError / Timing.speed;
            float fallTime = Time.time - Timing.droptime;
            //accuracy2.Add(collision.transform.localPosition.z - drop.localPosition.z);
            accuracy2.Add(timeError / fallTime);
            score.text = accuracy2[accuracy2.Count - 1] + "\n" + score.text;
            score2.text = score.text;
            Destroy(collision.gameObject);
        }
    }
}
