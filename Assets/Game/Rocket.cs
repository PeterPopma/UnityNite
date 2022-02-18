using System.Collections;
using System.Collections.Generic;
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
    private Player playerScript;

    private void Awake()
    {
        playerScript = GameObject.Find("/Player").GetComponent<Player>();
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
                Rigidbody rigidbody = collider.GetComponent<Rigidbody>();
                if (rigidbody != null)
                {
                    rigidbody.constraints = RigidbodyConstraints.None;
                    rigidbody.AddExplosionForce(700f, explosionPos, 4f, 10f);
                    rigidbody.AddTorque(transform.up * UnityEngine.Random.Range(20f, 80f), ForceMode.VelocityChange);
                    rigidbody.AddTorque(transform.right * UnityEngine.Random.Range(20f, 80f), ForceMode.VelocityChange);
                }
                else if (collider.GetComponent<Enemy>() != null)
                {
                    Enemy enemy = collider.gameObject.GetComponent<Enemy>();
                    enemy.Hit();
                    rigidbody = collider.gameObject.AddComponent<Rigidbody>();
                    rigidbody.AddExplosionForce(700f, new Vector3(collider.transform.position.x, collider.transform.position.y - 1, collider.transform.position.z), 4f);
                    rigidbody.AddTorque(new Vector3(UnityEngine.Random.Range(-500f, 500f), UnityEngine.Random.Range(-500f, 500f), UnityEngine.Random.Range(-500f, 500f)), ForceMode.VelocityChange);
                    rigidbody.useGravity = true;
                    // Detach gun
                    Transform gun = TransformUtilities.RecursiveFindChild(collider.gameObject.transform, "AKM");
                    gun.parent = null;
                    rigidbody = gun.gameObject.AddComponent<Rigidbody>();
                    rigidbody.AddExplosionForce(700f, new Vector3(collider.transform.position.x, collider.transform.position.y - 1, collider.transform.position.z), 4f);
                    rigidbody.AddTorque(new Vector3(UnityEngine.Random.Range(-500f, 500f), UnityEngine.Random.Range(-500f, 500f), UnityEngine.Random.Range(-500f, 500f)), ForceMode.VelocityChange);
                    rigidbody.useGravity = true;
                    gun.gameObject.AddComponent<BoxCollider>();
                    gun.gameObject.AddComponent<DeleteAfterDelay>();
                    playerScript.IncreaseScore();
                }
            }
            soundRocketExplosion.Play();
            Instantiate(vfxHit, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
