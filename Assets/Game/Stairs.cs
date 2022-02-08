using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stairs : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    void Update()
    {
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.tag.Equals("Player") && !other.tag.Equals("Projectile"))
        {
        }
    }
}
