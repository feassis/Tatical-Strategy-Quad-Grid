using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GranadeProjectile : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 15f;
    [SerializeField] private float granadeRadious = 4f;
    [SerializeField] private float granadeDamage = 25f;
    [SerializeField] private GameObject granadeVFX;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private AnimationCurve archYAnomationCurve;

    public static event EventHandler OnAnyGranadeExploded;

    private const float reachedTargetDistance = 0.2f;
    private Vector3 targetPosition;

    private float totalDistance;
    private Vector3 posistionXZ;

    private Action onGranadeBehaviourComplete;
    public void Setup(GridPosition targetGridPosition, Action onGranadeBehaviourComplete)
    {
        targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition);
        this.onGranadeBehaviourComplete = onGranadeBehaviourComplete;

        posistionXZ = transform.position;
        posistionXZ.y = 0;
        totalDistance = Vector3.Distance(posistionXZ, targetPosition);
    }

    private void Update()
    {
        Vector3 moveDir = (targetPosition - posistionXZ).normalized;
        

        float distance = Vector3.Distance(posistionXZ, targetPosition);
        float distanceNormalized = 1 - distance / totalDistance;
        float maxHeight = totalDistance / 4f;
        float positionY = archYAnomationCurve.Evaluate(distanceNormalized)* maxHeight;
        posistionXZ += moveDir * moveSpeed * Time.deltaTime;

        transform.position = new Vector3(posistionXZ.x, positionY, posistionXZ.z);

        if(Vector3.Distance(posistionXZ, targetPosition) < reachedTargetDistance)
        {
            Collider[] colliderArray = Physics.OverlapSphere(targetPosition, granadeRadious);

            foreach (var collider in colliderArray)
            {
                if(collider.TryGetComponent<Unit>(out Unit targetUnit))
                {
                    targetUnit.Damage(granadeDamage);
                    continue;
                }

                if (collider.TryGetComponent<DestructableCrate>(out DestructableCrate crate))
                {
                    crate.Damage();
                    continue;
                }
            }

            onGranadeBehaviourComplete?.Invoke();
            OnAnyGranadeExploded?.Invoke(this, EventArgs.Empty);
            trailRenderer.transform.parent = null;
            Instantiate(granadeVFX, posistionXZ + Vector3.up * 1f, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
