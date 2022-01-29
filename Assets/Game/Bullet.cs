using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using TMPro;

public class BulletProjectile : MonoBehaviour
{
    [SerializeField] private Transform vfxHit;
    private TextMeshProUGUI textScore;
    private Rigidbody myRigidbody;

    private void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
        textScore = GameObject.Find("/Canvas/Score").GetComponent<TextMeshProUGUI>();
    }

    // Start is called before the first frame update
    void Start()
    {
        float speed = 80f;
        myRigidbody.velocity = transform.forward * speed;
//        transform.Rotate(Vector3.left, 90);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<BulletTarget>() != null)
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
            //            bulletVirtualCamera.gameObject.SetActive(false);
            Instantiate(vfxHit, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
