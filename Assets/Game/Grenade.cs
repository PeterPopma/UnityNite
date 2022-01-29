using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] float angularVelocity = 10000.0f;
    [SerializeField] float lifeTime = 0.4f;
    [SerializeField] private Transform vfxExplosion;
    private AudioSource soundGrenadeExplosion;
    private Vector3 axisOfRotation;
    private Rigidbody myRigidbody;

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
            soundGrenadeExplosion.Play();
            Instantiate(vfxExplosion, transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }

}
