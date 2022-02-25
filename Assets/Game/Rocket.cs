using UnityEngine;
using TMPro;
using Utilities;

public class Rocket : MonoBehaviour
{
    [SerializeField] private Transform vfxHit;
    [SerializeField] private Transform vfxSmoke;
    private AudioSource soundRocketExplosion;
    private TextMeshProUGUI textScore;
    private Rigidbody myRigidbody;
    private float timeLastSmoke;
    private Score score;

    private void Awake()
    {
        score = GameObject.Find("/Canvas/Score").GetComponent<Score>();
        myRigidbody = GetComponent<Rigidbody>();
        textScore = GameObject.Find("/Canvas/Score").GetComponent<TextMeshProUGUI>();
        soundRocketExplosion = GameObject.Find("/Sound/RocketExplosion").GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        float speed = 80f;
        Vector3 dirSpeed = transform.forward;
        dirSpeed = Quaternion.Euler(5, 0, 0) * dirSpeed;
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
        if (!other.tag.Equals("Player") && !other.tag.Equals("Projectile"))
        {
            Vector3 explosionPos = transform.position;
            Collider[] colliders = Physics.OverlapSphere(explosionPos, 15.0f);

            foreach (Collider collider in colliders)
            {
                if (TransformUtilities.CheckHit(collider.transform))
                {
                    score.IncreaseScore();
                }
            }
            soundRocketExplosion.Play();
            Instantiate(vfxHit, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
