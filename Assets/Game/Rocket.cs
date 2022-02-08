using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Rocket : MonoBehaviour
{
    [SerializeField] private Transform vfxHit;
    [SerializeField] private Transform vfxSmoke;
    private AudioSource soundRocketExplosion;
    private TextMeshProUGUI textScore;
    private Rigidbody myRigidbody;
    private float timeLastSmoke;

    private void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
        textScore = GameObject.Find("/Canvas/Score").GetComponent<TextMeshProUGUI>();
        soundRocketExplosion = GameObject.Find("/Sound/RocketExplosion").GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        float speed = 80f;
        Vector3 dirSpeed = transform.forward;
        dirSpeed = Quaternion.Euler(10, 0, 0) * dirSpeed;
        myRigidbody.velocity = dirSpeed * speed;
//        transform.Rotate(Vector3.left, 90);
    }

    private void Update()
    {
        if (Time.time >= timeLastSmoke + 0.04f)
        {
            timeLastSmoke = Time.time;
            Instantiate(vfxSmoke, transform.position, Quaternion.identity);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Target>() != null)
        {
            Rigidbody rigidbody = other.gameObject.GetComponent<Rigidbody>();
            rigidbody.AddExplosionForce(500f, new Vector3(other.transform.position.x, other.transform.position.y, other.transform.position.z), 3f);
            Vector3 randomRotation = new Vector3(Random.Range(30f, 600f), Random.Range(30f, 600f), Random.Range(30f, 600f));
            rigidbody.AddRelativeTorque(randomRotation, ForceMode.Impulse);
            rigidbody.useGravity = true;

            int score = System.Convert.ToInt32(textScore.text);
            score++;
            textScore.text = score.ToString();
        }

        if (!other.tag.Equals("Player") && !other.tag.Equals("Projectile"))
        {
            soundRocketExplosion.Play();
            Instantiate(vfxHit, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
