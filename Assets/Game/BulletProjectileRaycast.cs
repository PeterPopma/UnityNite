using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BulletProjectileRaycast : MonoBehaviour
{
    [SerializeField] private Transform vfxHit;
    private Vector3 targetPosition;

    public void Setup(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }

    // Update is called once per frame
    void Update()
    {
        float distanceBefore = Vector3.Distance(transform.position, targetPosition);

        Vector3 moveDir = (targetPosition - transform.position).normalized;
        float moveSpeed = 200f;
        transform.position += moveDir * moveSpeed * Time.deltaTime;

        float distanceAfter = Vector3.Distance(transform.position, targetPosition);

        // if the trail moved passed the object it hits, it should be removed
        if (distanceBefore < distanceAfter)
        {
//            Instantiate(vfxHit, targetPosition, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
