using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] private Transform vfxHit;
    private AudioSource soundSmash;
    private Score score;

    // Start is called before the first frame update
    void Start()
    {
        score = GameObject.Find("/Canvas/Score").GetComponent<Score>();
        soundSmash = GameObject.Find("/Sound/Smash").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Hit(Vector3 hitPosition)
    {
        score.IncreaseScore(5);
        soundSmash.Play();
        if (!hitPosition.Equals(Vector3.zero))
        {
            Instantiate(vfxHit, hitPosition, vfxHit.transform.rotation);
        }
    }
}
