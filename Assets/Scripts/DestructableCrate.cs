using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableCrate : MonoBehaviour
{
    public static event EventHandler OnAnyDestroyed;

    private GridPosition gridPosition;
    [SerializeField] private Transform destroyedCreate;
    [SerializeField] private float explotionForce = 300f;
    [SerializeField] private float explotionRange = 10f;

    public GridPosition GetGridPosition() => gridPosition;

    private void Start()
    {
        gridPosition  = LevelGrid.Instance.GetGridPosition(transform.position);
    }

    public void Damage()
    {
        Transform root = Instantiate(destroyedCreate, transform.position, transform.rotation);

        Destroy(gameObject);

        ApplyExplosionToChieldren(root, explotionForce, root.transform.position, explotionRange);
        
        

        OnAnyDestroyed?.Invoke(this, EventArgs.Empty);
    }

    private void ApplyExplosionToChieldren(Transform root, float explosionForce,
        Vector3 explosionPosition, float explosionRange)
    {
        foreach (Transform child in root)
        {
            if (child.TryGetComponent<Rigidbody>(out Rigidbody childRB))
            {
                childRB.AddExplosionForce(explosionForce, explosionPosition, explosionRange);
            }

            ApplyExplosionToChieldren(child, explosionForce, explosionPosition, explosionRange);
        }
    }
}
