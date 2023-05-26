using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    [SerializeField] private float bulletSpeed = 200f;
    [SerializeField] private Transform trail;
    [SerializeField] private Transform bulletHitPrefab;
    private Vector3 targetPosition;
    void Update()
    {
        Vector3 moveDir = targetPosition - transform.position;

        float distanceBeforeMoving = Vector3.Distance(transform.position, targetPosition);

        transform.position += moveDir * bulletSpeed * Time.deltaTime;

        float distanceAfterMoving = Vector3.Distance(transform.position, targetPosition);

        if(distanceBeforeMoving < distanceAfterMoving || Vector3.Distance(targetPosition, transform.position) < 0.08)
        {
            transform.position = targetPosition;
            
            trail.parent = null;
            Destroy(gameObject);

            Instantiate(bulletHitPrefab, transform.position, Quaternion.identity);
        }
    }

    public void Setup(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }
}
