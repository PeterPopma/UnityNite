using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    private Rigidbody myRigidbody;
    public float angularVelocity = 100.0f;
    private Vector3 axisOfRotation;
    private AudioSource soundShell;
    private void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
        soundShell = GameObject.Find("/Sound/Shell").GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //rigidbody.velocity = new Vector3((Random.value / 1f) + 0.5f, 3f, (Random.value / 1f) + 0.5f);
        var velocity = transform.right * ((Random.value / 1f) + 0.5f);
        velocity = new Vector3(velocity.x, velocity.y + 3f, velocity.z);
        axisOfRotation = Random.onUnitSphere;
        myRigidbody.velocity = velocity;
    }

    void Update()
    {
        transform.Rotate(axisOfRotation, angularVelocity * Time.smoothDeltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.tag.Equals("Player") && !other.tag.Equals("Projectile"))
        {
            soundShell.Play();
            Destroy(gameObject);
        }
    }
}
