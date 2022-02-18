using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

public class Grenade : MonoBehaviour
{
    [SerializeField] float angularVelocity = 10000.0f;
    [SerializeField] float lifeTime = 0.4f;
    [SerializeField] private Transform vfxExplosion;
    private AudioSource soundGrenadeExplosion;
    private Vector3 axisOfRotation;
    private Rigidbody myRigidbody;
    private Player playerScript;

    private void Awake()
    {
        playerScript = GameObject.Find("/Player").GetComponent<Player>();
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

            soundGrenadeExplosion.Play();
            Instantiate(vfxExplosion, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }

}
