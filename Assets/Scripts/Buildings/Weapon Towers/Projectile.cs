using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 15f, verticalOffset = 0.6f;

    private Enemy target;
    private int damage;

    public void Initialize(Enemy target, int damage)
    {
        this.target = target;
        this.damage = damage;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 direction = (target.transform.position + new Vector3(0, verticalOffset, 0) - transform.position).normalized;

        transform.position += direction * speed * Time.deltaTime;

        transform.rotation = Quaternion.LookRotation(direction);

        if (Vector3.Distance(transform.position, target.transform.position + new Vector3(0, verticalOffset, 0)) < 0.3f) HitTarget();
    }

    private void HitTarget()
    {
        if (target != null) target.TakeDamage(damage);

        Destroy(gameObject);
    }
}
