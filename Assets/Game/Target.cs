using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] private Transform vfxHit;
    private AudioSource soundSmash;

    // Start is called before the first frame update
    void Start()
    {
        soundSmash = GameObject.Find("/Sound/Smash").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Hit(Vector3 hitPosition)
    {
        soundSmash.Play();
        if (!hitPosition.Equals(Vector3.zero))
        {
            Instantiate(vfxHit, hitPosition, vfxHit.transform.rotation);
        }
    }
}
