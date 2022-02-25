using UnityEngine;
using Utilities;

public class Grenade : MonoBehaviour
{
    [SerializeField] float angularVelocity = 10000.0f;
    [SerializeField] float lifeTime = 0.4f;
    [SerializeField] private Transform vfxExplosion;
    private AudioSource soundGrenadeBounce;
    private AudioSource soundGrenadeExplosion;
    private Vector3 axisOfRotation;
    private Rigidbody myRigidbody;
    private Score score;

    private void Awake()
    {
        score = GameObject.Find("/Canvas/Score").GetComponent<Score>();
    }

    // Start is called before the first frame update
    void Start()
    {
        angularVelocity = 1000.0f;
        myRigidbody = GetComponent<Rigidbody>();
        float speed = 20f;
        Vector3 velocity = transform.forward * speed;
//        velocity.y = 10f;
        myRigidbody.velocity = velocity;
        //axisOfRotation = Random.onUnitSphere;
        axisOfRotation = new Vector3(1, 0.2f, 0.2f);
        soundGrenadeExplosion = GameObject.Find("/Sound/Grenade").GetComponent<AudioSource>();
        soundGrenadeBounce = GameObject.Find("/Sound/GrenadeBounce").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(axisOfRotation, angularVelocity * Time.smoothDeltaTime);

        lifeTime -= Time.deltaTime;
        if (lifeTime < 0f)
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

            soundGrenadeExplosion.Play();
            Instantiate(vfxExplosion, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        soundGrenadeBounce.Play();
    }

}
