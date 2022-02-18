using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using TMPro;
using Utilities;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Transform vfxHit;
    private TextMeshProUGUI textScore;
//    private Rigidbody myRigidbody;

    private void Awake()
    {
//        myRigidbody = GetComponent<Rigidbody>();
        textScore = GameObject.Find("/Canvas/Score").GetComponent<TextMeshProUGUI>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //        myRigidbody.velocity = transform.forward * 200f;
        //        transform.Rotate(Vector3.forward, -90);
    }

    private void Update()
    {
        transform.position += transform.forward * Time.deltaTime * 20f;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.tag.Equals("Player") && !other.tag.Equals("Projectile"))
        {
            //            bulletVirtualCamera.gameObject.SetActive(false);
            Instantiate(vfxHit, transform.position, Quaternion.identity);
            TransformUtilities.CheckEnemyHit(other.transform);
            Destroy(gameObject);
        }
    }
}
