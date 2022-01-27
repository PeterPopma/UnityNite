using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] private Transform vfxHit;
    private Rigidbody myRigidbody;
    public float angularVelocity = 10000.0f;
    private Vector3 axisOfRotation;

    // Start is called before the first frame update
    void Start()
    {
        angularVelocity = 1000.0f;
        myRigidbody = GetComponent<Rigidbody>();
        float speed = 20f;
        Vector3 velocity = transform.forward * speed;
        velocity.y = 10f;
        myRigidbody.velocity = velocity;
        //axisOfRotation = Random.onUnitSphere;
        axisOfRotation = new Vector3(1, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(axisOfRotation, angularVelocity * Time.smoothDeltaTime);
    }
}
